using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace EmployeeManagement.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly DbHelper _db;


        public UserRepository(IConfiguration config, DbHelper db)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found in configuration.");
            _db = db;
        }
        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }


        public async Task<(UserModel? user, List<string> roles, string message)> LoginAsync(string username, string password)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            await using var cmd = new SqlCommand("sp_LoginUser", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return (null, new(), "Username does not exist");

            var storedHash = reader["PasswordHash"]?.ToString();

            if (string.IsNullOrEmpty(storedHash))
                return (null, new(), "Password not set");

            // ✅ MUST MATCH EnhancedHashPassword
            bool isValid =
                BCrypt.Net.BCrypt.EnhancedVerify(password, storedHash);

            if (!isValid)
                return (null, new(), "Invalid password");

            var user = new UserModel
            {
                Id = Convert.ToInt32(reader["Id"]),
                Username = reader["Username"].ToString()!
            };

            return (user, new(), "Login successful");
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
                    BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password);

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

        public (int UserId, string Username) Login(string username, string passwordHash)
        {
            using var con = _db.GetConnection();
            using var cmd = new SqlCommand("sp_LoginUser", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return (0, null);

            return (reader.GetInt32(0), reader.GetString(1));
        }

        public List<string> GetRoles(int userId)
        {
            var roles = new List<string>();

            using var con = _db.GetConnection();
            using var cmd = new SqlCommand("sp_GetUserRoles", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                roles.Add(reader.GetString(0));
            }

            return roles;
        }
    }
}