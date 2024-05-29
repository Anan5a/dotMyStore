using DataAccess.IRepository;
using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly string tableName = "Products";

        public ProductRepository(IMyDbConnection dbConnection) : base(dbConnection)
        {
        }

        public int Remove(int Id)
        {
            return base.Remove(tableName, "Id", Id);
        }
        public int RemoveRange(List<int> Ids)
        {
            return base.RemoveRange(tableName, "Id", Ids);
        }
        public new int Add(Product product)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
                {
                    connection.Open();
                    string sql = $"INSERT INTO [Products] ([Title], [Description], [SKU], [Price], [Weight]) VALUES (@Title, @Description, @SKU, @Price, @Weight); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Title", product.Title);
                        command.Parameters.AddWithValue("@Description", product.Description);
                        command.Parameters.AddWithValue("@SKU", product.SKU);
                        command.Parameters.AddWithValue("@Price", product.Price);
                        command.Parameters.AddWithValue("@Weight", product.Weight);
                        command.Parameters.AddWithValue("@Id", product.Id);
                        int insertedProductId = Convert.ToInt32(command.ExecuteScalar());
                        return insertedProductId;
                    }
                }

            }
            catch (Exception ex)
            {
                return 0;
            }



        }

        public new IEnumerable<Product> GetAll(string? includeProperties = null)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
            {
                connection.Open();

                string query = $"SELECT [Id], [Title], [Description], [SKU], [Price], [Weight] FROM [Products]";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            Product Product = new Product
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                Title = Convert.ToString(row["Title"]),
                                Description = Convert.ToString(row["Description"]),
                                SKU = Convert.ToString(row["SKU"]),
                                Price = Convert.ToDecimal(row["Price"]),
                                Weight = Convert.ToDecimal(row["Weight"]),
                            };

                            products.Add(Product);
                        }
                    }
                }
            }
            return products;
        }

        public new Product? Get(Dictionary<string, dynamic> condition, string? includeProperties)
        {
            Product product = null;
            using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
            {
                connection.Open();

                // Build the WHERE clause based on the conditions in the dictionary
                StringBuilder whereClause = new StringBuilder();
                List<SqlParameter> parameters = new List<SqlParameter>();
                foreach (var pair in condition)
                {
                    if (whereClause.Length > 0)
                        whereClause.Append(" AND ");

                    whereClause.Append($"[{pair.Key}] = @{pair.Key}");
                    parameters.Add(new SqlParameter($"@{pair.Key}", pair.Value));
                }

                string query = $"SELECT [Id], [Title], [Description], [SKU], [Price], [Weight] FROM [Products] WHERE {whereClause}";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Product
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                Title = Convert.ToString(reader["Title"]),
                                Description = Convert.ToString(reader["Description"]),
                                SKU = Convert.ToString(reader["SKU"]),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Weight = Convert.ToDecimal(reader["Weight"]),
                            };
                        }
                    }
                }
            }
            return product;
        }

        public new Product? Update(Product existingProduct)
        {
            using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
            {
                connection.Open();

                // Execute the SQL update command to update the product record in the database
                string query = $"UPDATE [Products] SET [Title] = @Title, [Description] = @Description, [SKU] = @SKU, [Price] = @Price, [Weight] = @Weight, [ModifiedAt] = @ModifiedAt WHERE [Id] = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", existingProduct.Title);
                    command.Parameters.AddWithValue("@Description", existingProduct.Description);
                    command.Parameters.AddWithValue("@SKU", existingProduct.SKU);
                    command.Parameters.AddWithValue("@Price", existingProduct.Price);
                    command.Parameters.AddWithValue("@Weight", existingProduct.Weight);
                    command.Parameters.AddWithValue("@Id", existingProduct.Id);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        // If no rows were affected, the update operation failed
                        return null;
                    }
                }
            }

            return existingProduct;
        }
    }
}
