using System.ComponentModel.DataAnnotations;

namespace OrderService.Entities
{
    public class ProductSnapshot
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;
    }
}
