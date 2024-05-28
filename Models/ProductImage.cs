using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Models
{
    public class ProductImage
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public long ProductId { get; set; }
        public Product? Product { get; set; }

        
    }
}
