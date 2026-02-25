using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<RefreshTokenRepository> _logger;

        public RefreshTokenRepository(IConfiguration configuration, ILogger<RefreshTokenRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));

            _logger = logger;
        }

        // ✅ Save refresh token
        public async Task<int> SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_SaveRefreshToken", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserId", refreshToken.UserId);
                command.Parameters.AddWithValue("@Token", refreshToken.Token);
                command.Parameters.AddWithValue("@ExpiryDate", refreshToken.ExpiryDate);

                var result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving refresh token for user {UserId}", refreshToken.UserId);
                throw new ApplicationException("Failed to save refresh token", ex);
            }
        }

        // ✅ Get refresh token
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_GetRefreshToken", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Token", token);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new RefreshToken
                    {
                        Id = reader.GetInt32("Id"),
                        UserId = reader.GetInt32("UserId"),
                        Token = reader.GetString("Token"),
                        ExpiryDate = reader.GetDateTime("ExpiryDate"),
                        CreatedDate = reader.GetDateTime("CreatedDate"),
                        IsRevoked = reader.GetBoolean("IsRevoked"),
                        RevokedDate = reader.IsDBNull("RevokedDate") ? null : reader.GetDateTime("RevokedDate")
                        //ReplacedByToken = reader.IsDBNull("ReplacedByToken") ? null : reader.GetString("ReplacedByToken")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving refresh token");
                throw new ApplicationException("Failed to retrieve refresh token", ex);
            }
        }

        // ✅ Revoke single refresh token
        public async Task RevokeRefreshTokenAsync(string token, string? replacedByToken = null)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_RevokeRefreshToken", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Token", token);
                command.Parameters.AddWithValue("@ReplacedByToken",
                    (object?)replacedByToken ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token");
                throw new ApplicationException("Failed to revoke refresh token", ex);
            }
        }

        // ✅ Revoke all tokens of user
        public async Task RevokeAllUserTokensAsync(int userId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_RevokeAllUserTokens", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserId", userId);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all tokens for user {UserId}", userId);
                throw new ApplicationException("Failed to revoke user tokens", ex);
            }
        }
    }
}