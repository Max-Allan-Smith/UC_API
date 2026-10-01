using UC_API.Application.Products;
using UC_API.Domain.Products;

namespace UC_API.Infrastructure.Products
{
    public sealed class InMemoryProductRepository : IProductRepository
    {
        private static readonly Product[] Products =
        [
            new(Guid.NewGuid(), "Laptop", 999.00m),
            new(Guid.NewGuid(), "Mouse", 25.00m)
        ];

        public Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Product>>(Products);
        }
    }
}