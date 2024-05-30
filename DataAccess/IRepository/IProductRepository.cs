using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepository
{
    public interface IProductRepository:IRepository<Product>
    {
        new int Add(Product Product);
        new IEnumerable<Product>? GetAll(Dictionary<string, dynamic>? condition = null, string? includeProperties = null);
        new Product? Get(Dictionary<string, dynamic> condition, string? includeProperties);
        new Product? Update(Product Product);
        int Remove(int id);
        int RemoveRange(List<int> Ids);
    }
}
