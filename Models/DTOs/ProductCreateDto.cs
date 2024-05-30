using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "SKU must contain only numeric characters")]
        [StringLength(10)]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Weight must be a positive value")]
        public decimal Weight { get; set; }
        [ValidateNever]
        public IList<IFormFile>? Images { get; set; }
    }
}
