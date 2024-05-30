
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace Models
{
    public class Product
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [ValidateNever]
        public IList<string>? ImageUrls { get; set; }

        [ValidateNever]
        [JsonIgnore]
        public List<ProductImage> Images { get; set; }

    }
}
