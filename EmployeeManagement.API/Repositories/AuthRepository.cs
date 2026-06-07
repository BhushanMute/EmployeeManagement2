using Dapper;
using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(IDbConnectionFactory connectionFactory, ILogger<AuthRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        #region Login & Authentication

        public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            try
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
                        PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        ProfilePicture = reader.IsDBNull(reader.GetOrdinal("ProfilePicture"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("ProfilePicture")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                        EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                        FailedLoginAttempts = reader.GetInt32(reader.GetOrdinal("FailedLoginAttempts")),
                        LockoutEndDate = reader.IsDBNull(reader.GetOrdinal("LockoutEndDate"))
                            ? null
                            : reader.GetDateTime(reader.GetOrdinal("LockoutEndDate"))
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username/email: {UsernameOrEmail}", usernameOrEmail);
                throw;
            }
        }

        public async Task<(int UserId, string Message)> RegisterUserAsync(User user, int? roleId, int? createdBy)
        {
            try
            {
                await using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                await using var command = new SqlCommand("sp_RegisterUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = user.Username;
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = user.Email;
                command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value = user.PasswordHash;
                command.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar, 500).Value = user.PasswordSalt;
                command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 100).Value = user.FirstName;
                command.Parameters.Add("@LastName", SqlDbType.NVarChar, 100).Value = user.LastName;
                command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value =
                    (object?)user.PhoneNumber ?? DBNull.Value;
                command.Parameters.Add("@RoleId", SqlDbType.Int).Value =
                    (object?)roleId ?? DBNull.Value;
                command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value =
                    (object?)createdBy ?? DBNull.Value;

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

                int userId = userIdParam.Value != DBNull.Value
                    ? Convert.ToInt32(userIdParam.Value)
                    : 0;

                string message = messageParam.Value?.ToString() ?? "Unknown error occurred";

                _logger.LogInformation("User registration result - UserId: {UserId}, Message: {Message}", userId, message);

                return (userId, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user: {Username}", user.Username);
                throw;
            }
        }

        #endregion

        #region Roles & Permissions

        public async Task<List<Role>> GetRolesAsync(int userId)
        {
            var roles = new List<Role>();

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            var permissions = new List<Permission>();

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> CheckUserPermissionAsync(int userId, string permissionName)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission} for user: {UserId}", permissionName, userId);
                throw;
            }
        }

        #endregion

        #region Login Status & Tokens

        public async Task UpdateLoginStatusAsync(int userId, bool isSuccess, string? ipAddress)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_UpdateLoginStatus", (SqlConnection)connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@IsSuccess", isSuccess);
                command.Parameters.AddWithValue("@IpAddress", (object?)ipAddress ?? DBNull.Value);

                await ((SqlConnection)connection).OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Login status updated for user {UserId}: Success={IsSuccess}", userId, isSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating login status for user: {UserId}", userId);
                throw;
            }
        }

        public async Task SaveRefreshTokenAsync(int userId, string token, DateTime expiryDate, string? ipAddress)
        {
            try
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

                _logger.LogInformation("Refresh token saved for user: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving refresh token for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<(RefreshToken? Token, User? User)> ValidateRefreshTokenAsync(string token)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating refresh token");
                throw;
            }
        }

        public async Task RevokeRefreshTokenAsync(string token, string? ipAddress, string? reason, string? replacedByToken)
        {
            try
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

                _logger.LogInformation("Refresh token revoked. Reason: {Reason}", reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token");
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

                _logger.LogInformation("Revoked all tokens for user: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all tokens for user: {UserId}", userId);
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
                _logger.LogError(ex, "Error getting active tokens for user: {UserId}", userId);
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

                _logger.LogInformation("Cleaned up expired tokens");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired tokens");
                throw;
            }
        }

        #endregion

        #region Password Management

        public async Task<string?> GetUserPasswordHashAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_GetUserPasswordHash", (SqlConnection)connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);

                await ((SqlConnection)connection).OpenAsync();
                var result = await command.ExecuteScalarAsync();

                return result?.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting password hash for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPasswordHash, string newPasswordSalt, int? updatedBy)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_ChangePassword", (SqlConnection)connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@NewPasswordHash", SqlDbType.NVarChar, 500).Value = newPasswordHash;
                command.Parameters.Add("@NewPasswordSalt", SqlDbType.NVarChar, 500).Value = newPasswordSalt;
                command.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = (object?)updatedBy ?? DBNull.Value;

                await ((SqlConnection)connection).OpenAsync();
                var rowsAffected = await command.ExecuteScalarAsync();

                var success = Convert.ToInt32(rowsAffected) > 0;

                _logger.LogInformation("Password change for user {UserId}: {Result}",
                    userId, success ? "Success" : "Failed");

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                throw;
            }
        }

        #endregion

        #region Password Reset Token Management

        /// <summary>
        /// ✅ UPDATED: Save password reset token with IP address and user agent
        /// </summary>
        public async Task SavePasswordResetTokenAsync(int userId, string token, DateTime expiryDate,
            string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_SavePasswordResetToken", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@Token", SqlDbType.NVarChar, 500).Value = token;
                command.Parameters.Add("@ExpiryDate", SqlDbType.DateTime).Value = expiryDate;
                command.Parameters.Add("@IpAddress", SqlDbType.NVarChar, 50).Value =
                    (object?)ipAddress ?? DBNull.Value;
                command.Parameters.Add("@UserAgent", SqlDbType.NVarChar, 500).Value =
                    (object?)userAgent ?? DBNull.Value;

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Password reset token saved for user {UserId}. Expires: {ExpiryDate}",
                    userId, expiryDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving password reset token for user: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// ✅ UPDATED: Validate password reset token with all required fields
        /// </summary>
        public async Task<PasswordResetToken?> ValidatePasswordResetTokenAsync(string token)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_ValidatePasswordResetToken", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add("@Token", SqlDbType.NVarChar, 500).Value = token;

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var resetToken = new PasswordResetToken
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Token = reader.GetString(reader.GetOrdinal("Token")),
                        ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                        IsUsed = reader.GetBoolean(reader.GetOrdinal("IsUsed")),
                        CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate"))
                            ? DateTime.UtcNow
                            : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("Email")),
                        Username = reader.IsDBNull(reader.GetOrdinal("Username"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("Username")),
                        FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.IsDBNull(reader.GetOrdinal("LastName"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("LastName"))
                    };

                    _logger.LogInformation("Password reset token validated for user {UserId}. IsUsed: {IsUsed}, Expired: {Expired}",
                        resetToken.UserId, resetToken.IsUsed, resetToken.ExpiryDate < DateTime.UtcNow);

                    return resetToken;
                }

                _logger.LogWarning("Password reset token not found or invalid");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating password reset token");
                throw;
            }
        }

        /// <summary>
        /// Mark password reset token as used
        /// </summary>
        public async Task MarkPasswordResetTokenUsedAsync(string token)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_MarkPasswordResetTokenUsed", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add("@Token", SqlDbType.NVarChar, 500).Value = token;

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Password reset token marked as used. Rows affected: {Rows}", rowsAffected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking password reset token as used");
                throw;
            }
        }

        /// <summary>
        /// ✅ NEW: Invalidate all unused password reset tokens for a user
        /// </summary>
        public async Task InvalidateAllPasswordResetTokensAsync(int userId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_InvalidateAllPasswordResetTokens", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Invalidated {Count} password reset tokens for user {UserId}",
                    rowsAffected, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating password reset tokens for user: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// ✅ NEW: Cleanup expired password reset tokens
        /// </summary>
        public async Task CleanupExpiredPasswordResetTokensAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var deletedCount = await connection.ExecuteScalarAsync<int>(
                    "sp_CleanupExpiredPasswordResetTokens",
                    commandType: CommandType.StoredProcedure
                );

                _logger.LogInformation("Cleaned up {Count} expired password reset tokens", deletedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired password reset tokens");
                throw;
            }
        }

        #endregion

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
                    var rolesData = await multi.ReadAsync<dynamic>();
                    user.Roles = MapRoles(rolesData);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
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
                    var rolesData = await multi.ReadAsync<dynamic>();
                    user.Roles = MapRoles(rolesData);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
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
                    var rolesData = await multi.ReadAsync<dynamic>();
                    user.Roles = MapRoles(rolesData);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
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

                _logger.LogInformation("Created user with ID: {UserId}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Username}", user.Username);
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
                        FullName = user.FirstName + " " + user.LastName,
                        user.PhoneNumber,
                        user.IsActive,
                        user.LastLoginDate,
                        user.ProfilePicture
                    },
                    commandType: CommandType.StoredProcedure
                );

                _logger.LogInformation("Updated user: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", user.Id);
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

                _logger.LogInformation("Updated last login for user: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login for user: {UserId}", userId);
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
                _logger.LogError(ex, "Error checking username exists: {Username}", username);
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
                _logger.LogError(ex, "Error checking email exists: {Email}", email);
                throw;
            }
        }

        #endregion

        #region Audit Logging

        public async Task LogAuditAsync(int? userId, string action, string? tableName, int? recordId,
            string? oldValues, string? newValues, string? ipAddress, string? userAgent)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging audit for action: {Action}", action);
                // Don't throw - audit logging failures shouldn't break main operations
            }
        }

        #endregion

        #region Private Helper Methods

        private List<UserRole> MapRoles(IEnumerable<dynamic> rolesData)
        {
            var roles = new List<UserRole>();

            foreach (var item in rolesData)
            {
                roles.Add(new UserRole
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

            return roles;
        }

        #endregion
    }
}
