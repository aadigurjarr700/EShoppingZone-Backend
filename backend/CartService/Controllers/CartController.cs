using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CartService.Interfaces;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/carts")]
    [Authorize] // Requires authentication for all endpoints
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            throw new System.UnauthorizedAccessException("Invalid Token: Missing UserId.");
        }

        [HttpPost]
        public async Task<IActionResult> AddCart([FromQuery] int productId, [FromQuery] int quantity)
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.UpdateCart(userId, productId, quantity);
            return Ok(cart);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            var userId = GetCurrentUserId();
            
            // Allow ADMIN to view any cart, but CUSTOMER can only view their own
            if (User.IsInRole("ADMIN") || userId == id)
            {
                var cart = await _cartService.GetCartById(id);
                return Ok(cart);
            }
            
            return Forbid("You do not have permission to view this cart.");
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")] // Only ADMIN can view all carts
        public async Task<IActionResult> GetAllCarts()
        {
            var carts = await _cartService.GetAllCarts();
            return Ok(carts);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCart([FromQuery] int productId, [FromQuery] int quantity)
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.UpdateCart(userId, productId, quantity);
            return Ok(cart);
        }
    }
}
