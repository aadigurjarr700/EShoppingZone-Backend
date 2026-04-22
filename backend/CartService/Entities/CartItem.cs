using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CartService.Entities
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int CartId { get; set; }

        [JsonIgnore] // Prevent circular reference during JSON serialization
        public Cart? Cart { get; set; }
    }
}
