using Dapper;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.API.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Salary
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<PayrollRepository> _logger;

        public PayrollRepository(IDbConnectionFactory connectionFactory, ILogger<PayrollRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        #region Payroll Cycle Management

        /// <summary>
        /// Get payroll cycle by ID
        /// </summary>
        public async Task<PayrollCycle?> GetPayrollCycleByIdAsync(int cycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<PayrollCycle>(
                    "sp_GetPayrollCycleById",
                    new { CycleId = cycleId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll cycle by ID: {CycleId}", cycleId);
                throw;
            }
        }

        /// <summary>
        /// Get payroll cycle by month and year
        /// </summary>
        public async Task<PayrollCycle?> GetPayrollCycleByMonthYearAsync(int month, int year)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<PayrollCycle>(
                    "sp_GetPayrollCycleByMonthYear",
                    new { Month = month, Year = year },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll cycle for {Month}/{Year}", month, year);
                throw;
            }
        }

        /// <summary>
        /// Get all payroll cycles for a year
        /// </summary>
        public async Task<List<PayrollCycle>> GetPayrollCyclesAsync(int year, string? status = null)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var cycles = await connection.QueryAsync<PayrollCycle>(
                    "sp_GetPayrollCycles",
                    new { Year = year, Status = status },
                    commandType: CommandType.StoredProcedure);

                return cycles.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll cycles for year: {Year}", year);
                throw;
            }
        }

        /// <summary>
        /// Create new payroll cycle
        /// </summary>
        public async Task<int> CreatePayrollCycleAsync(PayrollCycle cycle, int createdBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@CycleName", cycle.CycleName);
                parameters.Add("@CycleCode", cycle.CycleCode);
                parameters.Add("@PeriodType", cycle.PeriodType);
                parameters.Add("@StartDate", cycle.StartDate);
                parameters.Add("@EndDate", cycle.EndDate);
                parameters.Add("@FinancialYear", cycle.FinancialYear);
                parameters.Add("@Month", cycle.Month);
                parameters.Add("@Year", cycle.Year);
                parameters.Add("@Status", cycle.Status);
                parameters.Add("@CreatedBy", createdBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_CreatePayrollCycle",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Payroll cycle created: {CycleId} for {Month}/{Year}", result, cycle.Month, cycle.Year);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payroll cycle");
                throw;
            }
        }

        /// <summary>
        /// Update payroll cycle
        /// </summary>
        public async Task<bool> UpdatePayrollCycleAsync(PayrollCycle cycle, int updatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@CycleId", cycle.Id);
                parameters.Add("@CycleName", cycle.CycleName);
                parameters.Add("@CycleCode", cycle.CycleCode);
                parameters.Add("@PeriodType", cycle.PeriodType);
                parameters.Add("@StartDate", cycle.StartDate);
                parameters.Add("@EndDate", cycle.EndDate);
                parameters.Add("@FinancialYear", cycle.FinancialYear);
                parameters.Add("@Month", cycle.Month);
                parameters.Add("@Year", cycle.Year);
                parameters.Add("@Status", cycle.Status);
                parameters.Add("@SalaryCreditDate", cycle.SalaryCreditDate);
                parameters.Add("@UpdatedBy", updatedBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_UpdatePayrollCycle",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Payroll cycle updated: {CycleId}", cycle.Id);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payroll cycle: {CycleId}", cycle.Id);
                throw;
            }
        }

        /// <summary>
        /// Lock payroll cycle
        /// </summary>
        public async Task<bool> LockPayrollCycleAsync(int cycleId, int lockedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_LockPayrollCycle",
                    new { CycleId = cycleId, LockedBy = lockedBy },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Payroll cycle locked: {CycleId} by user: {LockedBy}", cycleId, lockedBy);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking payroll cycle: {CycleId}", cycleId);
                throw;
            }
        }

        /// <summary>
        /// Approve payroll cycle
        /// </summary>
        public async Task<bool> ApprovePayrollCycleAsync(int cycleId, int approvedBy, string? remarks)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ApprovePayrollCycle",
                    new { CycleId = cycleId, ApprovedBy = approvedBy, Remarks = remarks },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Payroll cycle approved: {CycleId} by user: {ApprovedBy}", cycleId, approvedBy);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving payroll cycle: {CycleId}", cycleId);
                throw;
            }
        }

        #endregion

        #region Payroll Processing

        /// <summary>
        /// Process payroll for single employee using stored procedure
        /// </summary>
        public async Task<int> ProcessEmployeePayrollAsync(ProcessSingleEmployeePayrollRequest request, int processedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@PayrollCycleId", request.PayrollCycleId);
                parameters.Add("@EmployeeId", request.EmployeeId);
                parameters.Add("@TotalWorkingDays", request.TotalWorkingDays);
                parameters.Add("@PresentDays", request.PresentDays);
                parameters.Add("@PaidLeaveDays", request.PaidLeaveDays);
                parameters.Add("@WeeklyOffDays", request.WeeklyOffDays);
                parameters.Add("@HolidayDays", request.HolidayDays);
                parameters.Add("@OvertimeHours", request.OvertimeHours);
                parameters.Add("@CalculatedBy", processedBy);

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "sp_CalculateEmployeePayroll",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                int payrollProcessingId = result?.PayrollProcessingId ?? 0;

                _logger.LogInformation("Payroll processed for Employee {EmployeeId}, PayrollId: {PayrollId}",
                    request.EmployeeId, payrollProcessingId);

                return payrollProcessingId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payroll for employee: {EmployeeId}", request.EmployeeId);
                throw;
            }
        }

        /// <summary>
        /// Process bulk payroll using stored procedure
        /// </summary>
        public async Task<bool> ProcessBulkPayrollAsync(int cycleId, List<EmployeeAttendanceData> attendanceData, int processedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@PayrollCycleId", cycleId);
                parameters.Add("@ProcessedBy", processedBy);

                await connection.ExecuteAsync(
                    "sp_ProcessBulkPayroll",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 300);

                _logger.LogInformation("Bulk payroll processed for cycle: {CycleId}", cycleId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bulk payroll for cycle: {CycleId}", cycleId);
                throw;
            }
        }

        /// <summary>
        /// Get payroll processing by ID
        /// </summary>
        public async Task<PayrollProcessing?> GetPayrollProcessingByIdAsync(int processingId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<PayrollProcessing>(
                    "sp_GetPayrollProcessingById",
                    new { ProcessingId = processingId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll processing: {ProcessingId}", processingId);
                throw;
            }
        }

        /// <summary>
        /// Get all payroll processing records for a cycle
        /// </summary>
        public async Task<List<PayrollProcessing>> GetPayrollProcessingByCycleAsync(int cycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var payrolls = await connection.QueryAsync<PayrollProcessing>(
                    "sp_GetPayrollProcessingByCycle",
                    new { CycleId = cycleId },
                    commandType: CommandType.StoredProcedure);

                return payrolls.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll processing for cycle: {CycleId}", cycleId);
                throw;
            }
        }

        /// <summary>
        /// Get employee payroll for specific cycle
        /// </summary>
        public async Task<PayrollProcessing?> GetEmployeePayrollAsync(int cycleId, int employeeId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<PayrollProcessing>(
                    "sp_GetEmployeePayroll",
                    new { CycleId = cycleId, EmployeeId = employeeId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee payroll for cycle: {CycleId}, employee: {EmployeeId}",
                    cycleId, employeeId);
                throw;
            }
        }

        /// <summary>
        /// Get payroll component details
        /// </summary>
        public async Task<List<PayrollProcessingDetail>> GetPayrollDetailsAsync(int processingId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var details = await connection.QueryAsync<PayrollProcessingDetail>(
                    "sp_GetPayrollDetails",
                    new { ProcessingId = processingId },
                    commandType: CommandType.StoredProcedure);

                return details.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll details: {ProcessingId}", processingId);
                throw;
            }
        }

        /// <summary>
        /// Get earnings breakdown
        /// </summary>
        public async Task<List<PayrollComponentResponse>> GetEarningsBreakdownAsync(int processingId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var earnings = await connection.QueryAsync<PayrollComponentResponse>(
                    "sp_GetEarningsBreakdown",
                    new { ProcessingId = processingId },
                    commandType: CommandType.StoredProcedure);

                return earnings.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting earnings breakdown: {ProcessingId}", processingId);
                throw;
            }
        }

        /// <summary>
        /// Get deductions breakdown
        /// </summary>
        public async Task<List<PayrollComponentResponse>> GetDeductionsBreakdownAsync(int processingId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var deductions = await connection.QueryAsync<PayrollComponentResponse>(
                    "sp_GetDeductionsBreakdown",
                    new { ProcessingId = processingId },
                    commandType: CommandType.StoredProcedure);

                return deductions.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting deductions breakdown: {ProcessingId}", processingId);
                throw;
            }
        }

        /// <summary>
        /// Recalculate payroll
        /// </summary>
        public async Task<bool> RecalculatePayrollAsync(int processingId, int recalculatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_RecalculatePayroll",
                    new { ProcessingId = processingId, RecalculatedBy = recalculatedBy },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Payroll recalculated: {ProcessingId} by user: {RecalculatedBy}", processingId, recalculatedBy);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating payroll: {ProcessingId}", processingId);
                throw;
            }
        }

        /// <summary>
        /// Hold payroll
        /// </summary>
        public async Task<bool> HoldPayrollAsync(int processingId, string reason, int userId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_HoldPayroll",
                    new { ProcessingId = processingId, Reason = reason, UserId = userId },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Payroll held: {ProcessingId}, Reason: {Reason}", processingId, reason);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error holding payroll: {ProcessingId}", processingId);
                throw;
            }
        }

        /// <summary>
        /// Release payroll hold
        /// </summary>
        public async Task<bool> ReleasePayrollHoldAsync(int processingId, int userId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ReleasePayrollHold",
                    new { ProcessingId = processingId, UserId = userId },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Payroll hold released: {ProcessingId}", processingId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing payroll hold: {ProcessingId}", processingId);
                throw;
            }
        }

        /// <summary>
        /// Update payment status
        /// </summary>
        public async Task<bool> UpdatePaymentStatusAsync(int processingId, string status, DateTime? paymentDate)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_UpdatePaymentStatus",
                    new { ProcessingId = processingId, Status = status, PaymentDate = paymentDate },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Payment status updated: {ProcessingId}, Status: {Status}", processingId, status);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status: {ProcessingId}", processingId);
                throw;
            }
        }

        #endregion

        #region Payroll Summary & Reports

        /// <summary>
        /// Get payroll summary for a cycle
        /// </summary>
        public async Task<PayrollSummaryResponse> GetPayrollSummaryAsync(int cycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<PayrollSummaryResponse>(
                    "sp_GetPayrollSummary",
                    new { CycleId = cycleId },
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    throw new Exception($"Payroll cycle not found: {cycleId}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll summary for cycle: {CycleId}", cycleId);
                throw;
            }
        }

        /// <summary>
        /// Get payroll register (detailed report)
        /// </summary>
        public async Task<List<EmployeePayrollDetailResponse>> GetPayrollRegisterAsync(int cycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var payrolls = await connection.QueryAsync<EmployeePayrollDetailResponse>(
                    "sp_GetPayrollRegister",
                    new { CycleId = cycleId },
                    commandType: CommandType.StoredProcedure);

                var payrollList = payrolls.ToList();

                // Get component breakdowns for each employee
                foreach (var payroll in payrollList)
                {
                    payroll.EarningsBreakdown = await GetEarningsBreakdownAsync(payroll.PayrollProcessingId);
                    payroll.DeductionsBreakdown = await GetDeductionsBreakdownAsync(payroll.PayrollProcessingId);
                }

                return payrollList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll register for cycle: {CycleId}", cycleId);
                throw;
            }
        }

        #endregion

        #region Dashboard

        /// <summary>
        /// Get payroll dashboard data
        /// </summary>
        public async Task<PayrollDashboardResponse> GetPayrollDashboardAsync()
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetPayrollDashboard",
                    new { Month = DateTime.Now.Month, Year = DateTime.Now.Year },
                    commandType: CommandType.StoredProcedure);

                var currentMonth = await multi.ReadFirstOrDefaultAsync<CurrentMonthSummary>();
                var statistics = await multi.ReadFirstOrDefaultAsync<PayrollStatistics>();
                var pendingActions = await multi.ReadFirstOrDefaultAsync<PendingActions>();

                var dashboard = new PayrollDashboardResponse
                {
                    CurrentMonth = currentMonth ?? new CurrentMonthSummary
                    {
                        Month = DateTime.Now.Month,
                        Year = DateTime.Now.Year,
                        MonthName = DateTime.Now.ToString("MMMM")
                    },
                    Statistics = statistics ?? new PayrollStatistics(),
                    PendingActions = pendingActions ?? new PendingActions(),
                    GeneratedDate = DateTime.Now
                };

                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll dashboard");
                throw;
            }
        }

        #endregion

        #region Payment Processing

        /// <summary>
        /// Mark payroll as paid
        /// </summary>
        public async Task<bool> MarkPayrollAsPaidAsync(int processingId, string paymentMode, string? referenceNo, int userId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_MarkPayrollAsPaid",
                    new
                    {
                        ProcessingId = processingId,
                        PaymentMode = paymentMode,
                        ReferenceNo = referenceNo,
                        UserId = userId
                    },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Payroll marked as paid: {ProcessingId}, Mode: {PaymentMode}, Ref: {ReferenceNo}",
                    processingId, paymentMode, referenceNo);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking payroll as paid: {ProcessingId}", processingId);
                throw;
            }
        }

        /// <summary>
        /// Process bulk payment
        /// </summary>
        public async Task<bool> ProcessBulkPaymentAsync(int cycleId, string paymentMode, string? referenceNo, int userId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ProcessBulkPayment",
                    new
                    {
                        CycleId = cycleId,
                        PaymentMode = paymentMode,
                        ReferenceNo = referenceNo,
                        UserId = userId
                    },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Bulk payment processed for cycle: {CycleId}", cycleId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bulk payment for cycle: {CycleId}", cycleId);
                throw;
            }
        }

        #endregion

        #region Arrears Management

        /// <summary>
        /// Calculate arrears using stored procedure
        /// </summary>
        public async Task<int> CalculateArrearsAsync(int employeeId, int newStructureId, DateTime revisionDate, int calculatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeId", employeeId);
                parameters.Add("@NewStructureId", newStructureId);
                parameters.Add("@RevisionEffectiveDate", revisionDate);
                parameters.Add("@CalculatedBy", calculatedBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_CalculateArrears",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Arrears calculated for employee: {EmployeeId}, ArrearsId: {ArrearsId}", employeeId, result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating arrears for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        /// <summary>
        /// Get pending arrears
        /// </summary>
        public async Task<List<PayrollArrears>> GetPendingArrearsAsync(int? employeeId = null)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var arrears = await connection.QueryAsync<PayrollArrears>(
                    "sp_GetPendingArrears",
                    new { EmployeeId = employeeId },
                    commandType: CommandType.StoredProcedure);

                return arrears.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending arrears");
                throw;
            }
        }

        /// <summary>
        /// Process arrears
        /// </summary>
        public async Task<bool> ProcessArrearsAsync(int arrearsId, int cycleId, int processedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ProcessArrears",
                    new { ArrearsId = arrearsId, CycleId = cycleId, ProcessedBy = processedBy },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Arrears processed: {ArrearsId} for cycle: {CycleId}", arrearsId, cycleId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing arrears: {ArrearsId}", arrearsId);
                throw;
            }
        }

        /// <summary>
        /// Get arrears by employee
        /// </summary>
        public async Task<List<PayrollArrears>> GetArrearsByEmployeeAsync(int employeeId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var arrears = await connection.QueryAsync<PayrollArrears>(
                    "sp_GetArrearsByEmployee",
                    new { EmployeeId = employeeId },
                    commandType: CommandType.StoredProcedure);

                return arrears.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting arrears for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        #endregion

        #region Bank File Generation

        /// <summary>
        /// Get bank file data for salary disbursement
        /// </summary>
        public async Task<List<BankFileData>> GetBankFileDataAsync(int cycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var data = await connection.QueryAsync<BankFileData>(
                    "sp_GetBankFileData",
                    new { CycleId = cycleId },
                    commandType: CommandType.StoredProcedure);

                return data.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bank file data for cycle: {CycleId}", cycleId);
                throw;
            }
        }

        /// <summary>
        /// Log bank file generation
        /// </summary>
        public async Task<int> LogBankFileGenerationAsync(int cycleId, string fileName, string fileType, int generatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_LogBankFileGeneration",
                    new
                    {
                        CycleId = cycleId,
                        FileName = fileName,
                        FileType = fileType,
                        GeneratedBy = generatedBy
                    },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Bank file generation logged for cycle: {CycleId}, File: {FileName}", cycleId, fileName);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging bank file generation for cycle: {CycleId}", cycleId);
                throw;
            }
        }

        #endregion

        #region Statutory Reports

        /// <summary>
        /// Get PF report data
        /// </summary>
        public async Task<List<PFReportData>> GetPFReportDataAsync(int cycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var data = await connection.QueryAsync<PFReportData>(
                    "sp_GetPFReportData",
                    new { CycleId = cycleId },
                    commandType: CommandType.StoredProcedure);

                return data.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PF report data for cycle: {CycleId}", cycleId);
                throw;
            }
        }

        /// <summary>
        /// Get ESI report data
        /// </summary>
        public async Task<List<ESIReportData>> GetESIReportDataAsync(int cycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var data = await connection.QueryAsync<ESIReportData>(
                    "sp_GetESIReportData",
                    new { CycleId = cycleId },
                    commandType: CommandType.StoredProcedure);

                return data.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ESI report data for cycle: {CycleId}", cycleId);
                throw;
            }
        }

        /// <summary>
        /// Get PT report data
        /// </summary>
        public async Task<List<PTReportData>> GetPTReportDataAsync(int cycleId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var data = await connection.QueryAsync<PTReportData>(
                    "sp_GetPTReportData",
                    new { CycleId = cycleId },
                    commandType: CommandType.StoredProcedure);

                return data.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PT report data for cycle: {CycleId}", cycleId);
                throw;
            }
        }

        #endregion

        #region Payslip Generation

        /// <summary>
        /// Get payslip data
        /// </summary>
        public async Task<PayslipData?> GetPayslipDataAsync(int processingId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetPayslipData",
                    new { ProcessingId = processingId },
                    commandType: CommandType.StoredProcedure);

                var payslip = await multi.ReadFirstOrDefaultAsync<PayslipData>();
                if (payslip != null)
                {
                    payslip.Earnings = (await multi.ReadAsync<PayrollComponentResponse>()).ToList();
                    payslip.Deductions = (await multi.ReadAsync<PayrollComponentResponse>()).ToList();
                }

                return payslip;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payslip data: {ProcessingId}", processingId);
                throw;
            }
        }

        /// <summary>
        /// Log payslip generation
        /// </summary>
        public async Task<bool> LogPayslipGenerationAsync(int processingId, int generatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.ExecuteAsync(
                    "sp_LogPayslipGeneration",
                    new { ProcessingId = processingId, GeneratedBy = generatedBy },
                    commandType: CommandType.StoredProcedure);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging payslip generation: {ProcessingId}", processingId);
                throw;
            }
        }

        #endregion

        #region Payroll History

        /// <summary>
        /// Get employee payroll history
        /// </summary>
        public async Task<List<PayrollProcessing>> GetEmployeePayrollHistoryAsync(int employeeId, int? year = null)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var history = await connection.QueryAsync<PayrollProcessing>(
                    "sp_GetEmployeePayrollHistory",
                    new { EmployeeId = employeeId, Year = year },
                    commandType: CommandType.StoredProcedure);

                return history.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payroll history for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        /// <summary>
        /// Get YTD summary for employee
        /// </summary>
        public async Task<YTDSummary?> GetEmployeeYTDSummaryAsync(int employeeId, int financialYear)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<YTDSummary>(
                    "sp_GetEmployeeYTDSummary",
                    new { EmployeeId = employeeId, FinancialYear = financialYear },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting YTD summary for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        #endregion
    }
}