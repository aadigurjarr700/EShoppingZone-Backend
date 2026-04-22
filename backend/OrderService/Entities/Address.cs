using System.ComponentModel.DataAnnotations;

namespace OrderService.Entities
{
    public class Address
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        public int FlatNumber { get; set; }

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public int Pincode { get; set; }

        [Required]
        public string State { get; set; } = string.Empty;
    }
}
