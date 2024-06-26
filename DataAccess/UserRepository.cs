﻿using DataAccess.IRepository;
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
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly string tableName = "Users";

        public UserRepository(IMyDbConnection dbConnection) : base(dbConnection)
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
        public new int Add(User user)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
                {
                    connection.Open();
                    string sql = $"INSERT INTO [{tableName}] ([Name], [Email], [Password], [Role], [CreatedAt], [ModifiedAt]) VALUES (@Name, @Email, @Password, @Role, @CreatedAt, @ModifiedAt); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Name", user.Name);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Password", user.Password);
                        command.Parameters.AddWithValue("@Role", user.RoleId);
                        command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                        command.Parameters.AddWithValue("@ModifiedAt", user.ModifiedAt);
                        int insertedUserId = Convert.ToInt32(command.ExecuteScalar());
                        return insertedUserId;
                    }
                }

            }
            catch (Exception ex)
            {
                return 0;
            }



        }

        public new IEnumerable<User> GetAll(Dictionary<string, dynamic>? condition = null, string? includeProperties = "true")
        {
            List<User> users = new List<User>();
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

                        whereClause.Append($"u.[{pair.Key}] = @{pair.Key}");
                        parameters.Add(new SqlParameter($"@{pair.Key}", pair.Value));
                    }

                }


                string query;
                if (whereClause.Length > 0)
                {
                    query = $"SELECT u.[Id], u.[Name], u.[Email], u.[Password], r.[RoleName], u.[Role], u.[CreatedAt], u.[ModifiedAt] FROM [{tableName}] u JOIN [Roles] r ON u.[Role] = r.[Id] WHERE {whereClause}";
                }
                else
                {
                    query = $"SELECT u.[Id], u.[Name], u.[Email], u.[Password], r.[RoleName], u.[Role], u.[CreatedAt], u.[ModifiedAt] FROM [{tableName}] u JOIN [Roles] r ON u.[Role] = r.[Id]";
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
                            User user = new User
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                Name = Convert.ToString(row["Name"]),
                                Email = Convert.ToString(row["Email"]),
                                Password = Convert.ToString(row["Password"]),
                                RoleId = Convert.ToInt64(row["Role"]),
                                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                                ModifiedAt = Convert.ToDateTime(row["ModifiedAt"])
                            };
                            if (includeProperties != null)
                            {
                                user.Role = new Role
                                {
                                    Id = user.RoleId,
                                    RoleName = Convert.ToString(row["RoleName"])
                                };
                            }
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }

        public new User? Get(Dictionary<string, dynamic> condition, string? includeProperties)
        {
            User? user = null;
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

                    whereClause.Append($"u.[{pair.Key}] = @{pair.Key}");
                    parameters.Add(new SqlParameter($"@{pair.Key}", pair.Value));
                }
                string query;

                if (includeProperties != null)
                {
                    query = $"SELECT u.[Id], u.[Name], u.[Email], u.[Password], r.[RoleName],u.[Role] , u.[CreatedAt], u.[ModifiedAt] FROM[{tableName}] u JOIN [Roles] r ON u.[Role] = r.[Id] WHERE {whereClause}";
                }
                else
                {
                    query = $"SELECT u.[Id], u.[Name], u.[Email], u.[Password], u.[Role], u.[CreatedAt], u.[ModifiedAt] FROM [{tableName}] u WHERE {whereClause}";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                Name = Convert.ToString(reader["Name"]),
                                Email = Convert.ToString(reader["Email"]),
                                Password = Convert.ToString(reader["Password"]),
                                RoleId = Convert.ToInt64(reader["Role"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                ModifiedAt = Convert.ToDateTime(reader["ModifiedAt"])
                            };
                            if (includeProperties != null)
                            {
                                user.Role = new Role
                                {
                                    Id = user.RoleId,
                                    RoleName = Convert.ToString(reader["RoleName"])
                                };
                            }
                        }
                    }
                }
            }
            return user;
        }

        public new User? Update(User existingUser)
        {
            using (SqlConnection connection = new SqlConnection(dbConnection.ConnectionString))
            {
                connection.Open();

                // Execute the SQL update command to update the user record in the database
                string query = $"UPDATE [{tableName}] SET [Name] = @Name, [Email] = @Email, [Password] = @Password, [Role] = @Role, [ModifiedAt] = @ModifiedAt WHERE [Id] = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", existingUser.Name);
                    command.Parameters.AddWithValue("@Email", existingUser.Email);
                    command.Parameters.AddWithValue("@Password", existingUser.Password);
                    command.Parameters.AddWithValue("@Role", existingUser.RoleId);
                    command.Parameters.AddWithValue("@ModifiedAt", existingUser.ModifiedAt);
                    command.Parameters.AddWithValue("@Id", existingUser.Id);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        // If no rows were affected, the update operation failed
                        return null;
                    }
                }
            }

            return existingUser;
        }
    }
}
