using System.Collections.Generic;

namespace OrderService.DTOs
{
    public class CartDto
    {
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public IList<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
