using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using UC_API.Application.Products;
using UC_API.Infrastructure.Products;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();

builder.Services.AddScoped<GetProducts>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("https://127.0.0.1:7214/");

        options.AddAudiences("resource-api");

        options
            .UseIntrospection()
            .SetClientId("resource-api")
            .SetClientSecret(
                builder.Configuration["Authentication:IntrospectionSecret"]!);

        options.UseSystemNetHttp();

        options.UseAspNetCore();
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiAccess", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireAssertion(context => context.User.HasScope("api"));
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

builder.Services.AddHealthChecks();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.UseHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();