using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using System.Text.Json;

namespace EmployeeManagement.API.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repository;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(IAuditLogRepository repository, ILogger<AuditLogService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task LogAsync(string action, string? entityName = null, int? entityId = null,
            object? oldValues = null, object? newValues = null, int? userId = null,
            string? username = null, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Username = username,
                    Action = action,
                    EntityName = entityName,
                    EntityId = entityId,
                    OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow
                };

                await _repository.LogAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in audit logging");
            }
        }

        public async Task<ApiResponse<AuditLogResponse>> GetLogsAsync(AuditLogRequest request)
        {
            try
            {
                var (logs, totalRecords) = await _repository.GetLogsAsync(request);

                var response = new AuditLogResponse
                {
                    Logs = logs,
                    TotalRecords = totalRecords,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return ApiResponse<AuditLogResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit logs");
                return ApiResponse<AuditLogResponse>.Fail("Failed to retrieve audit logs");
            }
        }
        public async Task<ApiResponse<AuditLog>> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting audit log by ID: {Id}", id);

                var request = new AuditLogRequest { PageNumber = 1, PageSize = 10000 };
                var (logs, _) = await _repository.GetLogsAsync(request);
                var log = logs.FirstOrDefault(l => l.Id == id);

                if (log == null)
                {
                    return ApiResponse<AuditLog>.Fail("Audit log not found");
                }

                return ApiResponse<AuditLog>.Success(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit log by ID: {Id}", id);
                return ApiResponse<AuditLog>.Fail("Failed to retrieve audit log");
            }
        }
    }
}