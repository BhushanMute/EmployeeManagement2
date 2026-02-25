using Dapper;
using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        //private readonly string _connectionString;

        //public AuthRepository(IConfiguration configuration)
        //{
        //    _connectionString = configuration.GetConnectionString("DefaultConnection")
        //        ?? throw new InvalidOperationException(
        //            "Connection string 'DefaultConnection' not found.");
        //}

        //public async Task Register(RegisterRequest model)
        //{
        //    using SqlConnection con = new SqlConnection(_connectionString);
        //    using SqlCommand cmd = new SqlCommand("sp_RegisterUser", con);

        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@Username", model.Username);
        //    cmd.Parameters.AddWithValue(
        //        "@PasswordHash",
        //        BCrypt.Net.BCrypt.HashPassword(model.Password)
        //    );

        //    await con.OpenAsync();
        //    await cmd.ExecuteNonQueryAsync();
        //}
        private readonly IDbConnectionFactory _connectionFactory;

        public AuthRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_LoginUser", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UsernameOrEmail", usernameOrEmail);

            await ((SqlConnection)connection).OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                    LockoutEndDate = reader.IsDBNull(reader.GetOrdinal("LockoutEndDate"))
                        ? null : reader.GetDateTime(reader.GetOrdinal("LockoutEndDate")),
                    FailedLoginAttempts = reader.GetInt32(reader.GetOrdinal("FailedLoginAttempts")),
                    EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed"))
                };
            }

            return null;
        }

        public async Task<(int UserId, string Message)> RegisterUserAsync(User user, int roleId, int? createdBy)
        {
            await using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await using var command = new SqlCommand("sp_RegisterUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // 🔹 Explicit parameter types (Best Practice)
            command.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = user.Username;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = user.Email;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value = user.PasswordHash;
            command.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar, 500).Value = user.PasswordSalt;
            command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 100).Value = user.FirstName;
            command.Parameters.Add("@LastName", SqlDbType.NVarChar, 100).Value = user.LastName;
            command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value =
                (object?)user.PhoneNumber ?? DBNull.Value;
            command.Parameters.Add("@RoleId", SqlDbType.Int).Value = roleId;
            command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value =
                (object?)createdBy ?? DBNull.Value;

            // 🔹 Output Parameters
            var userIdParam = new SqlParameter("@UserId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(userIdParam);
            command.Parameters.Add(messageParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            // 🔹 Safe output handling
            int userId = userIdParam.Value != DBNull.Value
                ? Convert.ToInt32(userIdParam.Value)
                : 0;

            string message = messageParam.Value?.ToString() ?? "Unknown error occurred";

            return (userId, message);
        }
        public async Task<List<Role>> GetRolesAsync(int userId)
        {
            var roles = new List<Role>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_GetRoles", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);

            await ((SqlConnection)connection).OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(new Role
                {
                    Id = reader.GetInt32(reader.GetOrdinal("RoleId")),
                    RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                        ? null : reader.GetString(reader.GetOrdinal("Description"))
                });
            }

            return roles;
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            var permissions = new List<Permission>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_GetUserPermissions", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);

            await ((SqlConnection)connection).OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                permissions.Add(new Permission
                {
                    Id = reader.GetInt32(reader.GetOrdinal("PermissionId")),
                    PermissionName = reader.GetString(reader.GetOrdinal("PermissionName")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                        ? null : reader.GetString(reader.GetOrdinal("Description")),
                    Module = reader.IsDBNull(reader.GetOrdinal("Module"))
                        ? null : reader.GetString(reader.GetOrdinal("Module"))
                });
            }

            return permissions;
        }

        public async Task UpdateLoginStatusAsync(int userId, bool isSuccess, string? ipAddress)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_UpdateLoginStatus", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@IsSuccess", isSuccess);
            command.Parameters.AddWithValue("@IpAddress", (object?)ipAddress ?? DBNull.Value);

            await ((SqlConnection)connection).OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task SaveRefreshTokenAsync(int userId, string token, DateTime expiryDate, string? ipAddress)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_SaveRefreshToken", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Token", token);
            command.Parameters.AddWithValue("@ExpiryDate", expiryDate);
            command.Parameters.AddWithValue("@CreatedByIp", (object?)ipAddress ?? DBNull.Value);

            await ((SqlConnection)connection).OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<(RefreshToken? Token, User? User)> ValidateRefreshTokenAsync(string token)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_ValidateRefreshToken", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Token", token);

            await ((SqlConnection)connection).OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var refreshToken = new RefreshToken
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Token = reader.GetString(reader.GetOrdinal("Token")),
                    ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                    RevokedDate = reader.IsDBNull(reader.GetOrdinal("RevokedDate"))
                        ? null : reader.GetDateTime(reader.GetOrdinal("RevokedDate"))
                };

                var user = new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };

                return (refreshToken, user);
            }

            return (null, null);
        }

        public async Task RevokeRefreshTokenAsync(string token, string? ipAddress, string? reason, string? replacedByToken)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_RevokeRefreshToken", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Token", token);
            command.Parameters.AddWithValue("@RevokedByIp", (object?)ipAddress ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReasonRevoked", (object?)reason ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReplacedByToken", (object?)replacedByToken ?? DBNull.Value);

            await ((SqlConnection)connection).OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> CheckUserPermissionAsync(int userId, string permissionName)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_CheckUserPermission", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@PermissionName", permissionName);

            var hasPermissionParam = new SqlParameter("@HasPermission", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(hasPermissionParam);

            await ((SqlConnection)connection).OpenAsync();
            await command.ExecuteNonQueryAsync();

            return (bool)hasPermissionParam.Value;
        }

        public async Task LogAuditAsync(int? userId, string action, string? tableName, int? recordId,
            string? oldValues, string? newValues, string? ipAddress, string? userAgent)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_LogAudit", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Action", action);
            command.Parameters.AddWithValue("@TableName", (object?)tableName ?? DBNull.Value);
            command.Parameters.AddWithValue("@RecordId", (object?)recordId ?? DBNull.Value);
            command.Parameters.AddWithValue("@OldValues", (object?)oldValues ?? DBNull.Value);
            command.Parameters.AddWithValue("@NewValues", (object?)newValues ?? DBNull.Value);
            command.Parameters.AddWithValue("@IpAddress", (object?)ipAddress ?? DBNull.Value);
            command.Parameters.AddWithValue("@UserAgent", (object?)userAgent ?? DBNull.Value);

            await ((SqlConnection)connection).OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<string?> GetUserPasswordHashAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_GetUserPasswordHash", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);

            await ((SqlConnection)connection).OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result?.ToString();
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPasswordHash, string newPasswordSalt, int? updatedBy)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_ChangePassword", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);
            command.Parameters.AddWithValue("@NewPasswordSalt", newPasswordSalt);  // ✅ Add this
            command.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);

            await ((SqlConnection)connection).OpenAsync();
            var rowsAffected = await command.ExecuteScalarAsync();

            return Convert.ToInt32(rowsAffected) > 0;
        }

        public async Task SavePasswordResetTokenAsync(int userId, string token, DateTime expiryDate)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_SavePasswordResetToken", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Token", token);
            command.Parameters.AddWithValue("@ExpiryDate", expiryDate);

            await ((SqlConnection)connection).OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<PasswordResetToken?> ValidatePasswordResetTokenAsync(string token)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_ValidatePasswordResetToken", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Token", token);

            await ((SqlConnection)connection).OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new PasswordResetToken
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Token = reader.GetString(reader.GetOrdinal("Token")),
                    ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                    IsUsed = reader.GetBoolean(reader.GetOrdinal("IsUsed")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                    Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username"))
                };
            }

            return null;
        }

        public async Task MarkPasswordResetTokenUsedAsync(string token)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_MarkPasswordResetTokenUsed", (SqlConnection)connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Token", token);

            await ((SqlConnection)connection).OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
        #region User Methods

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetUserByUsername",
                    new { Username = username },
                    commandType: CommandType.StoredProcedure
                );

                var user = await multi.ReadFirstOrDefaultAsync<User>();

                if (user != null)
                {
                    var RolesData = await multi.ReadAsync<dynamic>();
                    user.Roles = MapRoles(RolesData);
                }

                return user;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetUserByEmail",
                    new { Email = email },
                    commandType: CommandType.StoredProcedure
                );

                var user = await multi.ReadFirstOrDefaultAsync<User>();

                if (user != null)
                {
                    var RolesData = await multi.ReadAsync<dynamic>();
                    user.Roles = MapRoles(RolesData);
                }

                return user;
            }
            catch (Exception ex)
            {
              //  _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetUserById",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure
                );

                var user = await multi.ReadFirstOrDefaultAsync<User>();

                if (user != null)
                {
                    var RolesData = await multi.ReadAsync<dynamic>();
                    user.Roles = MapRoles(RolesData);
                }

                return user;
            }
            catch (Exception ex)
            {
              //  _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
                throw;
            }
        }

        

        public async Task<int> CreateUserAsync(User user)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_CreateUser",
                    new
                    {
                        user.Username,
                        user.Email,
                        user.PasswordHash,
                        user.FirstName,
                        user.LastName,
                        FullName = user.FirstName + " " + user.LastName,
                        user.PhoneNumber,
                        user.IsActive
                    },
                    commandType: CommandType.StoredProcedure
                );

               // _logger.LogInformation("Created user with ID: {UserId}", result);
                return result;
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error creating user: {Username}", user.Username);
                throw;
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                await connection.ExecuteAsync(
                    "sp_UpdateUser",
                    new
                    {
                        UserId = user.Id,
                        user.Username,
                        user.Email,
                        user.PasswordHash,
                        user.FirstName,
                        user.LastName,
                        fullName = user.FirstName + user.LastName,
                        user.PhoneNumber,
                        user.IsActive,
                        user.LastLoginDate
                    },
                    commandType: CommandType.StoredProcedure
                );

               // _logger.LogInformation("Updated user: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error updating user: {UserId}", user.Id);
                throw;
            }
        }

        public async Task UpdateUserLastLoginAsync(int userId, DateTime lastLoginDate, string? ipAddress)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                await connection.ExecuteAsync(
                    "sp_UpdateUserLastLogin",
                    new
                    {
                        UserId = userId,
                        LastLoginDate = lastLoginDate,
                        LastLoginIp = ipAddress
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error updating last login for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var exists = await connection.QueryFirstOrDefaultAsync<bool>(
                    "sp_CheckUsernameExists",
                    new { Username = username },
                    commandType: CommandType.StoredProcedure
                );

                return exists;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error checking username exists: {Username}", username);
                throw;
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var exists = await connection.QueryFirstOrDefaultAsync<bool>(
                    "sp_CheckEmailExists",
                    new { Email = email },
                    commandType: CommandType.StoredProcedure
                );

                return exists;
            }
            catch (Exception ex)
            {
                //o_logger.LogError(ex, "Error checking email exists: {Email}", email);
                throw;
            }
        }
        public async Task RevokeAllUserTokensAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                await connection.ExecuteAsync(
                    "sp_RevokeAllUserTokens",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure
                );

                //_logger.LogInformation("Revoked all tokens for user: {UserId}", userId);
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error revoking all tokens for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<RefreshToken>> GetUserActiveTokensAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var tokens = await connection.QueryAsync<RefreshToken>(
                    "sp_GetUserActiveTokens",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure
                );

                return tokens.ToList();
            }
            catch (Exception ex)
            {
               /// _logger.LogError(ex, "Error getting active tokens for user: {UserId}", userId);
                throw;
            }
        }

        public async Task CleanupExpiredTokensAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                await connection.ExecuteAsync(
                    "sp_CleanupExpiredTokens",
                    commandType: CommandType.StoredProcedure
                );

                //_logger.LogInformation("Cleaned up expired tokens");
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error cleaning up expired tokens");
                throw;
            }
        }


        #endregion
        #region Private Helper Methods

        private List<UserRole> MapRoles(IEnumerable<dynamic> RolesData)
        {
            var Roles = new List<UserRole>();

            foreach (var item in RolesData)
            {
                Roles.Add(new UserRole
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    RoleId = item.RoleId,
                    IsActive = item.IsActive,
                    AssignedDate = item.AssignedDate,
                    AssignedBy = item.AssignedBy,
                    Role = new Role
                    {
                        Id = item.RoleId,
                        RoleName = item.RoleName,
                        Description = item.Description,
                        IsActive = item.RoleIsActive
                    }
                });
            }

            return Roles;
        }

#endregion

    }



}
 