using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IAuditLogRepository
    {
        Task LogAsync(AuditLog auditLog);
        Task<(List<AuditLog> Logs, int TotalRecords)> GetLogsAsync(AuditLogRequest request);
    }
}
