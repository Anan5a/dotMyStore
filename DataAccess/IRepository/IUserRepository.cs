using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        new int Add(User user);
        new IEnumerable<User> GetAll(string? includeProperties = null);
        new User? Get(Dictionary<string, dynamic> condition, string? includeProperties);
        new User? Update(User user);
        int Remove(int id);
        int RemoveRange(List<int> Ids);
    }
}
