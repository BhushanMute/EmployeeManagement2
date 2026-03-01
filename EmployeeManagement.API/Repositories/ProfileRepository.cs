// Repositories/ProfileRepository.cs
using Dapper;
using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<ProfileRepository> _logger;

        public ProfileRepository(IDbConnectionFactory connectionFactory, ILogger<ProfileRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<ProfileResponse?> GetUserProfileAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetUserProfile",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure
                );

                var profile = await multi.ReadFirstOrDefaultAsync<ProfileResponse>();

                if (profile != null)
                {
                    // ✅ FIX: Map Role objects to RoleName strings
                    var roles = await multi.ReadAsync<Role>();
                    profile.Roles = roles.Select(r => r.RoleName).ToList();

                    // ✅ FIX: Map Permission objects to PermissionName strings
                    var permissions = await multi.ReadAsync<Permission>();
                    profile.Permissions = permissions.Select(p => p.PermissionName).ToList();
                }

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile for user {UserId}", userId);
                throw;
            }
        }

        public async Task<(bool Success, string Message)> UpdateUserProfileAsync(int userId, UpdateProfileRequest request, int? updatedBy)
        {
            try
            {
                _logger.LogInformation("Updating profile for UserId: {UserId}", userId);

                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_UpdateUserProfile", (SqlConnection)connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@FirstName", request.FirstName);
                command.Parameters.AddWithValue("@LastName", request.LastName);
                command.Parameters.AddWithValue("@Email", request.Email);
                command.Parameters.AddWithValue("@PhoneNumber", (object?)request.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@DateOfBirth", (object?)request.DateOfBirth ?? DBNull.Value);
                command.Parameters.AddWithValue("@Address", (object?)request.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@ProfilePicture", DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);

                await ((SqlConnection)connection).OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // ✅ FIX: SP returns INT (0 or 1), not BOOLEAN
                    var success = reader.GetInt32(reader.GetOrdinal("Success")) == 1;
                    var message = reader.GetString(reader.GetOrdinal("Message"));

                    _logger.LogInformation("Update result - Success: {Success}, Message: {Message}", success, message);

                    return (success, message);
                }

                return (false, "No response from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for user {UserId}", userId);
                throw;
            }
        }
            public async Task<(bool Success, string Message)> UpdateProfilePictureAsync(int userId, string profilePictureUrl, int? updatedBy)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_UpdateProfilePicture", (SqlConnection)connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@ProfilePicture", profilePictureUrl);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);

                await ((SqlConnection)connection).OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // ✅ Fix: Read value as object first, then convert
                    var successObj = reader["Success"];
                    bool success;

                    // Handle both INT and BIT return types
                    if (successObj is bool boolValue)
                    {
                        success = boolValue;
                    }
                    else if (successObj is int intValue)
                    {
                        success = intValue == 1;
                    }
                    else
                    {
                        success = Convert.ToBoolean(successObj);
                    }

                    var message = reader["Message"]?.ToString() ?? "Unknown";

                    return (success, message);
                }

                return (false, "No response from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile picture for user {UserId}", userId);
                throw;
            }
        }
        public async Task<List<UserActivityLog>> GetUserActivityLogAsync(int userId, int pageNumber, int pageSize)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetUserActivityLog",
                    new { UserId = userId, PageNumber = pageNumber, PageSize = pageSize },
                    commandType: CommandType.StoredProcedure
                );

                // Skip total count
                await multi.ReadFirstOrDefaultAsync<int>();

                // Get activity logs
                var logs = await multi.ReadAsync<UserActivityLog>();
                return logs.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activity log for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<UserSession>> GetUserSessionsAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sessions = await connection.QueryAsync<UserSession>(
                    "sp_GetUserSessions",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure
                );

                return sessions.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for user {UserId}", userId);
                throw;
            }
        }

        public async Task<(bool Success, string Message)> RevokeUserSessionAsync(int userId, string sessionToken)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_RevokeUserSession", (SqlConnection)connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@SessionToken", sessionToken);

                await ((SqlConnection)connection).OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return (
                        reader.GetBoolean(reader.GetOrdinal("Success")),
                        reader.GetString(reader.GetOrdinal("Message"))
                    );
                }

                return (false, "Revoke failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking session for user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserSettings?> GetUserSettingsAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var settings = await connection.QueryFirstOrDefaultAsync<UserSettings>(
                    "sp_GetUserSettings",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure
                );

                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting settings for user {UserId}", userId);
                throw;
            }
        }

        public async Task<(bool Success, string Message)> UpdateUserSettingsAsync(int userId, UpdateUserSettingsRequest request)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_UpdateUserSettings", (SqlConnection)connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Theme", request.Theme);
                command.Parameters.AddWithValue("@Language", request.Language);
                command.Parameters.AddWithValue("@EmailNotifications", request.EmailNotifications);
                command.Parameters.AddWithValue("@TwoFactorEnabled", request.TwoFactorEnabled);

                await ((SqlConnection)connection).OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return (
                        reader.GetBoolean(reader.GetOrdinal("Success")),
                        reader.GetString(reader.GetOrdinal("Message"))
                    );
                }

                return (false, "Update failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating settings for user {UserId}", userId);
                throw;
            }
        }
        public async Task<ProfileResponse?> GetProfileByIdAsync(int id)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetUserProfile",  // Same SP, just different parameter name
                    new { UserId = id },
                    commandType: CommandType.StoredProcedure
                );

                var profile = await multi.ReadFirstOrDefaultAsync<ProfileResponse>();

                if (profile != null)
                {
                    var roles = await multi.ReadAsync<Role>();
                    profile.Roles = roles.Select(r => r.RoleName).ToList();

                    // ✅ FIX: Map Permission objects to PermissionName strings
                    var permissions = await multi.ReadAsync<Permission>();
                    profile.Permissions = permissions.Select(p => p.PermissionName).ToList();
                }

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile by ID: {Id}", id);
                throw;
            }
        }
        
    }
}