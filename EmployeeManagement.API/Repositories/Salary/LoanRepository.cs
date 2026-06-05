using Dapper;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.API.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Salary
{
    /// <summary>
    /// Loan Repository Implementation
    /// कर्ज रिपॉझिटरी (Dapper + Stored Procedures)
    /// </summary>
    public class LoanRepository : ILoanRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<LoanRepository> _logger;

        public LoanRepository(IDbConnectionFactory connectionFactory, ILogger<LoanRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        #region Loan Types

        /// <summary>
        /// Get all loan types
        /// </summary>
        public async Task<List<LoanType>> GetAllLoanTypesAsync(bool activeOnly = true)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var loanTypes = await connection.QueryAsync<LoanType>(
                    "sp_GetAllLoanTypes",
                    new { ActiveOnly = activeOnly },
                    commandType: CommandType.StoredProcedure);

                return loanTypes.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all loan types");
                throw;
            }
        }

        /// <summary>
        /// Get loan type by ID
        /// </summary>
        public async Task<LoanType?> GetLoanTypeByIdAsync(int loanTypeId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<LoanType>(
                    "sp_GetLoanTypeById",
                    new { LoanTypeId = loanTypeId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan type: {LoanTypeId}", loanTypeId);
                throw;
            }
        }

        /// <summary>
        /// Create loan type
        /// </summary>
        public async Task<int> CreateLoanTypeAsync(LoanType loanType, int createdBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@LoanTypeCode", loanType.LoanTypeCode);
                parameters.Add("@LoanTypeName", loanType.LoanTypeName);
                parameters.Add("@Description", loanType.Description);
                parameters.Add("@InterestRate", loanType.InterestRate);
                parameters.Add("@MaxAmount", loanType.MaxAmount);
                parameters.Add("@MinAmount", loanType.MinAmount);
                parameters.Add("@MaxTenureMonths", loanType.MaxTenureMonths);
                parameters.Add("@MinTenureMonths", loanType.MinTenureMonths);
                parameters.Add("@RequiresGuarantor", loanType.RequiresGuarantor);
                parameters.Add("@RequiresCollateral", loanType.RequiresCollateral);
                parameters.Add("@MaxLoanMultiplier", loanType.MaxLoanMultiplier);
                parameters.Add("@ProcessingFeePercent", loanType.ProcessingFeePercent);
                parameters.Add("@DisplayOrder", loanType.DisplayOrder);
                parameters.Add("@CreatedBy", createdBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_CreateLoanType",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Loan type created: {LoanTypeId}", result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating loan type");
                throw;
            }
        }

        /// <summary>
        /// Update loan type
        /// </summary>
        public async Task<bool> UpdateLoanTypeAsync(LoanType loanType, int updatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@LoanTypeId", loanType.Id);
                parameters.Add("@LoanTypeName", loanType.LoanTypeName);
                parameters.Add("@Description", loanType.Description);
                parameters.Add("@InterestRate", loanType.InterestRate);
                parameters.Add("@MaxAmount", loanType.MaxAmount);
                parameters.Add("@MinAmount", loanType.MinAmount);
                parameters.Add("@MaxTenureMonths", loanType.MaxTenureMonths);
                parameters.Add("@MinTenureMonths", loanType.MinTenureMonths);
                parameters.Add("@RequiresGuarantor", loanType.RequiresGuarantor);
                parameters.Add("@RequiresCollateral", loanType.RequiresCollateral);
                parameters.Add("@MaxLoanMultiplier", loanType.MaxLoanMultiplier);
                parameters.Add("@ProcessingFeePercent", loanType.ProcessingFeePercent);
                parameters.Add("@DisplayOrder", loanType.DisplayOrder);
                parameters.Add("@IsActive", loanType.IsActive);
                parameters.Add("@UpdatedBy", updatedBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_UpdateLoanType",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating loan type: {LoanTypeId}", loanType.Id);
                throw;
            }
        }

        #endregion

        #region Loan Application

        /// <summary>
        /// Create loan application
        /// कर्ज अर्ज तयार करा
        /// </summary>
        public async Task<int> CreateLoanApplicationAsync(LoanApplicationRequest request, int appliedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeId", request.EmployeeId);
                parameters.Add("@LoanTypeId", request.LoanTypeId);
                parameters.Add("@RequestedAmount", request.RequestedAmount);
                parameters.Add("@TenureMonths", request.TenureMonths);
                parameters.Add("@Purpose", request.Purpose);
                parameters.Add("@GuarantorEmployeeId", request.GuarantorEmployeeId);
                parameters.Add("@GuarantorName", request.GuarantorName);
                parameters.Add("@GuarantorRelation", request.GuarantorRelation);
                parameters.Add("@CreatedBy", appliedBy);

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "sp_CreateLoanApplication",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                int loanId = result?.LoanId ?? 0;

                _logger.LogInformation("Loan application created: {LoanId} for employee {EmployeeId}", loanId, request.EmployeeId);

                return loanId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating loan application for employee: {EmployeeId}", request.EmployeeId);
                throw;
            }
        }

        /// <summary>
        /// Get loan by ID
        /// </summary>
        public async Task<EmployeeLoan?> GetLoanByIdAsync(int loanId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<EmployeeLoan>(
                    "sp_GetLoanById",
                    new { LoanId = loanId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan: {LoanId}", loanId);
                throw;
            }
        }

        /// <summary>
        /// Get employee loans
        /// </summary>
        public async Task<List<EmployeeLoan>> GetEmployeeLoansAsync(int employeeId, string? status = null)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var loans = await connection.QueryAsync<EmployeeLoan>(
                    "sp_GetEmployeeLoans",
                    new { EmployeeId = employeeId, Status = status },
                    commandType: CommandType.StoredProcedure);

                return loans.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee loans: {EmployeeId}", employeeId);
                throw;
            }
        }

        /// <summary>
        /// Get pending loan approvals
        /// </summary>
        public async Task<List<EmployeeLoan>> GetPendingLoanApprovalsAsync()
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var loans = await connection.QueryAsync<EmployeeLoan>(
                    "sp_GetPendingLoanApprovals",
                    commandType: CommandType.StoredProcedure);

                return loans.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending loan approvals");
                throw;
            }
        }

        /// <summary>
        /// Get all loans (admin view)
        /// </summary>
        public async Task<List<EmployeeLoan>> GetAllLoansAsync(string? status = null, int? loanTypeId = null, int? departmentId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var loans = await connection.QueryAsync<EmployeeLoan>(
                    "sp_GetAllLoans",
                    new { Status = status, LoanTypeId = loanTypeId, DepartmentId = departmentId, FromDate = fromDate, ToDate = toDate },
                    commandType: CommandType.StoredProcedure);

                return loans.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all loans");
                throw;
            }
        }

        #endregion

        #region Loan Approval

        /// <summary>
        /// Approve loan
        /// कर्ज मंजूर करा
        /// </summary>
        public async Task<bool> ApproveLoanAsync(ApproveLoanRequest request, int approvedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@LoanId", request.LoanId);
                parameters.Add("@ApprovedAmount", request.ApprovedAmount);
                parameters.Add("@ApprovedTenureMonths", request.ApprovedTenureMonths);
                parameters.Add("@Remarks", request.Remarks);
                parameters.Add("@ApprovedBy", approvedBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ApproveLoan",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (result > 0)
                {
                    _logger.LogInformation("Loan approved: {LoanId} by user {UserId}", request.LoanId, approvedBy);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving loan: {LoanId}", request.LoanId);
                throw;
            }
        }

        /// <summary>
        /// Reject loan
        /// </summary>
        public async Task<bool> RejectLoanAsync(int loanId, string reason, int rejectedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_RejectLoan",
                    new { LoanId = loanId, Reason = reason, RejectedBy = rejectedBy },
                    commandType: CommandType.StoredProcedure);

                if (result > 0)
                {
                    _logger.LogInformation("Loan rejected: {LoanId}", loanId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting loan: {LoanId}", loanId);
                throw;
            }
        }

        #endregion

        #region Loan Disbursement

        /// <summary>
        /// Disburse loan
        /// कर्ज वितरित करा
        /// </summary>
        public async Task<bool> DisburseLoanAsync(int loanId, DateTime disbursementDate, string mode, string? referenceNo, int disbursedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_DisburseLoan",
                    new
                    {
                        LoanId = loanId,
                        DisbursementDate = disbursementDate,
                        DisbursementMode = mode,
                        ReferenceNo = referenceNo,
                        DisbursedBy = disbursedBy
                    },
                    commandType: CommandType.StoredProcedure);

                if (result > 0)
                {
                    _logger.LogInformation("Loan disbursed: {LoanId}", loanId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disbursing loan: {LoanId}", loanId);
                throw;
            }
        }

        #endregion

        #region EMI Schedule

        /// <summary>
        /// Generate EMI schedule
        /// EMI शेड्यूल तयार करा
        /// </summary>
        public async Task<bool> GenerateEMIScheduleAsync(int loanId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_GenerateEMISchedule",
                    new { LoanId = loanId },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("EMI schedule generated for loan: {LoanId}", loanId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating EMI schedule for loan: {LoanId}", loanId);
                throw;
            }
        }

        /// <summary>
        /// Get loan EMI schedule
        /// </summary>
        public async Task<List<LoanEMISchedule>> GetLoanEMIScheduleAsync(int loanId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var schedule = await connection.QueryAsync<LoanEMISchedule>(
                    "sp_GetLoanEMISchedule",
                    new { LoanId = loanId },
                    commandType: CommandType.StoredProcedure);

                return schedule.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EMI schedule for loan: {LoanId}", loanId);
                throw;
            }
        }

        /// <summary>
        /// Get next pending EMI
        /// </summary>
        public async Task<LoanEMISchedule?> GetNextPendingEMIAsync(int loanId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<LoanEMISchedule>(
                    "sp_GetNextPendingEMI",
                    new { LoanId = loanId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next pending EMI for loan: {LoanId}", loanId);
                throw;
            }
        }

        /// <summary>
        /// Get pending EMIs for payroll
        /// </summary>
        public async Task<List<LoanEMISchedule>> GetPendingEMIsForPayrollAsync(int payrollCycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var emis = await connection.QueryAsync<LoanEMISchedule>(
                    "sp_GetPendingEMIsForPayroll",
                    new { PayrollCycleId = payrollCycleId },
                    commandType: CommandType.StoredProcedure);

                return emis.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending EMIs for payroll cycle: {CycleId}", payrollCycleId);
                throw;
            }
        }

        /// <summary>
        /// Process EMI deduction (called during payroll processing)
        /// </summary>
        public async Task<bool> ProcessEMIDeductionAsync(int loanId, int cycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "sp_ProcessLoanEMI",
                    new { PayrollCycleId = cycleId, LoanId = loanId },
                    commandType: CommandType.StoredProcedure);

                return result?.Success == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing EMI deduction for loan: {LoanId}", loanId);
                throw;
            }
        }

        /// <summary>
        /// Mark EMI as paid
        /// </summary>
        public async Task<bool> MarkEMIPaidAsync(int emiId, int cycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_MarkEMIPaid",
                    new { EMIId = emiId, CycleId = cycleId },
                    commandType: CommandType.StoredProcedure);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking EMI as paid: {EMIId}", emiId);
                throw;
            }
        }

        #endregion

        #region Loan Prepayment & Closure

        /// <summary>
        /// Prepay loan
        /// </summary>
        public async Task<bool> PrepayLoanAsync(PrepayLoanRequest request, int userId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@LoanId", request.LoanId);
                parameters.Add("@PrepaymentAmount", request.PrepaymentAmount);
                parameters.Add("@PrepaymentType", request.PrepaymentType);
                parameters.Add("@PaymentMode", request.PaymentMode);
                parameters.Add("@ReferenceNo", request.ReferenceNo);
                parameters.Add("@Remarks", request.Remarks);
                parameters.Add("@UserId", userId);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_PrepayLoan",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (result > 0)
                {
                    _logger.LogInformation("Loan prepayment processed: {LoanId}, Amount: {Amount}", request.LoanId, request.PrepaymentAmount);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing loan prepayment: {LoanId}", request.LoanId);
                throw;
            }
        }

        /// <summary>
        /// Close loan
        /// </summary>
        public async Task<bool> CloseLoanAsync(int loanId, int closedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_CloseLoan",
                    new { LoanId = loanId, ClosureType = "Manual", ClosedBy = closedBy },
                    commandType: CommandType.StoredProcedure);

                if (result > 0)
                {
                    _logger.LogInformation("Loan closed: {LoanId}", loanId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing loan: {LoanId}", loanId);
                throw;
            }
        }

        #endregion

        #region Loan Summary & Reports

        /// <summary>
        /// Get loan details with schedule
        /// संपूर्ण कर्ज तपशील मिळवा
        /// </summary>
        public async Task<LoanDetailsResponse> GetLoanDetailsAsync(int loanId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetLoanDetails",
                    new { LoanId = loanId },
                    commandType: CommandType.StoredProcedure);

                // ✅ Strong typing instead of dynamic
                var loan = await multi.ReadFirstOrDefaultAsync<LoanDetailsDbModel>();
                var emiSchedule = (await multi.ReadAsync<LoanEMISchedule>()).ToList();
                var prepayments = (await multi.ReadAsync<LoanPrepayment>()).ToList();

                if (loan == null)
                    throw new Exception($"Loan not found: {loanId}");

                var totalEmisPaid = loan.TotalEMIsPaid ?? 0;
                var tenure = loan.TenureMonths ?? 0;

                var response = new LoanDetailsResponse
                {
                    LoanId = loan.LoanId, // ✅ FIXED
                    LoanNumber = loan.LoanNumber,
                    EmployeeId = loan.EmployeeId,
                    EmployeeName = loan.EmployeeName,
                    EmployeeCode = loan.EmployeeCode,
                    LoanTypeId = loan.LoanTypeId,
                    LoanTypeName = loan.LoanTypeName,
                    DepartmentName = loan.DepartmentName,

                    LoanAmount = loan.LoanAmount,
                    InterestRate = loan.InterestRate,
                    TenureMonths = tenure,
                    EMIAmount = loan.EMIAmount,
                    TotalRepayableAmount = loan.TotalRepayableAmount,

                    ApplicationDate = loan.ApplicationDate,
                    RequestedAmount = loan.RequestedAmount,
                    ApprovedAmount = loan.ApprovedAmount,
                    Purpose = loan.Purpose,

                    Status = loan.Status,
                    ApprovedDate = loan.ApprovedDate,
                    ApprovedByName = loan.ApprovedByName,
                    DisbursementDate = loan.DisbursementDate,
                    DisbursedByName = loan.DisbursedByName,

                    FirstEMIDate = loan.FirstEMIDate,
                    LastEMIDate = loan.LastEMIDate,

                    TotalEMIs = tenure,
                    TotalEMIsPaid = totalEmisPaid,
                    RemainingEMIs = tenure - totalEmisPaid,

                    TotalAmountPaid = loan.TotalAmountPaid,
                    PrincipalPaid = loan.PrincipalPaid,
                    InterestPaid = loan.InterestPaid,

                    OutstandingAmount = loan.OutstandingAmount,
                    OutstandingPrincipal = loan.OutstandingPrincipal,
                    OutstandingInterest = loan.OutstandingInterest,

                    GuarantorName = loan.GuarantorName,
                    GuarantorRelation = loan.GuarantorRelation,
                    GuarantorEmployeeName = loan.GuarantorEmployeeName,

                    IsFullyPaid = loan.IsFullyPaid,
                    ClosureDate = loan.ClosureDate,
                    CreatedDate = loan.CreatedDate,

                    EMISchedule = emiSchedule.Select(e => new LoanEMIResponse
                    {
                        EMINumber = e.EMINumber,
                        EMIDueDate = e.EMIDueDate,
                        EMIAmount = e.EMIAmount,
                        PrincipalAmount = e.PrincipalAmount,
                        InterestAmount = e.InterestAmount,
                        OpeningBalance = e.OpeningBalance,
                        ClosingBalance = e.ClosingBalance,
                        Status = e.Status,
                        PaymentDate = e.PaymentDate,
                        AmountPaid = e.AmountPaid,
                        IsLatePayment = e.IsLatePayment,
                        LateFee = e.LateFee
                    }).ToList(),

                    Prepayments = prepayments ?? new List<LoanPrepayment>()
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan details: {LoanId}", loanId);
                throw;
            }
        }
        /// <summary>
        /// Get loan summary for employee
        /// </summary>
        public async Task<LoanSummaryResponse> GetLoanSummaryAsync(int employeeId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var summary = await connection.QueryFirstOrDefaultAsync<LoanSummaryResponse>(
                    "sp_GetLoanSummary",
                    new { EmployeeId = employeeId },
                    commandType: CommandType.StoredProcedure);

                if (summary == null)
                {
                    summary = new LoanSummaryResponse();
                }

                // Get detailed loan list
                var loans = await GetEmployeeLoansAsync(employeeId);
                summary.Loans = new List<LoanDetailsResponse>();

                foreach (var loan in loans)
                {
                    var details = await GetLoanDetailsAsync(loan.Id);
                    summary.Loans.Add(details);
                }

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan summary for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        /// <summary>
        /// Get total loan outstanding for employee
        /// </summary>
        public async Task<decimal> GetTotalLoanOutstandingAsync(int employeeId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "sp_GetTotalLoanOutstanding",
                    new { EmployeeId = employeeId },
                    commandType: CommandType.StoredProcedure);

                return result?.TotalOutstanding ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total loan outstanding for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        /// <summary>
        /// Get monthly EMI deduction amount
        /// </summary>
        public async Task<decimal> GetMonthlyEMIDeductionAsync(int employeeId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "sp_GetMonthlyEMIDeduction",
                    new { EmployeeId = employeeId },
                    commandType: CommandType.StoredProcedure);

                return result?.MonthlyEMI ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly EMI deduction for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        #endregion

        #region Dashboard & Reports

        /// <summary>
        /// Get loan dashboard stats
        /// </summary>
        public async Task<LoanDashboardStats> GetLoanDashboardStatsAsync()
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<LoanDashboardStats>(
                    "sp_GetLoanDashboardStats",
                    commandType: CommandType.StoredProcedure) ?? new LoanDashboardStats();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan dashboard stats");
                throw;
            }
        }

        /// <summary>
        /// Get overdue EMIs
        /// </summary>
        public async Task<List<OverdueEMI>> GetOverdueEMIsAsync()
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var emis = await connection.QueryAsync<OverdueEMI>(
                    "sp_GetOverdueEMIs",
                    commandType: CommandType.StoredProcedure);

                return emis.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overdue EMIs");
                throw;
            }
        }

        /// <summary>
        /// Check loan eligibility
        /// </summary>
        public async Task<LoanEligibilityResponse> CheckLoanEligibilityAsync(int employeeId, int loanTypeId, decimal requestedAmount)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<LoanEligibilityResponse>(
                    "sp_CheckLoanEligibility",
                    new { EmployeeId = employeeId, LoanTypeId = loanTypeId, RequestedAmount = requestedAmount },
                    commandType: CommandType.StoredProcedure) ?? new LoanEligibilityResponse { IsEligible = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking loan eligibility for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        #endregion
    }
}