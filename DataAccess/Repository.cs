using DataAccess.IRepository;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public IMyDbConnection dbConnection { get; set; }

        public Repository(IMyDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public int Remove(string TableName, string ColumnName, int Id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
                {
                    connection.Open();
                    string sql = $"DELETE FROM {TableName} WHERE {ColumnName} = @Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public int RemoveRange(string TableName, string ColumnName, List<int> Ids)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
                {
                    connection.Open();
                    string sql = $"DELETE FROM {TableName} WHERE {ColumnName} IN ({string.Join(",", Ids)})";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int Add(T entity)
        {
            throw new NotImplementedException("Must implement specific logic in the repository");
        }

        public IEnumerable<T> GetAll(Dictionary<string, dynamic>? condition = null, string ? includeProperties = null)
        {
            throw new NotImplementedException("Must implement specific logic in the repository");
        }

        public T? Get(Dictionary<string, dynamic> condition, string? includeProperties)
        {
            throw new NotImplementedException("Must implement specific logic in the repository");
        }

        public T Update(T entity)
        {
            throw new NotImplementedException("Must implement specific logic in the repository");
        }
    }
}
