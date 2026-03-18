using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<AuditLogRepository> _logger;

        public AuditLogRepository(IDbConnectionFactory connectionFactory, ILogger<AuditLogRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task LogAsync(AuditLog auditLog)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_InsertAuditLog", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@UserId", (object?)auditLog.UserId ?? DBNull.Value);
                command.Parameters.AddWithValue("@Username", (object?)auditLog.Username ?? DBNull.Value);
                command.Parameters.AddWithValue("@Action", auditLog.Action);
                command.Parameters.AddWithValue("@EntityName", (object?)auditLog.EntityName ?? DBNull.Value);
                command.Parameters.AddWithValue("@EntityId", (object?)auditLog.EntityId ?? DBNull.Value);
                command.Parameters.AddWithValue("@OldValues", (object?)auditLog.OldValues ?? DBNull.Value);
                command.Parameters.AddWithValue("@NewValues", (object?)auditLog.NewValues ?? DBNull.Value);
                command.Parameters.AddWithValue("@IpAddress", (object?)auditLog.IpAddress ?? DBNull.Value);
                command.Parameters.AddWithValue("@UserAgent", (object?)auditLog.UserAgent ?? DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging audit entry");
            }
        }

        public async Task<(List<AuditLog> Logs, int TotalRecords)> GetLogsAsync(AuditLogRequest request)
        {
            var logs = new List<AuditLog>();
            int totalRecords = 0;

            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_GetAuditLogs", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add("@PageNumber", SqlDbType.Int).Value = request.PageNumber > 0 ? request.PageNumber : 1;
                command.Parameters.Add("@PageSize", SqlDbType.Int).Value = request.PageSize > 0 ? request.PageSize : 50;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = (object?)request.UserId ?? DBNull.Value;
                command.Parameters.Add("@Action", SqlDbType.NVarChar, 100).Value = (object?)request.Action ?? DBNull.Value;
                command.Parameters.Add("@EntityName", SqlDbType.NVarChar, 100).Value = (object?)request.EntityName ?? DBNull.Value;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = DBNull.Value;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = DBNull.Value;

                _logger.LogInformation("Calling sp_GetAuditLogs - Page: {Page}, Size: {Size}",
                    request.PageNumber, request.PageSize);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                _logger.LogInformation("Reader HasRows: {HasRows}", reader.HasRows);

                while (await reader.ReadAsync())
                {
                    var log = new AuditLog
                    {
                        Id = GetSafeInt32(reader, "Id"),  // ✅ Make sure this is being read!
                        UserId = GetSafeNullableInt32(reader, "UserId"),
                        Username = GetSafeString(reader, "Username"),
                        Action = GetSafeString(reader, "Action") ?? "Unknown",
                        EntityName = GetSafeString(reader, "EntityName"),
                        EntityId = GetSafeNullableInt32(reader, "EntityId"),
                        OldValues = GetSafeString(reader, "OldValues"),
                        NewValues = GetSafeString(reader, "NewValues"),
                        IpAddress = GetSafeString(reader, "IpAddress"),
                        UserAgent = GetSafeString(reader, "UserAgent"),
                        Timestamp = GetSafeDateTime(reader, "Timestamp")
                    };

                    // ✅ Debug: Log the Id
                    _logger.LogInformation("Read log: Id={Id}, Action={Action}", log.Id, log.Action);

                    logs.Add(log);
                }

                _logger.LogInformation("Total logs read: {Count}", logs.Count);

                // Read total count
                if (await reader.NextResultAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        totalRecords = GetSafeInt32(reader, 0);
                        _logger.LogInformation("Total records: {Total}", totalRecords);
                    }
                }

                return (logs, totalRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetLogsAsync: {Message}", ex.Message);
                throw;
            }
        }
        #region Safe Reader Helper Methods

        private int GetSafeInt32(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal)) return 0;
                return reader.GetInt32(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private int GetSafeInt32(SqlDataReader reader, int ordinal)
        {
            try
            {
                if (reader.IsDBNull(ordinal)) return 0;

                // Handle different numeric types
                var value = reader.GetValue(ordinal);
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        private int? GetSafeNullableInt32(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal)) return null;

                // Handle different numeric types
                var value = reader.GetValue(ordinal);
                return Convert.ToInt32(value);
            }
            catch
            {
                return null;
            }
        }

        private string? GetSafeString(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal)) return null;
                return reader.GetString(ordinal);
            }
            catch
            {
                return null;
            }
        }

        private DateTime GetSafeDateTime(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal)) return DateTime.MinValue;
                return reader.GetDateTime(ordinal);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        #endregion    }
    }

}