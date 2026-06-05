using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
 

namespace EmployeeManagement.API.Salary
{
    /// <summary>
    /// Interface for Payroll Repository
    /// वेतन प्रक्रिया रिपॉझिटरी इंटरफेस
    /// </summary>
    public interface IPayrollRepository
    {
        // Payroll Cycle Management
        Task<PayrollCycle?> GetPayrollCycleByIdAsync(int cycleId);
        Task<PayrollCycle?> GetPayrollCycleByMonthYearAsync(int month, int year);
        Task<List<PayrollCycle>> GetPayrollCyclesAsync(int year, string? status = null);
        Task<int> CreatePayrollCycleAsync(PayrollCycle cycle, int createdBy);
        Task<bool> UpdatePayrollCycleAsync(PayrollCycle cycle, int updatedBy);
        Task<bool> LockPayrollCycleAsync(int cycleId, int lockedBy);
        Task<bool> ApprovePayrollCycleAsync(int cycleId, int approvedBy, string? remarks);

        // Payroll Processing
        Task<int> ProcessEmployeePayrollAsync(ProcessSingleEmployeePayrollRequest request, int processedBy);
        Task<bool> ProcessBulkPayrollAsync(int cycleId, List<EmployeeAttendanceData> attendanceData, int processedBy);
        Task<PayrollProcessing?> GetPayrollProcessingByIdAsync(int processingId);
        Task<List<PayrollProcessing>> GetPayrollProcessingByCycleAsync(int cycleId);
        Task<PayrollProcessing?> GetEmployeePayrollAsync(int cycleId, int employeeId);
        Task<bool> RecalculatePayrollAsync(int processingId, int recalculatedBy);
        Task<bool> HoldPayrollAsync(int processingId, string reason, int userId);
        Task<bool> ReleasePayrollHoldAsync(int processingId, int userId);

        // Payroll Components Details
        Task<List<PayrollProcessingDetail>> GetPayrollDetailsAsync(int processingId);
        Task<List<PayrollComponentResponse>> GetEarningsBreakdownAsync(int processingId);
        Task<List<PayrollComponentResponse>> GetDeductionsBreakdownAsync(int processingId);

        // Payroll Summary & Reports
        Task<PayrollSummaryResponse> GetPayrollSummaryAsync(int cycleId);
        Task<List<EmployeePayrollDetailResponse>> GetPayrollRegisterAsync(int cycleId);
        Task<PayrollDashboardResponse> GetPayrollDashboardAsync();

        // Payment Processing
        Task<bool> MarkPayrollAsPaidAsync(int processingId, string paymentMode, string? referenceNo, int userId);
        Task<bool> UpdatePaymentStatusAsync(int processingId, string status, DateTime? paymentDate);

        // Arrears Management
        Task<int> CalculateArrearsAsync(int employeeId, int newStructureId, DateTime revisionDate, int calculatedBy);
        Task<List<PayrollArrears>> GetPendingArrearsAsync(int? employeeId = null);
        Task<bool> ProcessArrearsAsync(int arrearsId, int cycleId, int processedBy);
    }
}