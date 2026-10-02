using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UC_API.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}