# UC_API

UC_API is a .NET resource server built with ASP.NET Core and OpenIddict. It provides a secured REST API that validates bearer tokens from a central OpenID Connect authorization server (UC_Auth) and enforces scope-based access control.

This API is designed to be part of a broader ecosystem where authentication and authorization are managed centrally, allowing it to focus on resource delivery while trusting the issuer for token validation.

## What this project does

This repository implements a secured REST API that can:

- receive and validate OpenID Connect bearer tokens from client applications
- introspect tokens with the central authorization server to verify validity and scope
- enforce authorization policies based on token claims and requested scopes
- serve product data through a simple REST endpoint
- delegate authentication to a configured OpenIddict issuer via token introspection

In practical terms, this is a resource server that trusts tokens issued by UC_Auth and validates them on every request before serving API data.

## Solution structure

- `UC_API.WebApi`
  - ASP.NET Core API host
  - controllers and endpoint definitions
  - app startup and middleware configuration

- `UC_API.Application`
  - application layer use cases
  - service contracts for repositories
  - business logic orchestration

- `UC_API.Infrastructure`
  - persistence implementations (in-memory repository for this example)
  - concrete data source management

- `UC_API.Domain`
  - core domain models such as Product
  - domain entity definitions

## Key technologies

- ASP.NET Core 10
- OpenIddict Validation for bearer token introspection
- ASP.NET Core Authorization policies
- RESTful API design with Controllers
- In-memory data storage (example implementation)

## Authentication and authorization

The API uses OpenIddict's validation middleware to:

1. Extract the bearer token from the `Authorization` header
2. Call the configured OpenIddict issuer (UC_Auth) at the introspection endpoint
3. Validate the token signature, issuer, audience, and scope
4. Extract claims from the introspection response

Authorization is enforced via an `ApiAccess` policy that requires:

- An authenticated user (via validated bearer token)
- The presence of the `api` scope in the token

All endpoints are protected by this policy.

## Key configuration

The API expects configuration in `UC_API.WebApi/appsettings.json`:

```json
{
  "Authentication": {
    "IntrospectionSecret": "..."
  }
}
```

The introspection secret must match the `resource-api` client secret configured in UC_Auth (`UC_Auth.Infrastructure/OpenIddict/OpenIddictSeeder.cs`).

The issuer and audience are configured in `Program.cs`:

```csharp
options.SetIssuer("https://127.0.0.1:7214/");
options.AddAudiences("resource-api");
options.SetClientId("resource-api")
       .SetClientSecret(builder.Configuration["Authentication:IntrospectionSecret"]!);
```

## API endpoints

### GET /api/products

Returns a list of all available products.

**Authorization:** Requires a valid bearer token with the `api` scope.

**Response:**

```json
[
  {
    "id": "00000000-0000-0000-0000-000000000000",
    "name": "Laptop",
    "price": 999.00
  },
  {
    "id": "00000000-0000-0000-0000-000000000001",
    "name": "Mouse",
    "price": 25.00
  }
]
```

## Running the app

Prerequisites:

- .NET 10 SDK
- UC_Auth running and accessible at `https://127.0.0.1:7214/`
- Matching introspection secret in both UC_Auth and UC_API configuration

From the repository root:

```bash
dotnet restore
dotnet run --project UC_API.WebApi
```

The API will start on the configured port (typically `https://localhost:5001`).

## Making authenticated requests

To call the API, first obtain a bearer token from UC_Auth using the OAuth 2.0 Authorization Code with PKCE flow:

1. Redirect to `https://127.0.0.1:7214/connect/authorize` with your client ID
2. After login, retrieve an authorization code
3. Exchange the code for an access token at `https://127.0.0.1:7214/connect/token`
4. Use the access token in the `Authorization: Bearer <token>` header

Example using curl:

```bash
curl -H "Authorization: Bearer <access_token>" \
  https://localhost:5001/api/products
```

## Architecture decisions

This project follows Clean Architecture principles:

- **Domain layer**: Contains only entities and value objects, with no external dependencies.
- **Application layer**: Contains use cases and service contracts; depends only on the domain layer.
- **Infrastructure layer**: Implements the application contracts and handles persistence.
- **Presentation layer**: Exposes API endpoints and orchestrates request/response handling.

The repository pattern is used to abstract data access, making it simple to swap the in-memory implementation with a database-backed implementation.

## Token introspection flow

On each API request:

1. The OpenIddict Validation middleware extracts the bearer token from the request header.
2. It calls the introspection endpoint on the configured issuer (UC_Auth).
3. UC_Auth returns token details (subject, scope, audience, expiration, etc.).
4. The middleware validates issuer, audience, and scope requirements.
5. If valid, the token claims are added to the HTTP context user principal.
6. The authorization policy checks if the required `api` scope is present.
7. If all checks pass, the endpoint handler is executed.

This design ensures that only valid, correctly-scoped tokens can access the API.

## Local development notes

- The API expects UC_Auth to be running on `https://127.0.0.1:7214/`.
- Token validation happens on every request, so latency depends on introspection endpoint response time.
- In-memory product data is hardcoded for simplicity; replace with a database implementation as needed.
- The `AllowAll` CORS policy is configured for local development and should be restricted in production.

## Security considerations

Before production deployment, review and harden:

- Issuer URL validation and certificate pinning
- Audience configuration and claim verification
- Token lifetime expectations
- Introspection secret management
- CORS policies and allowed origins
- Rate limiting on API endpoints
- Logging of security-relevant events

## Ecosystem integration

This API is part of a two-tier authentication ecosystem:

- **UC_Auth** (https://github.com/Max-Allan-Smith/UC_Auth): Central authorization server that issues tokens
- **UC_API** (this repository): Resource server that consumes and validates tokens

The UC_Auth server seeds the `resource-api` client with introspection permissions, and this API uses those credentials to validate tokens issued by UC_Auth. Together, they form a complete OAuth 2.0 / OpenID Connect ecosystem.

## Repository status

This is a working resource server implementation and foundation for a secured REST API with centralized token-based access control in .NET.
