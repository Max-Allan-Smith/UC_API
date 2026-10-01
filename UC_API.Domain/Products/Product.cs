namespace UC_API.Domain.Products
{
    public record Product(
        Guid Id,
        string Name,
        decimal Price
    );
}