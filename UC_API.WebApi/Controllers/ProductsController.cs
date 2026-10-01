using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UC_API.Application.Products;
using UC_API.Domain.Products;

namespace UC_API.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApiAccess")]
    public class ProductsController(GetProducts getProducts) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<Product> products = await getProducts.ExecuteAsync(cancellationToken);

            return Ok(products);
        }
    }
}