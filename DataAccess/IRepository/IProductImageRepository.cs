using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        new int Add(ProductImage ProductImage);
        new IEnumerable<ProductImage> GetAll(Dictionary<string, dynamic>? condition, string? includeProperties = null);
        new ProductImage? Get(Dictionary<string, dynamic> condition, string? includeProperties);
        new ProductImage? Update(ProductImage ProductImage);
        int Remove(int id);
        int RemoveRange(List<int> Ids);
    }
}
