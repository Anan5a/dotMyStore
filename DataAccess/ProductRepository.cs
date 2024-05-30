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
                    string sql = $"INSERT INTO [{tableName}] ([Title], [Description], [SKU], [Price], [Weight], [CreatedAt], [ModifiedAt]) VALUES (@Title, @Description, @SKU, @Price, @Weight, @CreatedAt, @ModifiedAt); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Title", product.Title);
                        command.Parameters.AddWithValue("@Description", product.Description);
                        command.Parameters.AddWithValue("@SKU", product.SKU);
                        command.Parameters.AddWithValue("@Price", product.Price);
                        command.Parameters.AddWithValue("@Weight", product.Weight);
                        command.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);
                        command.Parameters.AddWithValue("@ModifiedAt", product.ModifiedAt);
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

        public new IEnumerable<Product> GetAll(Dictionary<string, dynamic>? condition = null, string? includeProperties = null)
        {
            Dictionary<long, Product> products = new Dictionary<long, Product>();

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

                        whereClause.Append($"p.{pair.Key} = @{pair.Key}");
                        parameters.Add(new SqlParameter($"@{pair.Key}", pair.Value));
                    }

                }

                string query;

                if (includeProperties != null && whereClause.Length > 0)
                {
                    query = $"SELECT  p.Id, p.Title, p.Description, p.SKU, p.Price, p.Weight, p.CreatedAt, p.ModifiedAt, pi.ImageUrl AS ImageUrl FROM {tableName} p LEFT JOIN ProductImages pi ON p.Id = pi.ProductId WHERE {whereClause};";
                }
                else if (includeProperties == null && whereClause.Length > 0)
                {
                    query = $"SELECT p.Id, p.Title, p.Description, p.SKU, p.Price, p.Weight, p.CreatedAt, p.ModifiedAt FROM [{tableName}]  WHERE {whereClause};";
                }
                else if (includeProperties != null && whereClause.Length <= 0)
                {
                    query = $"SELECT  p.Id, p.Title, p.Description, p.SKU, p.Price, p.Weight, p.CreatedAt, p.ModifiedAt, pi.ImageUrl AS ImageUrl FROM {tableName} p LEFT JOIN ProductImages pi ON p.Id = pi.ProductId;";
                }
                else
                {
                    query = $"SELECT [Id], [Title], [Description], [SKU], [Price], [Weight], [CreatedAt], [ModifiedAt] FROM [{tableName}]";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        Dictionary<long, List<string>> productImagesDict = new Dictionary<long, List<string>>();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var productId = Convert.ToInt64(row["Id"]);
                            if (!products.ContainsKey(productId))
                            {
                                Product product = new Product
                                {
                                    Id = Convert.ToInt64(row["Id"]),
                                    Title = Convert.ToString(row["Title"]),
                                    Description = Convert.ToString(row["Description"]),
                                    SKU = Convert.ToString(row["SKU"]),
                                    Price = Convert.ToDecimal(row["Price"]),
                                    Weight = Convert.ToDecimal(row["Weight"]),
                                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                                    ModifiedAt = Convert.ToDateTime(row["ModifiedAt"])
                                };

                                products.Add(productId, product);
                            }
                            //get the images if any
                            if (includeProperties != null)
                            {

                                if (!productImagesDict.ContainsKey(productId))
                                {
                                    productImagesDict[productId] = new List<string>() { };
                                }
                                string imageUrl = row["ImageUrl"] != DBNull.Value ? Convert.ToString(row["ImageUrl"]) : null;

                                if (!string.IsNullOrEmpty(imageUrl))
                                {
                                    productImagesDict[productId].Add(imageUrl.Replace("\\", "/")); //backslash (\) is a joke
                                }

                            }


                        }

                        foreach (var productImages in productImagesDict)
                        {
                            products[productImages.Key].ImageUrls = productImages.Value.ToArray();
                        }
                    }
                }
            }
            return products.Values;
        }

        public new Product? Get(Dictionary<string, dynamic> condition, string? includeProperties = null)
        {
            Dictionary<long, Product> products = new Dictionary<long, Product>();
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

                    whereClause.Append($"t.{pair.Key} = @{pair.Key}");
                    parameters.Add(new SqlParameter($"@{pair.Key}", pair.Value));
                }

                string query;
                if (includeProperties != null)
                {
                    query = $"SELECT  t.Id, t.Title, t.Description,t.SKU, t.Price, t.Weight, t.CreatedAt, t.ModifiedAt, pi.ImageUrl AS ImageUrl FROM {tableName} t LEFT JOIN ProductImages pi ON t.Id = pi.ProductId  WHERE {whereClause} ;";
                }
                else
                {
                    query = $"SELECT t.Id, t.Title, t.Description, t.SKU, t.Price, t.Weight, t.CreatedAt, t.ModifiedAt FROM {tableName} t WHERE {whereClause}";
                }




                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());




                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        Dictionary<long, List<string>> productImagesDict = new Dictionary<long, List<string>>();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var productId = Convert.ToInt64(row["Id"]);
                            if (!products.ContainsKey(productId))
                            {
                                Product product = new Product
                                {
                                    Id = Convert.ToInt64(row["Id"]),
                                    Title = Convert.ToString(row["Title"]),
                                    Description = Convert.ToString(row["Description"]),
                                    SKU = Convert.ToString(row["SKU"]),
                                    Price = Convert.ToDecimal(row["Price"]),
                                    Weight = Convert.ToDecimal(row["Weight"]),
                                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                                    ModifiedAt = Convert.ToDateTime(row["ModifiedAt"])
                                };

                                products.Add(productId, product);
                            }
                            //get the images if any
                            if (includeProperties != null)
                            {

                                if (!productImagesDict.ContainsKey(productId))
                                {
                                    productImagesDict[productId] = new List<string>() { };
                                }
                                string imageUrl = row["ImageUrl"] != DBNull.Value ? Convert.ToString(row["ImageUrl"]) : null;

                                if (!string.IsNullOrEmpty(imageUrl))
                                {
                                    productImagesDict[productId].Add(imageUrl.Replace("\\", "/")); //backslash (\) is a joke
                                }

                            }


                        }

                        foreach (var productImages in productImagesDict)
                        {
                            products[productImages.Key].ImageUrls = productImages.Value.ToArray();
                        }
                    }

                    //this will not work in case of relationship with images

                    //using (SqlDataReader reader = command.ExecuteReader())
                    //{
                    //    if (reader.Read())
                    //    {
                    //        product = new Product
                    //        {
                    //            Id = Convert.ToInt64(reader["Id"]),
                    //            Title = Convert.ToString(reader["Title"]),
                    //            Description = Convert.ToString(reader["Description"]),
                    //            SKU = Convert.ToString(reader["SKU"]),
                    //            Price = Convert.ToDecimal(reader["Price"]),
                    //            Weight = Convert.ToDecimal(reader["Weight"]),
                    //            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    //            ModifiedAt = Convert.ToDateTime(reader["ModifiedAt"])
                    //        };
                    //    }
                    //}
                }
            }
            //we are sure there is only one product here.
            return products.Values.FirstOrDefault();
        }

        public new Product? Update(Product existingProduct)
        {
            using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
            {
                connection.Open();

                // Execute the SQL update command to update the product record in the database
                string query = $"UPDATE [{tableName}] SET [Title] = @Title, [Description] = @Description, [SKU] = @SKU, [Price] = @Price, [Weight] = @Weight, [ModifiedAt] = @ModifiedAt WHERE [Id] = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", existingProduct.Title);
                    command.Parameters.AddWithValue("@Description", existingProduct.Description);
                    command.Parameters.AddWithValue("@SKU", existingProduct.SKU);
                    command.Parameters.AddWithValue("@Price", existingProduct.Price);
                    command.Parameters.AddWithValue("@Weight", existingProduct.Weight);
                    command.Parameters.AddWithValue("@ModifiedAt", existingProduct.ModifiedAt);
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
