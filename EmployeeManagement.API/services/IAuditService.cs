namespace EmployeeManagement.API.services
{
    public interface IAuditService
    {
        Task LogAsync(int? userId, string action, string? tableName = null, int? recordId = null,
           string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null);
    }
}
