using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string action, string? entityName = null, int? entityId = null,
            object? oldValues = null, object? newValues = null, int? userId = null,
            string? username = null, string? ipAddress = null, string? userAgent = null);

        Task<ApiResponse<AuditLogResponse>> GetLogsAsync(AuditLogRequest request);
        Task<ApiResponse<AuditLog>> GetByIdAsync(int id);

    }
}