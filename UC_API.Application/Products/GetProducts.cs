using UC_API.Domain.Products;

namespace UC_API.Application.Products
{
    public sealed class GetProducts(IProductRepository productRepository)
    {
        public Task<IReadOnlyCollection<Product>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return productRepository.GetAllAsync(cancellationToken);
        }
    }
}