using UC_API.Domain.Products;

namespace UC_API.Application.Products
{
    public interface IProductRepository
    {
        Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}