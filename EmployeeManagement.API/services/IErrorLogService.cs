using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.services
{
    public interface IErrorLogService
    {
        Task<Guid> LogErrorAsync(ErrorLog errorLog);
        Task<Guid> LogErrorAsync( Exception exception, HttpContext httpContext, int? userId = null, string? username = null);
        Task<List<ErrorLog>> GetErrorLogsAsync(int top = 100, bool? isResolved = null);
        Task<ErrorLog?> GetErrorDetailsAsync(Guid errorId);
        Task<bool> MarkAsResolvedAsync(Guid errorId, int resolvedBy, string? notes = null);
    }
}
