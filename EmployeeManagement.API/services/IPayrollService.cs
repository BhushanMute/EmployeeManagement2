using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;

namespace EmployeeManagement.API.Services.Interfaces
{
    /// <summary>
    /// Interface for Payroll Service
    /// वेतन प्रक्रिया सेवा इंटरफेस
    /// </summary>
    public interface IPayrollService
    {
        // Payroll Cycle Management
        Task<ApiResponse<int>> CreatePayrollCycleAsync(int month, int year, int createdBy);
        Task<ApiResponse<PayrollSummaryResponse>> GetPayrollSummaryAsync(int cycleId);
        Task<ApiResponse<List<PayrollSummaryResponse>>> GetPayrollCyclesAsync(int year);
        Task<ApiResponse<bool>> LockPayrollCycleAsync(int cycleId, int userId);
        Task<ApiResponse<bool>> ApprovePayrollCycleAsync(int cycleId, int userId, string? remarks);

        // Payroll Processing
        Task<ApiResponse<int>> ProcessSingleEmployeePayrollAsync(ProcessSingleEmployeePayrollRequest request, int userId);
        Task<ApiResponse<bool>> ProcessBulkPayrollAsync(ProcessPayrollRequest request, int userId);
        Task<ApiResponse<EmployeePayrollDetailResponse>> GetEmployeePayrollDetailsAsync(int cycleId, int employeeId);
        Task<ApiResponse<List<EmployeePayrollDetailResponse>>> GetPayrollRegisterAsync(int cycleId);

        // Payroll Actions
        Task<ApiResponse<bool>> RecalculatePayrollAsync(int processingId, int userId);
        Task<ApiResponse<bool>> HoldPayrollAsync(int processingId, string reason, int userId);
        Task<ApiResponse<bool>> ReleasePayrollAsync(int processingId, int userId);
        Task<ApiResponse<bool>> MarkPayrollAsPaidAsync(int processingId, string paymentMode, string? referenceNo, int userId);

        // Reports
        Task<ApiResponse<PayrollRegisterResponse>> GetPayrollRegisterReportAsync(int cycleId);
        Task<ApiResponse<BankTransferResponse>> GetBankTransferReportAsync(int cycleId);
        Task<ApiResponse<StatutoryComplianceResponse>> GetStatutoryComplianceReportAsync(int month, int year);
        Task<ApiResponse<DepartmentWiseSalaryResponse>> GetDepartmentWiseSalaryReportAsync(int cycleId);

        // Dashboard
        Task<ApiResponse<PayrollDashboardResponse>> GetPayrollDashboardAsync();

        // Arrears
        Task<ApiResponse<bool>> CalculateArrearsAsync(int employeeId, int newStructureId, DateTime revisionDate, int userId);
        Task<ApiResponse<List<PayrollArrears>>> GetPendingArrearsAsync(int? employeeId = null);
   
    }
}