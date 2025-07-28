using SellenceAdmin.Models.Interfaces;

namespace SellenceAdmin.Models;

public sealed class Product : IProduct
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public bool IsActive { get; set; }
}