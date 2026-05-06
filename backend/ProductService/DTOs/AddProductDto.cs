using System.Collections.Generic;

namespace ProductService.DTOs
{
    public class AddProductDto
    {
        public string ProductType { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public IList<string> Image { get; set; } = new List<string>();
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, string> Specification { get; set; } = new Dictionary<string, string>();
        public Dictionary<int, double> Rating { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, string> Review { get; set; } = new Dictionary<int, string>();
        public int? MerchantId { get; set; }
    }
}
