using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IReportRepository
    {
        Task<EmployeeReportData> GetEmployeeReport(int? departmentId, bool? isActive,
            DateTime? joiningFrom, DateTime? joiningTo, string? searchTerm);
        Task<AttendanceReportData> GetAttendanceReport(int month, int year,
            int? departmentId, int? employeeId);
        Task<SalaryReportData> GetSalaryReport(int? departmentId, int? month, int? year);
    }
}