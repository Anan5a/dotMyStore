using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepository
{
    public interface IRoleRepository:IRepository<Role>
    {
        new int Add(Role Role);
        new IEnumerable<Role> GetAll(Dictionary<string, dynamic>? condition, string? includeProperties = null);
        new Role? Get(Dictionary<string, dynamic> condition, string? includeProperties);
        new Role? Update(Role Role);
        int Remove(int id);
        int RemoveRange(List<int> Ids);
    }
}
