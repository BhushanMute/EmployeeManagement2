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
        #region Payroll Cycle Management

        Task<PayrollCycle?> GetPayrollCycleByIdAsync(int cycleId);
        Task<PayrollCycle?> GetPayrollCycleByMonthYearAsync(int month, int year);
        Task<List<PayrollCycle>> GetPayrollCyclesAsync(int year, string? status = null);
        Task<int> CreatePayrollCycleAsync(PayrollCycle cycle, int createdBy);
        Task<bool> UpdatePayrollCycleAsync(PayrollCycle cycle, int updatedBy);
        Task<bool> LockPayrollCycleAsync(int cycleId, int lockedBy);
        Task<bool> ApprovePayrollCycleAsync(int cycleId, int approvedBy, string? remarks);

        #endregion

        #region Payroll Processing

        Task<int> ProcessEmployeePayrollAsync(ProcessSingleEmployeePayrollRequest request, int processedBy);

        // Fix: Removed attendanceData param as SP handles DB fetch directly
        Task<bool> ProcessBulkPayrollAsync(int cycleId, int processedBy);

        Task<PayrollProcessing?> GetPayrollProcessingByIdAsync(int processingId);
        Task<List<PayrollProcessing>> GetPayrollProcessingByCycleAsync(int cycleId);
        Task<PayrollProcessing?> GetEmployeePayrollAsync(int cycleId, int employeeId);
        Task<bool> RecalculatePayrollAsync(int processingId, int recalculatedBy);
        Task<bool> HoldPayrollAsync(int processingId, string reason, int userId);
        Task<bool> ReleasePayrollHoldAsync(int processingId, int userId);
        Task<bool> UpdatePaymentStatusAsync(int processingId, string status, DateTime? paymentDate);

        #endregion

        #region Payroll Components Details

        Task<List<PayrollProcessingDetail>> GetPayrollDetailsAsync(int processingId);
        Task<List<PayrollComponentResponse>> GetEarningsBreakdownAsync(int processingId);
        Task<List<PayrollComponentResponse>> GetDeductionsBreakdownAsync(int processingId);

        #endregion

        #region Payroll Summary & Reports

        Task<PayrollSummaryResponse> GetPayrollSummaryAsync(int cycleId);
        Task<List<EmployeePayrollDetailResponse>> GetPayrollRegisterAsync(int cycleId);
        Task<PayrollDashboardResponse> GetPayrollDashboardAsync();

        #endregion

        #region Payment & Bank

        Task<bool> MarkPayrollAsPaidAsync(int processingId, string paymentMode, string? referenceNo, int userId);
        Task<bool> ProcessBulkPaymentAsync(int cycleId, string paymentMode, string? referenceNo, int userId);
        Task<List<BankFileData>> GetBankFileDataAsync(int cycleId);
        Task<int> LogBankFileGenerationAsync(int cycleId, string fileName, string fileType, int generatedBy);

        #endregion

        #region Arrears Management

        Task<int> CalculateArrearsAsync(int employeeId, int newStructureId, DateTime revisionDate, int calculatedBy);
        Task<List<PayrollArrears>> GetPendingArrearsAsync(int? employeeId = null);
        Task<bool> ProcessArrearsAsync(int arrearsId, int cycleId, int processedBy);
        Task<List<PayrollArrears>> GetArrearsByEmployeeAsync(int employeeId);

        #endregion

        #region Payslip / Salary Slip & Email

        Task<PayslipData?> GetPayslipDataAsync(int processingId);
        Task<bool> LogPayslipGenerationAsync(int processingId, int generatedBy);
        Task<List<PayrollProcessing>> GetEmployeePayrollHistoryAsync(int employeeId, int? year = null);
        Task<YTDSummary?> GetEmployeeYTDSummaryAsync(int employeeId, int financialYear);

        // Fixed Logic: Now properly inserts into Queue
        Task<int> InsertBulkEmailQueueAsync(int cycleId, List<int> processingIds);

        #endregion
    }
}