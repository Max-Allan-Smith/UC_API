using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UC_API.Infrastructure.Persistence;

namespace UC_API.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Ef Core
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });

            // OpenIddict
            services.AddOpenIddict()
                .AddValidation(options =>
                {
                    options.SetIssuer("https://127.0.0.1:7214/");

                    options.AddAudiences("resource-api");

                    options
                        .UseIntrospection()
                        .SetClientId("resource-api")
                        .SetClientSecret(configuration["Authentication:IntrospectionSecret"] ??
                                         throw new KeyNotFoundException(
                                             "The Introspection Secret has not been configured."));

                    options.UseSystemNetHttp();

                    options.UseAspNetCore();
                });

            return services;
        }
    }
}