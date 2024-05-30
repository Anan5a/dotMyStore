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
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly string tableName = "ProductImages";

        public ProductImageRepository(IMyDbConnection dbConnection) : base(dbConnection)
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
        public new int Add(ProductImage productImage)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
                {
                    connection.Open();
                    string sql = $"INSERT INTO [{tableName}] ([ImageUrl], [ProductId]) VALUES (@ImageUrl, @ProductId); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", productImage.ProductId);
                        command.Parameters.AddWithValue("@ImageUrl", productImage.ImageUrl);
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

        public new IEnumerable<ProductImage>? GetAll(Dictionary<string, dynamic>? condition = null, string? includeProperties = null)
        {
            List<ProductImage> productImages = new List<ProductImage>();
            using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
            {
                connection.Open();



                StringBuilder whereClause = new StringBuilder();
                List<SqlParameter> parameters = new List<SqlParameter>();
                if (condition != null)
                {
                    foreach (var pair in condition)
                    {
                        if (whereClause.Length > 0)
                            whereClause.Append(" AND ");

                        whereClause.Append($"[{pair.Key}] = @{pair.Key}");
                        parameters.Add(new SqlParameter($"@{pair.Key}", pair.Value));
                    }

                }


                string query;
                if (whereClause.Length > 0)
                {
                    query = $"SELECT [Id], [ProductId], [ImageUrl] FROM [{tableName}] WHERE {whereClause};";
                }
                else
                {
                    query = $"SELECT [Id], [ProductId], [ImageUrl] FROM [{tableName}]";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            ProductImage productImage = new ProductImage
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                ProductId = Convert.ToInt64(row["ProductId"]),
                                ImageUrl = Convert.ToString(row["ImageUrl"]),

                            };

                            productImages.Add(productImage);
                        }
                    }
                }
            }
            return productImages.AsEnumerable();
        }

        public new ProductImage? Get(Dictionary<string, dynamic> condition, string? includeProperties)
        {
            ProductImage productImage = null;
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

                string query = $"SELECT [Id], [ProductId], [ImageUrl] FROM [{tableName}] WHERE {whereClause}";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productImage = new ProductImage
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                ImageUrl = Convert.ToString(reader["ImageUrl"]),
                                ProductId = Convert.ToInt64(reader["ProductId"]),

                            };
                        }
                    }
                }
            }
            return productImage;
        }

        public new ProductImage? Update(ProductImage existingProductImage)
        {
            using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
            {
                connection.Open();

                // Execute the SQL update command to update the productImage record in the database
                string query = $"UPDATE [{tableName}] SET [ImageUrl] = @ImageUrl, [ProductId] = @ProductId WHERE [Id] = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ImageUrl", existingProductImage.ImageUrl);
                    command.Parameters.AddWithValue("@ProductId", existingProductImage.ProductId);
                    command.Parameters.AddWithValue("@Id", existingProductImage.Id);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        // If no rows were affected, the update operation failed
                        return null;
                    }
                }
            }

            return existingProductImage;
        }

        
    }
}
