namespace SellenceAdmin.Models.Interfaces;

public interface IProduct
{
    Guid Id { get; }

    string Name { get; }

    bool IsActive { get; }
}
