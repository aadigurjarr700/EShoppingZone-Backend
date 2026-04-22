using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductService.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string ProductType { get; set; } = string.Empty;

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public Dictionary<int, double> Rating { get; set; } = new Dictionary<int, double>();

        public Dictionary<int, string> Review { get; set; } = new Dictionary<int, string>();

        public IList<string> Image { get; set; } = new List<string>();

        [Required]
        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;

        public Dictionary<string, string> Specification { get; set; } = new Dictionary<string, string>();
    }
}
