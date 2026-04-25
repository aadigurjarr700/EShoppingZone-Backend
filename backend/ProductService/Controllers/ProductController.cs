using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs;
using ProductService.Interfaces;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Roles = "MERCHANT,ADMIN")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductDto productDto)
        {
            await _productService.AddProducts(productDto);
            return Ok("Product added successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);
            return Ok(product);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            var products = await _productService.GetProductByName(name);
            return Ok(products);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetProductByCategory(string category)
        {
            var products = await _productService.GetProductsByCategory(category);
            return Ok(products);
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetProductByType(string type)
        {
            var products = await _productService.GetProductsByType(type);
            return Ok(products);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "MERCHANT,ADMIN")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] AddProductDto productDto)
        {
            await _productService.UpdateProducts(id, productDto);
            return Ok("Product updated successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "MERCHANT,ADMIN")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null) return NotFound("Product not found.");

            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "MERCHANT" && int.TryParse(userIdStr, out int userId))
            {
                if (product.MerchantId != userId)
                {
                    return Forbid("You do not have permission to delete this product.");
                }
            }

            await _productService.DeleteProductById(id);
            return Ok("Product deleted successfully.");
        }
    }
}
