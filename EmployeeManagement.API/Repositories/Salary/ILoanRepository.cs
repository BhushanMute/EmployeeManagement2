using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
 

namespace EmployeeManagement.API.Salary    
{
    /// <summary>
    /// Interface for Loan Repository
    /// कर्ज रिपॉझिटरी इंटरफेस
    /// </summary>
    public interface ILoanRepository
    {
        // Loan Types
        Task<List<LoanType>> GetAllLoanTypesAsync(bool activeOnly = true);
        Task<LoanType?> GetLoanTypeByIdAsync(int loanTypeId);

        // Loan Application
        Task<int> CreateLoanApplicationAsync(LoanApplicationRequest request, int appliedBy);
        Task<EmployeeLoan?> GetLoanByIdAsync(int loanId);
        Task<List<EmployeeLoan>> GetEmployeeLoansAsync(int employeeId, string? status = null);
        Task<List<EmployeeLoan>> GetPendingLoanApprovalsAsync();

        // Loan Approval
        Task<bool> ApproveLoanAsync(ApproveLoanRequest request, int approvedBy);
        Task<bool> RejectLoanAsync(int loanId, string reason, int rejectedBy);

        // Loan Disbursement
        Task<bool> DisburseLoanAsync(int loanId, DateTime disbursementDate, string mode, string? referenceNo, int disbursedBy);

        // EMI Schedule
        Task<bool> GenerateEMIScheduleAsync(int loanId);
        Task<List<LoanEMISchedule>> GetLoanEMIScheduleAsync(int loanId);
        Task<LoanEMISchedule?> GetNextPendingEMIAsync(int loanId);
        Task<bool> ProcessEMIDeductionAsync(int loanId, int cycleId);
        Task<bool> MarkEMIPaidAsync(int emiId, int cycleId);

        // Loan Prepayment & Closure
        Task<bool> PrepayLoanAsync(PrepayLoanRequest request, int userId);
        Task<bool> CloseLoanAsync(int loanId, int closedBy);

        // Loan Summary
        Task<LoanSummaryResponse> GetLoanSummaryAsync(int employeeId);
        Task<LoanDetailsResponse> GetLoanDetailsAsync(int loanId);

        // Outstanding Loans
        Task<decimal> GetTotalLoanOutstandingAsync(int employeeId);
        Task<decimal> GetMonthlyEMIDeductionAsync(int employeeId);
    }
}