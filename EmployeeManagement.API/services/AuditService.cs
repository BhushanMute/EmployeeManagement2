using EmployeeManagement.API.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.services
{
    public class AuditService : IAuditService
    {
        private readonly DbHelper _dbHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(DbHelper dbHelper, IHttpContextAccessor httpContextAccessor)
        {
            _dbHelper = dbHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(int? userId, string action, string? tableName = null, int? recordId = null,
    string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null)
        {
            var context = _httpContextAccessor.HttpContext;
            ipAddress ??= context?.Connection.RemoteIpAddress?.ToString();
            userAgent ??= context?.Request.Headers["User-Agent"].ToString();

            using var connection = _dbHelper.GetConnection(); // ✅ Already open
            using var command = new SqlCommand("sp_LogAudit", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Action", action);
            command.Parameters.AddWithValue("@TableName", (object?)tableName ?? DBNull.Value);
            command.Parameters.AddWithValue("@RecordId", (object?)recordId ?? DBNull.Value);
            command.Parameters.AddWithValue("@OldValues", (object?)oldValues ?? DBNull.Value);
            command.Parameters.AddWithValue("@NewValues", (object?)newValues ?? DBNull.Value);
            command.Parameters.AddWithValue("@IpAddress", (object?)ipAddress ?? DBNull.Value);
            command.Parameters.AddWithValue("@UserAgent", (object?)userAgent ?? DBNull.Value);

            // ❌ REMOVE THIS LINE
            // await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

    }
}  
