using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.API.Salary;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EmployeeManagement.API.Services.Implementation
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<PayrollService> _logger;

        public PayrollService(
            IPayrollRepository payrollRepository,
            ILoanRepository loanRepository,
            IAuditService auditService,
            ILogger<PayrollService> logger)
        {
            _payrollRepository = payrollRepository;
            _loanRepository = loanRepository;
            _auditService = auditService;
            _logger = logger;
        }

        #region Payroll Cycle Management

        public async Task<ApiResponse<int>> CreatePayrollCycleAsync(int month, int year, int createdBy)
        {
            try
            {
                if (month < 1 || month > 12)
                    return ApiResponse<int>.Fail("Invalid month");

                if (year < 2020 || year > 2100)
                    return ApiResponse<int>.Fail("Invalid year");

                var existingCycle = await _payrollRepository.GetPayrollCycleByMonthYearAsync(month, year);
                if (existingCycle != null)
                    return ApiResponse<int>.Fail("Cycle already exists");

                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var cycle = new PayrollCycle
                {
                    CycleName = $"Payroll - {startDate:MMMM yyyy}",
                    CycleCode = $"{startDate:MMyyyy}",
                    StartDate = startDate,
                    EndDate = endDate,
                    Month = month,
                    Year = year,
                    Status = "Draft"
                };

                var cycleId = await _payrollRepository.CreatePayrollCycleAsync(cycle, createdBy);

                await _auditService.LogAsync(
                    createdBy,
                    "CREATE_PAYROLL_CYCLE",
                    "PayrollCycle",
                    cycleId,
                    null,
                    JsonSerializer.Serialize(cycle)
                );

                return ApiResponse<int>.Success(cycleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreatePayrollCycle failed");
                return ApiResponse<int>.Fail("Error");
            }
        }

        public async Task<ApiResponse<PayrollSummaryResponse>> GetPayrollSummaryAsync(int cycleId)
        {
            try
            {
                var summary = await _payrollRepository.GetPayrollSummaryAsync(cycleId);

                return summary == null
                    ? ApiResponse<PayrollSummaryResponse>.Fail("Not found")
                    : ApiResponse<PayrollSummaryResponse>.Success(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPayrollSummary failed");
                return ApiResponse<PayrollSummaryResponse>.Fail("Error");
            }
        }

        public async Task<ApiResponse<List<PayrollSummaryResponse>>> GetPayrollCyclesAsync(int year)
        {
            try
            {
                var cycles = await _payrollRepository.GetPayrollCyclesAsync(year);

                var list = new List<PayrollSummaryResponse>();
                foreach (var c in cycles)
                {
                    var s = await _payrollRepository.GetPayrollSummaryAsync(c.Id);
                    if (s != null) list.Add(s);
                }

                return ApiResponse<List<PayrollSummaryResponse>>.Success(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPayrollCycles failed");
                return ApiResponse<List<PayrollSummaryResponse>>.Fail("Error");
            }
        }

        public async Task<ApiResponse<bool>> LockPayrollCycleAsync(int cycleId, int userId)
        {
            try
            {
                var cycle = await _payrollRepository.GetPayrollCycleByIdAsync(cycleId);
                if (cycle == null)
                    return ApiResponse<bool>.Fail("Not found");

                var result = await _payrollRepository.LockPayrollCycleAsync(cycleId, userId);

                if (result)
                {
                    await _auditService.LogAsync(
                        userId,
                        "LOCK_PAYROLL_CYCLE",
                        "PayrollCycle",
                        cycleId,
                        JsonSerializer.Serialize(new { cycle.Status, cycle.IsLocked }),
                        JsonSerializer.Serialize(new { Status = "Locked", IsLocked = true })
                    );
                }

                return ApiResponse<bool>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LockPayrollCycle failed");
                return ApiResponse<bool>.Fail("Error");
            }
        }

        public async Task<ApiResponse<bool>> ApprovePayrollCycleAsync(int cycleId, int userId, string? remarks)
        {
            try
            {
                var cycle = await _payrollRepository.GetPayrollCycleByIdAsync(cycleId);
                if (cycle == null)
                    return ApiResponse<bool>.Fail("Not found");

                var result = await _payrollRepository.ApprovePayrollCycleAsync(cycleId, userId, remarks);

                if (result)
                {
                    await _auditService.LogAsync(
                        userId,
                        "APPROVE_PAYROLL_CYCLE",
                        "PayrollCycle",
                        cycleId,
                        JsonSerializer.Serialize(new { cycle.Status }),
                        JsonSerializer.Serialize(new { Status = "Approved", remarks })
                    );
                }

                return ApiResponse<bool>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApprovePayrollCycle failed");
                return ApiResponse<bool>.Fail("Error");
            }
        }

        #endregion

        #region Payroll Processing

        public async Task<ApiResponse<int>> ProcessSingleEmployeePayrollAsync(ProcessSingleEmployeePayrollRequest request, int userId)
        {
            try
            {
                var processingId = await _payrollRepository.ProcessEmployeePayrollAsync(request, userId);

                if (processingId > 0)
                {
                    await _auditService.LogAsync(
                        userId,
                        "PROCESS_EMPLOYEE_PAYROLL",
                        "PayrollProcessing",
                        processingId,
                        null,
                        JsonSerializer.Serialize(request)
                    );
                }

                return ApiResponse<int>.Success(processingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessSingleEmployeePayroll failed");
                return ApiResponse<int>.Fail("Error");
            }
        }

        public async Task<ApiResponse<bool>> ProcessBulkPayrollAsync(ProcessPayrollRequest request, int userId)
        {
            try
            {
                var result = await _payrollRepository.ProcessBulkPayrollAsync(
                    request.PayrollCycleId,
                     
                    userId);

                if (result)
                {
                    await _auditService.LogAsync(
                        userId,
                        "BULK_PAYROLL_PROCESS",
                        "PayrollCycle",
                        request.PayrollCycleId,
                        null,
                        JsonSerializer.Serialize(request)
                    );
                }

                return ApiResponse<bool>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessBulkPayroll failed");
                return ApiResponse<bool>.Fail("Error");
            }
        }

        #endregion

        #region Payroll Actions

        public async Task<ApiResponse<bool>> RecalculatePayrollAsync(int processingId, int userId)
        {
            try
            {
                var result = await _payrollRepository.RecalculatePayrollAsync(processingId, userId);

                if (result)
                {
                    await _auditService.LogAsync(
                        userId,
                        "RECALCULATE_PAYROLL",
                        "PayrollProcessing",
                        processingId,
                        null,
                        null
                    );
                }

                return ApiResponse<bool>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<bool>.Fail("Error");
            }
        }

        public async Task<ApiResponse<bool>> HoldPayrollAsync(int processingId, string reason, int userId)
        {
            try
            {
                var result = await _payrollRepository.HoldPayrollAsync(processingId, reason, userId);

                if (result)
                {
                    await _auditService.LogAsync(
                        userId,
                        "HOLD_PAYROLL",
                        "PayrollProcessing",
                        processingId,
                        null,
                        JsonSerializer.Serialize(new { reason, status = "OnHold" })
                    );
                }

                return ApiResponse<bool>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<bool>.Fail("Error");
            }
        }

        public async Task<ApiResponse<bool>> ReleasePayrollAsync(int processingId, int userId)
        {
            try
            {
                var result = await _payrollRepository.ReleasePayrollHoldAsync(processingId, userId);

                if (result)
                {
                    await _auditService.LogAsync(
                        userId,
                        "RELEASE_PAYROLL_HOLD",
                        "PayrollProcessing",
                        processingId,
                        null,
                        JsonSerializer.Serialize(new { status = "Active" })
                    );
                }

                return ApiResponse<bool>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<bool>.Fail("Error");
            }
        }

        public async Task<ApiResponse<bool>> MarkPayrollAsPaidAsync(int processingId, string paymentMode, string? referenceNo, int userId)
        {
            try
            {
                var result = await _payrollRepository.MarkPayrollAsPaidAsync(processingId, paymentMode, referenceNo, userId);

                if (result)
                {
                    await _auditService.LogAsync(
                        userId,
                        "MARK_PAYROLL_PAID",
                        "PayrollProcessing",
                        processingId,
                        null,
                        JsonSerializer.Serialize(new { paymentMode, referenceNo, status = "Paid" })
                    );
                }

                return ApiResponse<bool>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<bool>.Fail("Error");
            }
        }

        #endregion

        #region Arrears

        public async Task<ApiResponse<bool>> CalculateArrearsAsync(int employeeId, int newStructureId, DateTime revisionDate, int userId)
        {
            try
            {
                var result = await _payrollRepository.CalculateArrearsAsync(employeeId, newStructureId, revisionDate, userId);

                if (result > 0)
                {
                    await _auditService.LogAsync(
                        userId,
                        "CALCULATE_ARREARS",
                        "PayrollArrears",
                        result,
                        null,
                        JsonSerializer.Serialize(new { employeeId, newStructureId, revisionDate })
                    );
                }

                return ApiResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<bool>.Fail("Error");
            }
        }

        #endregion

        #region Reports

        public async Task<ApiResponse<PayrollRegisterResponse>> GetPayrollRegisterReportAsync(int cycleId)
        {
            try
            {
                var summary = await _payrollRepository.GetPayrollSummaryAsync(cycleId);
                var register = await _payrollRepository.GetPayrollRegisterAsync(cycleId);

                var response = new PayrollRegisterResponse
                {
                    PayrollCycleId = cycleId,
                    CycleName = summary.CycleName,
                    Month = summary.Month,
                    Year = summary.Year,
                    MonthName = summary.MonthName,
                    TotalEmployees = register.Count,
                    TotalBasic = register.Sum(x => x.BasicSalary),
                    TotalGross = register.Sum(x => x.GrossSalary),
                    TotalDeductions = register.Sum(x => x.TotalDeductions),
                    TotalNet = register.Sum(x => x.NetSalary),
                    TotalPFEmployee = register.Sum(x => x.PFEmployee),
                    TotalESIEmployee = register.Sum(x => x.ESIEmployee),
                    TotalPT = register.Sum(x => x.ProfessionalTax),
                    TotalTDS = register.Sum(x => x.TDS),
                    Employees = register.Select((x, index) => new PayrollRegisterEmployeeData
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        EmployeeName = x.EmployeeName,
                        Department = x.Department,
                        Designation = x.Designation,
                        WorkingDays = x.TotalWorkingDays,
                        PresentDays = x.PresentDays,
                        LOPDays = x.LOPDays,
                        BasicSalary = x.BasicSalary,
                        GrossSalary = x.GrossSalary,
                        PFEmployee = x.PFEmployee,
                        ESIEmployee = x.ESIEmployee,
                        PT = x.ProfessionalTax,
                        TDS = x.TDS,
                        LoanEMI = x.LoanEMI,
                        OtherDeductions = x.TotalDeductions - x.PFEmployee - x.ESIEmployee - x.ProfessionalTax - x.TDS - x.LoanEMI,
                        TotalDeductions = x.TotalDeductions,
                        NetSalary = x.NetSalary,
                        BankAccountNumber = x.AccountNumber,
                        IFSCCode = x.IFSCCode,
                        PaymentStatus = x.PaymentStatus
                    }).ToList(),
                    GeneratedDate = DateTime.UtcNow
                };

                return ApiResponse<PayrollRegisterResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPayrollRegisterReport failed for cycle {CycleId}", cycleId);
                return ApiResponse<PayrollRegisterResponse>.Fail("Error");
            }
        }

        public async Task<ApiResponse<BankTransferResponse>> GetBankTransferReportAsync(int cycleId)
        {
            try
            {
                var summary = await _payrollRepository.GetPayrollSummaryAsync(cycleId);
                var register = await _payrollRepository.GetPayrollRegisterAsync(cycleId);

                var response = new BankTransferResponse
                {
                    BatchNumber = $"PAY-{summary.Year}{summary.Month:00}-{cycleId}",
                    PayrollCycleId = cycleId,
                    CycleName = summary.CycleName,
                    Month = summary.Month,
                    Year = summary.Year,
                    TotalEmployees = register.Count,
                    TotalAmount = register.Sum(x => x.NetSalary),
                    Employees = register.Select((x, index) => new BankTransferEmployeeData
                    {
                        SrNo = index + 1,
                        EmployeeCode = x.EmployeeCode,
                        EmployeeName = x.EmployeeName,
                        BankName = x.BankName ?? string.Empty,
                        AccountNumber = x.AccountNumber ?? string.Empty,
                        IFSCCode = x.IFSCCode ?? string.Empty,
                        NetSalary = x.NetSalary,
                        Remarks = x.IsOnHold ? $"On hold: {x.HoldReason}" : x.PaymentStatus
                    }).ToList(),
                    GeneratedDate = DateTime.UtcNow
                };

                return ApiResponse<BankTransferResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBankTransferReport failed for cycle {CycleId}", cycleId);
                return ApiResponse<BankTransferResponse>.Fail("Error");
            }
        }

        public Task<ApiResponse<StatutoryComplianceResponse>> GetStatutoryComplianceReportAsync(int month, int year)
        {
            var response = new StatutoryComplianceResponse
            {
                Month = month,
                Year = year,
                MonthName = new DateTime(year, month, 1).ToString("MMMM"),
                GeneratedDate = DateTime.UtcNow
            };

            return Task.FromResult(ApiResponse<StatutoryComplianceResponse>.Success(response));
        }

        public async Task<ApiResponse<DepartmentWiseSalaryResponse>> GetDepartmentWiseSalaryReportAsync(int cycleId)
        {
            try
            {
                var summary = await _payrollRepository.GetPayrollSummaryAsync(cycleId);
                var register = await _payrollRepository.GetPayrollRegisterAsync(cycleId);

                var departments = register
                    .GroupBy(x => x.Department)
                    .Select(group =>
                    {
                        var employeeCount = group.Count();
                        var totalNet = group.Sum(x => x.NetSalary);

                        return new DepartmentSalaryData
                        {
                            DepartmentName = string.IsNullOrWhiteSpace(group.Key) ? "Unassigned" : group.Key,
                            EmployeeCount = employeeCount,
                            TotalGross = group.Sum(x => x.GrossSalary),
                            TotalDeductions = group.Sum(x => x.TotalDeductions),
                            TotalNet = totalNet,
                            AverageSalary = employeeCount == 0 ? 0 : totalNet / employeeCount
                        };
                    })
                    .OrderBy(x => x.DepartmentName)
                    .ToList();

                var response = new DepartmentWiseSalaryResponse
                {
                    PayrollCycleId = cycleId,
                    Month = summary.Month,
                    Year = summary.Year,
                    MonthName = summary.MonthName,
                    GrandTotalGross = departments.Sum(x => x.TotalGross),
                    GrandTotalDeductions = departments.Sum(x => x.TotalDeductions),
                    GrandTotalNet = departments.Sum(x => x.TotalNet),
                    Departments = departments,
                    GeneratedDate = DateTime.UtcNow
                };

                return ApiResponse<DepartmentWiseSalaryResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDepartmentWiseSalaryReport failed for cycle {CycleId}", cycleId);
                return ApiResponse<DepartmentWiseSalaryResponse>.Fail("Error");
            }
        }

        public async Task<ApiResponse<EmployeePayrollDetailResponse>> GetEmployeePayrollDetailsAsync(int cycleId, int employeeId)
        {
            try
            {
                var register = await _payrollRepository.GetPayrollRegisterAsync(cycleId);
                var employeePayroll = register.FirstOrDefault(x => x.EmployeeId == employeeId);

                return employeePayroll == null
                    ? ApiResponse<EmployeePayrollDetailResponse>.Fail("Payroll details not found")
                    : ApiResponse<EmployeePayrollDetailResponse>.Success(employeePayroll);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetEmployeePayrollDetails failed for cycle {CycleId}, employee {EmployeeId}", cycleId, employeeId);
                return ApiResponse<EmployeePayrollDetailResponse>.Fail("Error");
            }
        }

        public async Task<ApiResponse<List<EmployeePayrollDetailResponse>>> GetPayrollRegisterAsync(int cycleId)
        {
            try
            {
                var register = await _payrollRepository.GetPayrollRegisterAsync(cycleId);
                return ApiResponse<List<EmployeePayrollDetailResponse>>.Success(register);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPayrollRegister failed for cycle {CycleId}", cycleId);
                return ApiResponse<List<EmployeePayrollDetailResponse>>.Fail("Error");
            }
        }

        public async Task<ApiResponse<PayrollDashboardResponse>> GetPayrollDashboardAsync()
        {
            try
            {
                var dashboard = await _payrollRepository.GetPayrollDashboardAsync();
                return ApiResponse<PayrollDashboardResponse>.Success(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPayrollDashboard failed");
                return ApiResponse<PayrollDashboardResponse>.Fail("Error");
            }
        }

        public async Task<ApiResponse<List<PayrollArrears>>> GetPendingArrearsAsync(int? employeeId = null)
        {
            try
            {
                var arrears = await _payrollRepository.GetPendingArrearsAsync(employeeId);
                return ApiResponse<List<PayrollArrears>>.Success(arrears);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPendingArrears failed");
                return ApiResponse<List<PayrollArrears>>.Fail("Error");
            }
        }

        #endregion
    }
}
