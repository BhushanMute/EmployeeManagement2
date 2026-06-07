using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
 
using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.API.Salary;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services.Interfaces;

namespace EmployeeManagement.API.Services
{
    /// <summary>
    /// Loan Service Implementation
    /// कर्ज सेवा अंमलबजावणी
    /// </summary>
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<LoanService> _logger;

        public LoanService(
            ILoanRepository loanRepository,
            IAuditService auditService,
            ILogger<LoanService> logger)
        {
            _loanRepository = loanRepository;
            _auditService = auditService;
            _logger = logger;
        }

        #region Loan Types

        /// <summary>
        /// Get all loan types
        /// सर्व कर्ज प्रकार मिळवा
        /// </summary>
        public async Task<ApiResponse<List<LoanType>>> GetAllLoanTypesAsync(bool activeOnly = true)
        {
            try
            {
                var loanTypes = await _loanRepository.GetAllLoanTypesAsync(activeOnly);

                return ApiResponse<List<LoanType>>.Success(loanTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all loan types");
                return ApiResponse<List<LoanType>>.Fail("An error occurred while fetching loan types");
            }
        }

        /// <summary>
        /// Get loan type by ID
        /// </summary>
        public async Task<ApiResponse<LoanType>> GetLoanTypeByIdAsync(int loanTypeId)
        {
            try
            {
                var loanType = await _loanRepository.GetLoanTypeByIdAsync(loanTypeId);

                if (loanType == null)
                {
                    return ApiResponse<LoanType>.Fail("Loan type not found");
                }

                return ApiResponse<LoanType>.Success(loanType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan type: {LoanTypeId}", loanTypeId);
                return ApiResponse<LoanType>.Fail("An error occurred while fetching loan type");
            }
        }

        #endregion

        #region Loan Application

        /// <summary>
        /// Apply for loan
        /// कर्जासाठी अर्ज करा
        /// </summary>
        public async Task<ApiResponse<int>> ApplyForLoanAsync(LoanApplicationRequest request, int userId)
        {
            try
            {
                // Validate loan type
                var loanType = await _loanRepository.GetLoanTypeByIdAsync(request.LoanTypeId);
                if (loanType == null)
                {
                    return ApiResponse<int>.Fail("Loan type not found");
                }

                // Validate amount
                if (request.RequestedAmount < loanType.MinAmount || request.RequestedAmount > loanType.MaxAmount)
                {
                    return ApiResponse<int>.Fail($"Loan amount must be between {loanType.MinAmount:N0} and {loanType.MaxAmount:N0}");
                }

                // Validate tenure
                if (request.TenureMonths < 1 || request.TenureMonths > loanType.MaxTenureMonths)
                {
                    return ApiResponse<int>.Fail($"Tenure must be between 1 and {loanType.MaxTenureMonths} months");
                }

                // Check if employee has any pending loan applications
                var existingLoans = await _loanRepository.GetEmployeeLoansAsync(request.EmployeeId, "Pending");
                if (existingLoans.Any())
                {
                    return ApiResponse<int>.Fail("You already have a pending loan application");
                }

                // Check total outstanding loans
                var totalOutstanding = await _loanRepository.GetTotalLoanOutstandingAsync(request.EmployeeId);
                if (totalOutstanding + request.RequestedAmount > 500000) // Example: Max total loan limit
                {
                    return ApiResponse<int>.Fail("Total loan outstanding limit exceeded");
                }

                // Create loan application
                var loanId = await _loanRepository.CreateLoanApplicationAsync(request, userId);

                if (loanId > 0)
                {
                    // Audit log
            //        await _auditService.LogAsync(
            //    userId,
            //    "APPLY_LOAN",
            //    "EmployeeLoan",
            //    loanId,
            //    null,
            //    $"Amount: {request.RequestedAmount}, Tenure: {request.TenureMonths} months"
            //);

                    _logger.LogInformation("Loan application created: {LoanId} for employee {EmployeeId}, Amount: {Amount}",
                        loanId, request.EmployeeId, request.RequestedAmount);
                }

                return ApiResponse<int>.Success(loanId, "Loan application submitted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying for loan");
                return ApiResponse<int>.Fail("An error occurred while submitting loan application");
            }
        }

        /// <summary>
        /// Get loan details
        /// कर्ज तपशील मिळवा
        /// </summary>
        public async Task<ApiResponse<LoanDetailsResponse>> GetLoanDetailsAsync(int loanId)
        {
            try
            {
                var loanDetails = await _loanRepository.GetLoanDetailsAsync(loanId);

                return ApiResponse<LoanDetailsResponse>.Success(loanDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan details: {LoanId}", loanId);
                return ApiResponse<LoanDetailsResponse>.Fail("An error occurred while fetching loan details");
            }
        }

        /// <summary>
        /// Get employee loans
        /// </summary>
        public async Task<ApiResponse<List<LoanDetailsResponse>>> GetEmployeeLoansAsync(int employeeId, string? status = null)
        {
            try
            {
                var loans = await _loanRepository.GetEmployeeLoansAsync(employeeId, status);

                var loanDetails = new List<LoanDetailsResponse>();
                foreach (var loan in loans)
                {
                    var details = await _loanRepository.GetLoanDetailsAsync(loan.Id);
                    loanDetails.Add(details);
                }

                return ApiResponse<List<LoanDetailsResponse>>.Success(loanDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee loans: {EmployeeId}", employeeId);
                return ApiResponse<List<LoanDetailsResponse>>.Fail("An error occurred while fetching employee loans");
            }
        }

        /// <summary>
        /// Get pending loan approvals
        /// </summary>
        public async Task<ApiResponse<List<LoanDetailsResponse>>> GetPendingLoanApprovalsAsync()
        {
            try
            {
                var pendingLoans = await _loanRepository.GetPendingLoanApprovalsAsync();

                var loanDetails = new List<LoanDetailsResponse>();
                foreach (var loan in pendingLoans)
                {
                    var details = await _loanRepository.GetLoanDetailsAsync(loan.Id);
                    loanDetails.Add(details);
                }

                return ApiResponse<List<LoanDetailsResponse>>.Success(loanDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending loan approvals");
                return ApiResponse<List<LoanDetailsResponse>>.Fail("An error occurred while fetching pending loan approvals");
            }
        }

        #endregion

        #region Loan Approval

        /// <summary>
        /// Approve loan
        /// कर्ज मंजूर करा
        /// </summary>
        public async Task<ApiResponse<bool>> ApproveLoanAsync(ApproveLoanRequest request, int userId)
        {
            try
            {
                // Get loan details
                var loan = await _loanRepository.GetLoanByIdAsync(request.LoanId);
                if (loan == null)
                {
                    return ApiResponse<bool>.Fail("Loan not found");
                }

                if (loan.Status != "Pending")
                {
                    return ApiResponse<bool>.Fail($"Cannot approve loan with status: {loan.Status}");
                }

                if (!request.IsApproved)
                {
                    return ApiResponse<bool>.Fail("Please use reject endpoint to reject the loan");
                }

                // Validate approved amount
                if (request.ApprovedAmount > loan.RequestedAmount)
                {
                    return ApiResponse<bool>.Fail("Approved amount cannot be greater than requested amount");
                }

                if (request.ApprovedAmount <= 0)
                {
                    return ApiResponse<bool>.Fail("Approved amount must be greater than zero");
                }

                var result = await _loanRepository.ApproveLoanAsync(request, userId);

                if (result)
                {
                    //await _auditService.LogAsync(new AuditLog
                    //{
                    //    UserId = userId,
                    //    Action = "APPROVE_LOAN",
                    //    EntityName = "EmployeeLoan",
                    //    EntityId = request.LoanId,
                    //    NewValues = $"Approved Amount: {request.ApprovedAmount}, Remarks: {request.Remarks}"
                    //});

                    _logger.LogInformation("Loan approved: {LoanId} by user {UserId}", request.LoanId, userId);
                }

                return ApiResponse<bool>.Success(result, "Loan approved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving loan: {LoanId}", request.LoanId);
                return ApiResponse<bool>.Fail("An error occurred while approving loan");
            }
        }

        /// <summary>
        /// Reject loan
        /// कर्ज नाकारा
        /// </summary>
        public async Task<ApiResponse<bool>> RejectLoanAsync(int loanId, string reason, int userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    return ApiResponse<bool>.Fail("Rejection reason is required");
                }

                var loan = await _loanRepository.GetLoanByIdAsync(loanId);
                if (loan == null)
                {
                    return ApiResponse<bool>.Fail("Loan not found");
                }

                if (loan.Status != "Pending")
                {
                    return ApiResponse<bool>.Fail($"Cannot reject loan with status: {loan.Status}");
                }

                var result = await _loanRepository.RejectLoanAsync(loanId, reason, userId);

                if (result)
                {
                    //await _auditService.LogAsync(new AuditLog
                    //{
                    //    UserId = userId,
                    //    Action = "REJECT_LOAN",
                    //    EntityName = "EmployeeLoan",
                    //    EntityId = loanId,
                    //    NewValues = $"Rejection Reason: {reason}"
                    //});

                    _logger.LogInformation("Loan rejected: {LoanId} by user {UserId}", loanId, userId);
                }

                return ApiResponse<bool>.Success(result, "Loan rejected successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting loan: {LoanId}", loanId);
                return ApiResponse<bool>.Fail("An error occurred while rejecting loan");
            }
        }

        #endregion

        #region Loan Disbursement

        /// <summary>
        /// Disburse loan
        /// कर्ज वितरित करा
        /// </summary>
        public async Task<ApiResponse<bool>> DisburseLoanAsync(int loanId, DateTime disbursementDate, string mode, string? referenceNo, int userId)
        {
            try
            {
                var loan = await _loanRepository.GetLoanByIdAsync(loanId);
                if (loan == null)
                {
                    return ApiResponse<bool>.Fail("Loan not found");
                }

                if (loan.Status != "Approved")
                {
                    return ApiResponse<bool>.Fail($"Cannot disburse loan with status: {loan.Status}");
                }

                if (disbursementDate > DateTime.Now)
                {
                    return ApiResponse<bool>.Fail("Disbursement date cannot be in future");
                }

                var result = await _loanRepository.DisburseLoanAsync(loanId, disbursementDate, mode, referenceNo, userId);

                if (result)
                {
                    //await _auditService.LogAsync(new AuditLog
                    //{
                    //    UserId = userId,
                    //    Action = "DISBURSE_LOAN",
                    //    EntityName = "EmployeeLoan",
                    //    EntityId = loanId,
                    //    NewValues = $"Date: {disbursementDate:yyyy-MM-dd}, Mode: {mode}, Reference: {referenceNo}"
                    //});

                    _logger.LogInformation("Loan disbursed: {LoanId} by user {UserId}", loanId, userId);
                }

                return ApiResponse<bool>.Success(result, "Loan disbursed successfully. EMI schedule has been generated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disbursing loan: {LoanId}", loanId);
                return ApiResponse<bool>.Fail("An error occurred while disbursing loan");
            }
        }

        #endregion

        #region EMI Management

        /// <summary>
        /// Get loan EMI schedule
        /// कर्ज EMI शेड्यूल मिळवा
        /// </summary>
        public async Task<ApiResponse<List<LoanEMIResponse>>> GetLoanEMIScheduleAsync(int loanId)
        {
            try
            {
                var loan = await _loanRepository.GetLoanByIdAsync(loanId);
                if (loan == null)
                {
                    return ApiResponse<List<LoanEMIResponse>>.Fail("Loan not found");
                }

                var emiSchedule = await _loanRepository.GetLoanEMIScheduleAsync(loanId);

                var emiResponses = emiSchedule.Select(emi => new LoanEMIResponse
                {
                    EMINumber = emi.EMINumber,
                    EMIDueDate = emi.EMIDueDate,
                    EMIAmount = emi.EMIAmount,
                    PrincipalAmount = emi.PrincipalAmount,
                    InterestAmount = emi.InterestAmount,
                    OpeningBalance = emi.OpeningBalance,
                    ClosingBalance = emi.ClosingBalance,
                    Status = emi.Status,
                    PaymentDate = emi.PaymentDate,
                    AmountPaid = emi.AmountPaid,
                    IsLatePayment = emi.IsLatePayment,
                    LateFee = emi.LateFee
                }).ToList();

                return ApiResponse<List<LoanEMIResponse>>.Success(emiResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EMI schedule for loan: {LoanId}", loanId);
                return ApiResponse<List<LoanEMIResponse>>.Fail("An error occurred while fetching EMI schedule");
            }
        }

        /// <summary>
        /// Get monthly EMI deduction amount
        /// </summary>
        public async Task<ApiResponse<decimal>> GetMonthlyEMIDeductionAsync(int employeeId)
        {
            try
            {
                var emiAmount = await _loanRepository.GetMonthlyEMIDeductionAsync(employeeId);

                return ApiResponse<decimal>.Success(emiAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly EMI deduction for employee: {EmployeeId}", employeeId);
                return ApiResponse<decimal>.Fail("An error occurred while fetching EMI deduction amount");
            }
        }

        #endregion

        #region Loan Summary

        /// <summary>
        /// Get loan summary for employee
        /// कर्मचाऱ्याचा कर्ज सारांश मिळवा
        /// </summary>
        public async Task<ApiResponse<LoanSummaryResponse>> GetLoanSummaryAsync(int employeeId)
        {
            try
            {
                var summary = await _loanRepository.GetLoanSummaryAsync(employeeId);

                return ApiResponse<LoanSummaryResponse>.Success(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan summary for employee: {EmployeeId}", employeeId);
                return ApiResponse<LoanSummaryResponse>.Fail("An error occurred while fetching loan summary");
            }
        }

        /// <summary>
        /// Get total loan outstanding
        /// </summary>
        public async Task<ApiResponse<decimal>> GetTotalLoanOutstandingAsync(int employeeId)
        {
            try
            {
                var outstanding = await _loanRepository.GetTotalLoanOutstandingAsync(employeeId);

                return ApiResponse<decimal>.Success(outstanding);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total loan outstanding for employee: {EmployeeId}", employeeId);
                return ApiResponse<decimal>.Fail("An error occurred while fetching loan outstanding");
            }
        }

        #endregion
    }
}