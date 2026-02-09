using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace EmployeeManagement.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found in configuration.");
        }
        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

          
        public async Task<(UserModel? user, string message)> LoginAsync(string username, string password)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            await using var cmd = new SqlCommand("sp_LoginUser", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Username", username);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                string storedHash = reader["PasswordHash"].ToString()!;

                if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                {
                    return (new UserModel
                    {
                        Id = (int)reader["Id"],
                        Username = reader["Username"].ToString()!
                    }, "Login successful");
                }
                else
                {
                    return (null, "Please enter valid password");
                }
            }

            return (null, "Username does not exist");
        }
        public async Task<int> RegisterAsync(RegisterRequest model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            try
            {
                await using var con = new SqlConnection(_connectionString);
                await con.OpenAsync();

                await using var cmd = new SqlCommand("sp_RegisterUser", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = model.Username;
                cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value =
                    BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Execute and get return value
                var returnParam = new SqlParameter("@ReturnVal", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                cmd.Parameters.Add(returnParam);

                await cmd.ExecuteNonQueryAsync();

                int result = (int)returnParam.Value;
                return result; // -1 = duplicate, 1 = success
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return 0;
            }
        }
    }
}