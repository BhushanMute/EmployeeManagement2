using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Services
{
    /// <summary>
    /// Audit Service - Tracks user actions in system
    /// ऑडिट सेवा - सर्व क्रिया लॉग करते
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly DbHelper _dbHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            DbHelper dbHelper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _dbHelper = dbHelper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        #region Public Methods

        /// <summary>
        /// Log audit using individual parameters
        /// </summary>
        public async Task LogAsync(
            int? userId,
            string action,
            string? tableName = null,
            int? recordId = null,
            string? oldValues = null,
            string? newValues = null,
            string? ipAddress = null,
            string? userAgent = null)
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;

                ipAddress ??= GetIpAddress(context);
                userAgent ??= GetUserAgent(context);

                using var connection = _dbHelper.GetConnection(); // already open
                using var command = CreateCommand(connection, "sp_LogAudit");

                AddParameter(command, "@UserId", userId);
                AddParameter(command, "@Action", action);
                AddParameter(command, "@TableName", tableName);
                AddParameter(command, "@RecordId", recordId);
                AddParameter(command, "@OldValues", oldValues);
                AddParameter(command, "@NewValues", newValues);
                AddParameter(command, "@IpAddress", ipAddress);
                AddParameter(command, "@UserAgent", userAgent);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing audit log: Action={Action}, UserId={UserId}", action, userId);
                // Do NOT throw → audit failure should not break main flow
            }
        }

        /// <summary>
        /// Log audit using AuditLog object (BEST PRACTICE)
        /// </summary>
        public async Task LogAsync(AuditLog log)
        {
            if (log == null)
                return;

            await LogAsync(
                log.UserId,
                log.Action,
                log.EntityName,
                log.EntityId,
                log.OldValues,
                log.NewValues,
                log.IpAddress,
                log.UserAgent
            );
        }

        /// <summary>
        /// Log simple action (shortcut)
        /// </summary>
        public async Task LogActionAsync(int? userId, string action)
        {
            await LogAsync(userId, action);
        }

        /// <summary>
        /// Log create operation
        /// </summary>
        public async Task LogCreateAsync(int userId, string tableName, int recordId, string? newValues = null)
        {
            await LogAsync(
                userId,
                "CREATE",
                tableName,
                recordId,
                null,
                newValues
            );
        }

        /// <summary>
        /// Log update operation
        /// </summary>
        public async Task LogUpdateAsync(int userId, string tableName, int recordId, string? oldValues, string? newValues)
        {
            await LogAsync(
                userId,
                "UPDATE",
                tableName,
                recordId,
                oldValues,
                newValues
            );
        }

        /// <summary>
        /// Log delete operation
        /// </summary>
        public async Task LogDeleteAsync(int userId, string tableName, int recordId, string? oldValues = null)
        {
            await LogAsync(
                userId,
                "DELETE",
                tableName,
                recordId,
                oldValues,
                null
            );
        }

        #endregion

        #region Private Helpers

        private static SqlCommand CreateCommand(SqlConnection connection, string storedProcedure)
        {
            return new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private static void AddParameter(SqlCommand command, string name, object? value)
        {
            command.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        private static string? GetIpAddress(HttpContext? context)
        {
            return context?.Connection?.RemoteIpAddress?.ToString();
        }

        private static string? GetUserAgent(HttpContext? context)
        {
            return context?.Request?.Headers["User-Agent"].ToString();
        }

        #endregion
    }
}