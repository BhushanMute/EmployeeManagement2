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


        //public async Task<(UserModel? user, List<string> roles, string message)> LoginAsync(string username, string password)
        //{
        //    await using var con = new SqlConnection(_connectionString);
        //    await con.OpenAsync();

        //    await using var cmd = new SqlCommand("sp_LoginUser", con)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };

        //    cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;

        //    await using var reader = await cmd.ExecuteReaderAsync();

        //    if (!await reader.ReadAsync())
        //        return (null, new(), "Username does not exist");

        //    var storedHash = reader["PasswordHash"]?.ToString();

        //    if (string.IsNullOrEmpty(storedHash))
        //        return (null, new(), "Password not set");

        //    // ✅ MUST MATCH EnhancedHashPassword
        //    bool isValid =
        //        BCrypt.Net.BCrypt.EnhancedVerify(password, storedHash);

        //    if (!isValid)
        //        return (null, new(), "Invalid password");

        //    var user = new UserModel
        //    {
        //        Id = Convert.ToInt32(reader["Id"]),
        //        Username = reader["Username"].ToString()!
        //    };

        //    return (user, new(), "Login successful");
        //}

        public async Task<(UserModel?, List<string>, string)> LoginAsync(string username, string password)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_LoginUser", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;

            await con.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return (null, new List<string>(), "User not found");

            var storedHash = reader["PasswordHash"]?.ToString();

            if (string.IsNullOrEmpty(storedHash))
                return (null, new List<string>(), "Password not set");

            // ✅ Verify password using BCrypt
            bool isValid = BCrypt.Net.BCrypt.EnhancedVerify(password, storedHash);

            if (!isValid)
                return (null, new List<string>(), "Invalid password");

            var user = new UserModel
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Username = reader["Username"].ToString(),
                Email = reader["Email"].ToString() ?? string.Empty
            };

            return (user, new List<string>(), "Login successful");
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

                // ✅ ADD THIS
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = model.Email;

                cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value =
                    BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password);

                // Optional: You can pass provider explicitly
                cmd.Parameters.Add("@Provider", SqlDbType.NVarChar, 50).Value = "Local";

                var returnParam = new SqlParameter("@ReturnVal", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                cmd.Parameters.Add(returnParam);

                await cmd.ExecuteNonQueryAsync();

                return (int)returnParam.Value;
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
        public async Task<UserModel?> GoogleLogin(string email, string googleId)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("sp_SocialLogin", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email;
                cmd.Parameters.Add("@Provider", SqlDbType.NVarChar, 50).Value = "Google";
                cmd.Parameters.Add("@SocialId", SqlDbType.NVarChar, 100)
                    .Value = string.IsNullOrEmpty(googleId) ? (object)DBNull.Value : googleId;

                await con.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new UserModel
                    {
                        Id = (int)reader["Id"],
                        Username = reader["Username"].ToString(),
                        Email = reader["Email"].ToString(),
                        Provider = reader["Provider"].ToString(),
                        GoogleId = reader["GoogleId"]?.ToString(),
                        FacebookId = reader["FacebookId"]?.ToString()
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("FULL ERROR: " + ex.ToString());
                throw; // IMPORTANT: shows real error
            }
        }
        //public async Task<UserModel?> SocialLogin(string email, string provider, string? socialId = null)
        //{
        //    using SqlConnection con = new SqlConnection(_connectionString);
        //    using SqlCommand cmd = new SqlCommand("sp_SocialLogin", con);

        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email;
        //    cmd.Parameters.Add("@Provider", SqlDbType.NVarChar, 50).Value = provider;
        //    cmd.Parameters.Add("@SocialId", SqlDbType.NVarChar, 100).Value = (object?)socialId ?? DBNull.Value;

        //    await con.OpenAsync();

        //    using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        //    if (await reader.ReadAsync())
        //    {
        //        return new UserModel
        //        {
        //            Id = Convert.ToInt32(reader["Id"]),
        //            Username = reader["Username"].ToString()
        //        };
        //    }

        //    return null;
        //}

        //public async Task<UserModel?> SocialLogin(string email, string provider, string? socialId)
        //{
        //    using SqlConnection con = new SqlConnection(_connectionString);

        //    using SqlCommand cmd = new SqlCommand("sp_SocialLogin", con);

        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email;

        //    // ✅ FIX: use provider parameter, don't hardcode wrong value
        //    cmd.Parameters.Add("@Provider", SqlDbType.NVarChar, 50).Value = provider;

        //    cmd.Parameters.Add("@SocialId", SqlDbType.NVarChar, 100).Value =
        //        string.IsNullOrEmpty(socialId) ? DBNull.Value : socialId;

        //    await con.OpenAsync();

        //    using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        //    if (await reader.ReadAsync())
        //    {
        //        return new UserModel
        //        {
        //            Id = Convert.ToInt32(reader["Id"]),
        //            Username = reader["Username"].ToString(),
        //            Email = reader["Email"].ToString(),
        //            Provider = reader["Provider"].ToString()
        //        };
        //    }

        //    return null;
        //}

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_GetUserByEmail", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                     
                };
            }

            return null;
        }
        public async Task<UserModel?> SocialLogin( string email, string provider, string socialId)
        {
            using SqlConnection con = new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand("sp_SocialLogin", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Provider", provider);
            cmd.Parameters.AddWithValue("@SocialId", socialId);

            await con.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserModel
                {
                    Id = (int)reader["Id"],
                    Username = reader["Username"].ToString(),
                    Email = reader["Email"].ToString(),
                    Provider = reader["Provider"].ToString(),
                    GoogleId = reader["GoogleId"]?.ToString(),
                    FacebookId = reader["FacebookId"]?.ToString()
                };
            }

            return null;
        }
        public async Task SaveRefreshTokenAsync(int userId, string refreshToken)
        {
            using SqlConnection con = new SqlConnection(_connectionString);

            using SqlCommand cmd = new SqlCommand("sp_SaveRefreshToken", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            cmd.Parameters.Add("@RefreshToken", SqlDbType.NVarChar, 500)
                .Value = refreshToken;

            await con.OpenAsync();

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<UserModel?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            using SqlConnection con = new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand("sp_GetUserByRefreshToken", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@RefreshToken", SqlDbType.NVarChar, 500)
                .Value = refreshToken;

            await con.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString(),
                    Email = reader["Email"].ToString()
                };
            }

            return null;
        }
        public async Task<List<string>> GetRolesAsync(int userId)
        {
            List<string> roles = new List<string>();

            using SqlConnection con = new SqlConnection(_connectionString);

            using SqlCommand cmd = new SqlCommand("sp_GetUserRoles", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            await con.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(reader.GetString(0));
            }

            return roles;
        }
    }
}