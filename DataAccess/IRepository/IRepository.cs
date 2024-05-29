using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepository
{
    public interface IRepository<T> where T : class
    {
        IMyDbConnection dbConnection { get; set; }
        int Add(T entity);
        IEnumerable<T> GetAll(string? includeProperties = null);
        T? Get(Dictionary<string, dynamic> condition, string? includeProperties = null);
        T? Update(T entity);
        int Remove(string TableName, string ColumnName, int Id);
        int RemoveRange(string TableName, string ColumnName, List<int> Ids);
    }
}
