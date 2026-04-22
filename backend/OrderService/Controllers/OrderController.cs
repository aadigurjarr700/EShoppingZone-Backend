using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OrderService.DTOs;
using OrderService.Entities;
using OrderService.Interfaces;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize] // All endpoints require authentication
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            throw new System.UnauthorizedAccessException("Invalid Token: Missing UserId.");
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("customer")]
        public async Task<IActionResult> GetOrderByCustomerId()
        {
            var customerId = GetCurrentUserId();
            var orders = await _orderService.GetOrderByCustomerId(customerId);
            return Ok(orders);
        }

        [HttpPost("placeOrder")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> PlaceOrder([FromBody] CheckoutRequest request)
        {
            var customerId = GetCurrentUserId();
            request.Address.CustomerId = customerId; // Ensure correct mapping
            
            await _orderService.PlaceOrder(request.Cart, customerId, request.Address);
            return Ok("Order placed successfully via Cash On Delivery.");
        }

        [HttpPost("onlinePayment")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> OnlinePayment([FromBody] CheckoutRequest request)
        {
            var customerId = GetCurrentUserId();
            request.Address.CustomerId = customerId;

            await _orderService.OnlinePayment(request.Cart, customerId, request.Address);
            return Ok("Order placed successfully via E-Wallet Online Payment.");
        }

        [HttpPost("storeAddress")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> StoreAddress([FromQuery] int orderId, [FromBody] Address address)
        {
            var customerId = GetCurrentUserId();
            address.CustomerId = customerId;
            
            await _orderService.StoreAddress(address, orderId);
            return Ok("Address stored successfully for the order.");
        }

        [HttpPut("changeStatus")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ChangeStatus([FromQuery] int orderId, [FromQuery] string status)
        {
            await _orderService.ChangeStatus(status, orderId);
            return Ok($"Order status updated to {status}.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var customerId = GetCurrentUserId();
            var order = await _orderService.GetOrderById(id);
            
            // Allow if ADMIN or if the order belongs to the customer
            if (User.IsInRole("ADMIN") || order.CustomerId == customerId)
            {
                await _orderService.DeleteOrder(id);
                return Ok("Order deleted successfully.");
            }
            
            return Forbid("You do not have permission to delete this order.");
        }

        [HttpGet("allAddress")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllAddress()
        {
            var addresses = await _orderService.GetAllAddress();
            return Ok(addresses);
        }
    }

    public class CheckoutRequest
    {
        public CartDto Cart { get; set; } = new CartDto();
        public Address Address { get; set; } = new Address();
    }
}
