#nullable enable

using Dealtje.Backend.CQRS2.Models.Accounts;
using Dealtje.Backend.CQRS2.Models.ProductImages;
using Dealtje.Backend.CQRS2.Queries.Account.Orders;
using Dealtje.Shared.CoreFeatures;
using Dealtje.Shared.Extensions;
using Dealtje.Sites.DataBroker.DAL.Expressions;
using Dealtje.Sites.DataBroker.Database;
using LinqKit;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Dealtje.Sites.DataBroker.CQRS2.QueryHandlers.Account.Orders;

internal sealed class AccountOrdersRequestHandler : IQueryHandler<AccountOrdersRequest, AccountOrdersResponse>
{
    private readonly IDealtjeBackendDatabaseSelector selector;

    public AccountOrdersRequestHandler(IDealtjeBackendDatabaseSelector selector)
    {
        this.selector = selector;
    }

    public async ValueTask<AccountOrdersResponse> Handle(AccountOrdersRequest request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 10, 150);
        var query = request.Query.Trim();

        var db = this.selector.GetInstanceFor(request);
        var autoLoginUrl = OrderExpressions.AutomaticLoginUrl();
        var ordersQuery = db.GetOrders()
            .Where(o => o.ShopUserId == request.Id);

        if (!string.IsNullOrEmpty(query) && int.TryParse(query, out var id))
        {
            ordersQuery = ordersQuery.Where(o => o.Id == id || o.OrderProductEditions.Any(ope => ope.ProductEdition.ProductId == id));
        }

        if (request.StartDate.HasValue)
        {
            ordersQuery = ordersQuery.Where(x => x.Created >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            ordersQuery = ordersQuery.Where(x => x.Created < request.EndDate.Value.AddDays(1));
        }

        var total = await ordersQuery.CountAsync(cancellationToken);
        var orders = await ordersQuery
            .AsNoTracking()
            .AsExpandableEFCore()
            .OrderByDescending(o => o.Created)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new
            {
                o.Id,
                o.Created,
                o.Status,
                o.SellPriceCurrency,
                o.ShippingMethodPrice,
                OrderId = o.CompanyId + "-" + o.Id,
                Url = autoLoginUrl.Invoke(o),
                ProductEditionIds = o.OrderProductEditions.Select(ope => ope.Id).ToHashSet(),
                Discounts = o.OrderDiscounts.Select(d => d.DiscountPricePerVoucher).ToList(),
                TransactionCosts = o.OrderPaymentTransactions.Select(t => t.TransactionCosts).ToList(),
                ProductConfigIds = o.OrderProductConfigs.Select(op => op.Id).ToHashSet(),
            }).ToListAsync(cancellationToken);

        if (orders.Count == 0)
        {
            return new AccountOrdersResponse();
        }

        var orderProductConfigIds = orders
            .SelectMany(o => o.ProductConfigIds)
            .ToHashSet();

        var orderProductConfigs = await db.GetOrderProductConfigs()
            .Where(o => orderProductConfigIds.Contains(o.Id))
            .Select(o => new
            {
                o.Id,
                o.Configuration.ProductId,
                o.Configuration.Amount,
                Editions = o.Configuration.Editions.Select(e => new
                {
                    e.EditionId,
                    e.Amount,
                    e.SellPrice,
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        var orderProductEditionIds = orders
            .SelectMany(o => o.ProductEditionIds)
            .ToHashSet();

        var orderProductEditions = await db.GetOrderProductEditions()
            .Where(o => orderProductEditionIds.Contains(o.Id))
            .Select(o => new
            {
                o.Id,
                o.ProductEdition.ProductId,
                o.ProductEditionId,
                o.Amount,
                o.SellPrice,
            })
            .ToDictionaryAsync(o => o.Id, cancellationToken);

        var company = await db.GetCompanies()
          .Select(x => new
          {
              x.Id,
              x.DefaultLanguage,
              x.Currency
          })
          .FirstOrDefaultAsync(x => x.Id == request.Company.CompanyId, cancellationToken);

        var language = request.Language.ToLower();
        if (string.IsNullOrEmpty(language))
        {
            language = company.DefaultLanguage.ToLower();
        }

        var productIds = orderProductConfigs.Select(o => o.Value.ProductId).ToHashSet();
        productIds.UnionWith(orderProductEditions.Select(o => o.Value.ProductId));

        var products = await db.GetProducts()
            .AsNoTracking()
            .AsSingleQuery()
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new
            {
                p.Id,
                p.Name,
                Image = p.ProductImages.OrderBy(pi => pi.ViewOrder).Select(pi => new
                {
                    pi.ProductImageId,
                    pi.ProductImage.Name,
                    pi.ProductImage.BlobMd5,
                    pi.ProductImage.BlobVersion,
                    pi.ProductImage.MimeType,
                    pi.ProductImage.VideoId,
                    pi.ProductImage.VideoUrl
                }).FirstOrDefault(),
            }).ToDictionaryAsync(p => p.Id, cancellationToken);

        var productTranslations = await db.GetProductTranslations()
            .AsNoTracking()
            .AsSingleQuery()
            .Where(pt => productIds.Contains(pt.ProductId))
            .Select(pt => new
            {
                pt.ProductId,
                pt.Title,
                Language = pt.Language.ToLower()
            }).ToDictionaryAsync(pt => (pt.ProductId, pt.Language), cancellationToken);

        var ordersResponse = new List<AccountOrderResponseModel>();
        foreach (var order in orders)

        {

            var currency = Currency.GetCurrency(order.SellPriceCurrency ?? company.Currency);
            var shippingCost = order.ShippingMethodPrice;
            var discount = order.Discounts.Sum();
            var transactionCost = order.TransactionCosts.Sum();
            var subTotal = 0.00m;
            var serviceFee = 0.00m;
            var totalPrice = 0.00m;
            var productsResponse = new List<AccountOrderProductResponseModel>();

            var orderNonConfigurationEditions = order.ProductEditionIds
                .Where(orderProductEditions.ContainsKey)
                .Select(key => orderProductEditions[key])
                .ToList();

            var orderConfigurationProducts = orderProductConfigs
                .Where(opc => order.ProductConfigIds.Contains(opc.Id))
                .ToList();

            if (orderConfigurationProducts.Count > 0)
            {
                foreach (var productConfig in orderConfigurationProducts)
                {

                    if (!products.TryGetValue(productConfig.ProductId, out var product))
                    {
                        continue;
                    }

                    var productTotalPrice = 0.00m;
                    productTotalPrice = productConfig.Sum(pc => pc.Editions.Sum(pce => pce.SellPrice * pce.Amount) * pc.Amount);


                    var productName = product.Name;
                    if (productTranslations.TryGetValue((product.Id, language), out var translation))
                    {
                        productName = translation.Title;
                    }

                    var productImage = product.Image == null
                        ? null
                        : new ProductImageResponseModel
                        {
                            Id = product.Image.ProductImageId,
                            Etag = product.Image.BlobMd5,
                            Version = product.Image.BlobVersion,
                            MimeType = product.Image.MimeType,
                            VideoId = product.Image.VideoId,
                            VideoUrl = product.Image.VideoUrl

                        };

                    productsResponse.Add(new AccountOrderProductResponseModel
                    {
                        Id = product.Id,
                        Name = productName,
                        Amount = productConfig.Amount,
                        Image = productImage,
                        Price = new Money(productTotalPrice * productAmount, currency)
                    });
                }
            }

            if (orderNonConfigurationEditions.Any())
            {
                foreach (var productNonConfig in orderNonConfigurationEditions)
                {
                    if (!products.TryGetValue(productNonConfig.ProductId, out var product))
                    {
                        continue;
                    }

                    var productTotalPrice = 0.00m;
                    var productAmount = 0;
                    productTotalPrice = productNonConfig.SellPrice.Amount;
                    productAmount = productNonConfig.Amount;
                    var productName = product.Name;
                    if (productTranslations.TryGetValue((product.Id, language), out var translation))
                    {
                        productName = translation.Title;
                    }

                    var productImage = product.Image == null
                        ? null
                        : new ProductImageResponseModel
                        {
                            Id = product.Image.ProductImageId,
                            Etag = product.Image.BlobMd5,
                            Version = product.Image.BlobVersion,
                            MimeType = product.Image.MimeType,
                            VideoId = product.Image.VideoId,
                            VideoUrl = product.Image.VideoUrl

                        };

                    productsResponse.Add(new AccountOrderProductResponseModel
                    {
                        Id = product.Id,
                        Name = productName,
                        Amount = productAmount,
                        Image = productImage,
                        Price = new Money(productTotalPrice, currency)
                    });
                }
            }

            ////totalPrice = subTotal + serviceFee + shippingCost + transactionCost - discount;

            totalPrice = productsResponse.Sum(p => p.Price.Amount);
            ordersResponse.Add(new AccountOrderResponseModel
            {
                Id = order.Id,
                OrderId = order.OrderId,
                Url = order.Url,
                Status = order.Status,
                Created = order.Created,
                TotalPrice = new Money(totalPrice, currency),
                Products = productsResponse
            });
        }
    }
}
