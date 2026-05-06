using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Entities
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int CustomerId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        [Required]
        public string ModeOfPayment { get; set; } = string.Empty;

        [Required]
        public string OrderStatus { get; set; } = "Placed";

        [Required]
        public int Quantity { get; set; }

        // Owned Entities
        public Address? Address { get; set; }
        public ProductSnapshot? Product { get; set; }
    }
}
