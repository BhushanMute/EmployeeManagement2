using EmployeeManagement.API.Models;
using EmployeeManagement.API.Common;

namespace EmployeeManagement.API.Repositories
{
    public interface ILeaveRepository
    {
        // Leave Types
        Task<List<LeaveType>> GetAllLeaveTypes();
        Task<LeaveType?> GetLeaveTypeById(int id);

        // Leave Requests
        Task<int> ApplyLeave(LeaveRequest request);
        Task<LeaveRequest?> GetLeaveRequestById(int id);
        Task<List<LeaveRequest>> GetEmployeeLeaveHistory(int employeeId, int? year = null);
        Task<List<LeaveRequest>> GetPendingLeaveRequests(int? approverId = null, int? departmentId = null);
        Task<PagedResult<LeaveRequest>> GetAllLeaveRequests(string? status, int? departmentId, int? leaveTypeId,
            DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);

        // Leave Actions
        Task ApproveLeave(int leaveRequestId, int approvedBy, string? remarks);
        Task RejectLeave(int leaveRequestId, int rejectedBy, string? remarks);
        Task CancelLeave(int leaveRequestId, int cancelledBy, string? cancelReason);

        // Leave Balance
        Task<List<LeaveBalance>> GetLeaveBalance(int employeeId, int? year = null);
        Task AllocateLeaveBalance(int employeeId, int leaveTypeId, int year, decimal totalAllocated,
            decimal carryForward, int? createdBy);
        Task AllocateDefaultLeaveForAllEmployees(int year, int? createdBy);

        // Holidays
        Task<List<Holiday>> GetHolidays(int? year = null);
         Task<Holiday?> GetHolidayById(int id);
        Task<int> AddHoliday(Holiday holiday);
        Task UpdateHoliday(Holiday holiday);
        Task DeleteHoliday(int id, int? deletedBy);
        Task<HolidayStats> GetHolidaysCount(int? year = null);

        // ========== Reports ==========
        Task<List<MonthlyLeaveReportItem>> GetMonthlyLeaveReport(int month, int year, int? departmentId = null);
        
        Task<List<EmployeeLeaveReportItem>> GetEmployeeLeaveReport(int? year, int? departmentId, int? employeeId);
        Task<List<DepartmentLeaveReportItem>> GetDepartmentLeaveReport(int? year);
         Task<List<LeaveCalendarItem>> GetLeaveCalendarData(int month, int year, int? departmentId);
        Task<LeaveDashboardStats> GetLeaveDashboardStats(int? year);
 
        // ========== Leave Allocation ==========
        Task<int> AllocateFixedLeaveForAllEmployees(int year, decimal leavesPerType, int? createdBy);
        Task AllocateLeaveForSingleEmployee(int employeeId, int year, decimal leavesPerType, int? createdBy);
        Task<List<LeaveBalance>> GetAllEmployeeBalances(int year);

        Task<AdminLeaveDashboard> GetAdminLeaveDashboard(int? year = null);
        Task<int> CreateLeaveRequestAsync(LeaveRequest request, int createdBy);


    }
}