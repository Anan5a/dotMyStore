using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{

    public class ProductUpdateDto
    {
        [StringLength(150)]
        public string? Title { get; set; }
        [StringLength(1500)]
        public string? Description { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "SKU must contain only alphanumeric characters")]
        public string? SKU { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal? Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Weight must be a positive value")]
        public decimal? Weight { get; set; }

    }
}
