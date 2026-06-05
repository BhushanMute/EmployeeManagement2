using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models.Payroll;

namespace EmployeeManagement.API.Services
{
    /// <summary>
    /// Interface for Loan Service
    /// कर्ज सेवा इंटरफेस
    /// </summary>
    public interface ILoanService
    {
        // Loan Types
        Task<ApiResponse<List<LoanType>>> GetAllLoanTypesAsync(bool activeOnly = true);
        Task<ApiResponse<LoanType>> GetLoanTypeByIdAsync(int loanTypeId);

        // Loan Application
        Task<ApiResponse<int>> ApplyForLoanAsync(LoanApplicationRequest request, int userId);
        Task<ApiResponse<LoanDetailsResponse>> GetLoanDetailsAsync(int loanId);
        Task<ApiResponse<List<LoanDetailsResponse>>> GetEmployeeLoansAsync(int employeeId, string? status = null);
        Task<ApiResponse<List<LoanDetailsResponse>>> GetPendingLoanApprovalsAsync();

        // Loan Approval
        Task<ApiResponse<bool>> ApproveLoanAsync(ApproveLoanRequest request, int userId);
        Task<ApiResponse<bool>> RejectLoanAsync(int loanId, string reason, int userId);

        // Loan Disbursement
        Task<ApiResponse<bool>> DisburseLoanAsync(int loanId, DateTime disbursementDate, string mode, string? referenceNo, int userId);

        // EMI Management
        Task<ApiResponse<List<LoanEMIResponse>>> GetLoanEMIScheduleAsync(int loanId);
        Task<ApiResponse<decimal>> GetMonthlyEMIDeductionAsync(int employeeId);

        // Loan Summary
        Task<ApiResponse<LoanSummaryResponse>> GetLoanSummaryAsync(int employeeId);
        Task<ApiResponse<decimal>> GetTotalLoanOutstandingAsync(int employeeId);
    }
}