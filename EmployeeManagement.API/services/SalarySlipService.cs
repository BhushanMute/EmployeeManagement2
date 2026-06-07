using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;           // ✅ Add this
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.Salary;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.API.Services.Implementation
{
    public class SalarySlipService : ISalarySlipService
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SalarySlipService> _logger;

        public SalarySlipService(
            IPayrollRepository payrollRepository,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<SalarySlipService> logger)
        {
            _payrollRepository = payrollRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<ApiResponse<SalarySlipResponse>> GetSalarySlipAsync(int slipId)
        {
            try
            {
                var payslip = await _payrollRepository.GetPayslipDataAsync(slipId);
                if (payslip == null)
                {
                    return ApiResponse<SalarySlipResponse>.Fail("Salary slip not found");
                }

                return ApiResponse<SalarySlipResponse>.Success(MapToResponse(payslip));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load salary slip {SlipId}", slipId);
                return ApiResponse<SalarySlipResponse>.Fail("Error loading salary slip");
            }
        }

        public async Task<ApiResponse<List<SalarySlipResponse>>> GetEmployeeSalarySlipsAsync(int employeeId, int? year, int? month)
        {
            try
            {
                var payrollHistory = await _payrollRepository.GetEmployeePayrollHistoryAsync(employeeId, year);
                if (month.HasValue)
                {
                    payrollHistory = payrollHistory.Where(x => x.PayrollCycle?.Month == month.Value).ToList();
                }

                var slips = new List<SalarySlipResponse>();
                foreach (var payroll in payrollHistory.OrderByDescending(x => x.PayrollCycle?.Year).ThenByDescending(x => x.PayrollCycle?.Month))
                {
                    var payslip = await _payrollRepository.GetPayslipDataAsync(payroll.Id);
                    if (payslip != null)
                    {
                        slips.Add(MapToResponse(payslip));
                    }
                }

                return ApiResponse<List<SalarySlipResponse>>.Success(slips);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load salary slips for employee {EmployeeId}", employeeId);
                return ApiResponse<List<SalarySlipResponse>>.Fail("Error loading salary slips");
            }
        }

        public async Task<ApiResponse<bool>> GenerateSalarySlipsAsync(GenerateSalarySlipRequest request, int generatedBy)
        {
            try
            {
                var payrolls = await _payrollRepository.GetPayrollProcessingByCycleAsync(request.PayrollCycleId);
                if (request.EmployeeIds?.Any() == true)
                {
                    payrolls = payrolls.Where(x => request.EmployeeIds.Contains(x.EmployeeId)).ToList();
                }

                if (!payrolls.Any())
                {
                    return ApiResponse<bool>.Fail("No processed payroll records found for this cycle");
                }

                foreach (var payroll in payrolls)
                {
                    await _payrollRepository.LogPayslipGenerationAsync(payroll.Id, generatedBy);
                }

                return ApiResponse<bool>.Success(true, "Salary slips generated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate salary slips for cycle {CycleId}", request.PayrollCycleId);
                return ApiResponse<bool>.Fail("Error generating salary slips");
            }
        }

        public async Task<ApiResponse<bool>> SendSalarySlipEmailAsync(SendSalarySlipEmailRequest request)
        {
            try
            {
                var payslip = await _payrollRepository.GetPayslipDataAsync(request.SlipId);
                if (payslip == null)
                {
                    return ApiResponse<bool>.Fail("Salary slip not found");
                }

                var emailTo = request.EmailTo ?? payslip.Email;
                if (string.IsNullOrWhiteSpace(emailTo))
                {
                    return ApiResponse<bool>.Fail("Employee email is not available for this salary slip");
                }

                var sent = await _emailService.SendEmailAsync(
                    emailTo,
                    $"Salary Slip - {GetMonthName(payslip.Month)} {payslip.Year}",
                    BuildSalarySlipEmailBody(payslip, request.CustomMessage));

                return sent
                    ? ApiResponse<bool>.Success(true, "Salary slip email sent")
                    : ApiResponse<bool>.Fail("Salary slip email failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send salary slip email for slip {SlipId}", request.SlipId);
                return ApiResponse<bool>.Fail("Error sending salary slip email");
            }
        }

        //public async Task<ApiResponse<bool>> SendBulkSalarySlipEmailAsync(SendBulkSalarySlipEmailRequest request)
        //{
        //    try
        //    {
        //        var slipIds = request.SlipIds?.Where(x => x > 0).Distinct().ToList() ?? new List<int>();
        //        if (request.SendToAll || !slipIds.Any())
        //        {
        //            slipIds = (await _payrollRepository.GetPayrollProcessingByCycleAsync(request.CycleId))
        //                .Select(x => x.Id)
        //                .ToList();
        //        }

        //        if (!slipIds.Any())
        //        {
        //            return ApiResponse<bool>.Fail("No salary slips found for email sending");
        //        }

        //        var failures = new List<string>();
        //        foreach (var slipId in slipIds)
        //        {
        //            var result = await SendSalarySlipEmailAsync(new SendSalarySlipEmailRequest
        //            {
        //                SlipId = slipId,
        //                CustomMessage = request.CustomMessage,
        //                IncludePayslipPDF = request.IncludePayslipPDF
        //            });

        //            if (!result.Status)
        //            {
        //                failures.Add($"Slip {slipId}: {result.Message}");
        //            }
        //        }

        //        return failures.Any()
        //            ? ApiResponse<bool>.Fail("Some salary slip emails failed", failures)
        //            : ApiResponse<bool>.Success(true, "Salary slip emails sent");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed bulk salary slip email for cycle {CycleId}", request.CycleId);
        //        return ApiResponse<bool>.Fail("Error sending salary slip emails");
        //    }
        //}

        public async Task<ApiResponse<bool>> SendBulkSalarySlipEmailAsync(SendBulkSalarySlipEmailRequest request)
        {
            try
            {
                // 1. Validate Cycle
                var payrolls = await _payrollRepository.GetPayrollProcessingByCycleAsync(request.CycleId);
                if (!payrolls.Any())
                    return ApiResponse<bool>.Fail("No payroll records found for this cycle.");

                // 2. Filter specific employees if needed
                var targetPayrolls = payrolls.AsEnumerable();
                if (request.SlipIds?.Any() == true && !request.SendToAll)
                {
                    targetPayrolls = targetPayrolls.Where(p => request.SlipIds.Contains(p.Id));
                }

                if (!targetPayrolls.Any())
                    return ApiResponse<bool>.Fail("No valid slips selected for email.");

                // 3. 🚀 CORPORATE LOGIC: Insert into Queue Table (Don't send email here!)
                // Note: Tumhe IPayrollRepository me ek naya method add karna hoga: InsertBulkEmailQueueAsync
                int queuedCount = await _payrollRepository.InsertBulkEmailQueueAsync(
                    request.CycleId,
                    targetPayrolls.Select(p => p.Id).ToList()
                );

                _logger.LogInformation("Queued {Count} emails for Cycle {CycleId}", queuedCount, request.CycleId);

                return ApiResponse<bool>.Success(true, $"{queuedCount} salary slips queued for email delivery. Background process started.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to queue bulk emails for cycle {CycleId}", request.CycleId);
                return ApiResponse<bool>.Fail("Error queueing emails.");
            }
        }

        // ... (Baaki methods same rahenge) ...
        public Task<ApiResponse<bool>> TrackViewAsync(int slipId)
        {
            return Task.FromResult(ApiResponse<bool>.Success(true));
        }

        public Task<ApiResponse<bool>> TrackDownloadAsync(int slipId)
        {
            return Task.FromResult(ApiResponse<bool>.Success(true));
        }

        public ApiResponse<string> GetReportUrl(int slipId)
        {
            var reportServerUrl = _configuration["SSRSSettings:ReportServerUrl"] ?? "/ReportServer";
            var reportPath = _configuration["SSRSSettings:ReportPath"] ?? "/PayrollReports";
            var reportUrl = $"{reportServerUrl}?{reportPath}/SalarySlip&SlipId={slipId}&rs:Format=PDF";

            return ApiResponse<string>.Success(reportUrl);
        }

        private SalarySlipResponse MapToResponse(PayslipData payslip)
        {
            var payPeriodStart = new DateTime(payslip.Year, payslip.Month, 1);
            var payPeriodEnd = payPeriodStart.AddMonths(1).AddDays(-1);

            return new SalarySlipResponse
            {
                // ===== Basic Slip Info =====
                SlipId = payslip.ProcessingId,
                SlipNumber = $"SLIP-{payslip.Year}{payslip.Month:00}-{payslip.ProcessingId}",
                PayrollCycleId = 0,
                EmployeeId = payslip.EmployeeId,

                // ===== Employee Details =====
                EmployeeCode = payslip.EmployeeCode,
                EmployeeName = payslip.EmployeeName,
                Email = payslip.Email ?? string.Empty,
                Department = payslip.Department ?? string.Empty,
                Designation = payslip.Designation ?? string.Empty,
                JoiningDate = null,
                PanNumber = null,

                // ===== Period =====
                Month = payslip.Month,
                Year = payslip.Year,
                MonthName = GetMonthName(payslip.Month),
                PayPeriodStart = payPeriodStart,
                PayPeriodEnd = payPeriodEnd,

                // ===== Status & Tracking =====
                Status = "Generated",
                EmailSent = false,
                EmailSentDate = null,
                GeneratedDate = DateTime.UtcNow,
                ViewCount = 0,
                DownloadCount = 0,

                // ===== Salary Amounts =====
                BasicSalary = payslip.BasicSalary,
                GrossSalary = payslip.GrossSalary,
                TotalEarnings = payslip.TotalEarnings,
                TotalDeductions = payslip.TotalDeductions,
                NetSalary = payslip.NetSalary,
                NetSalaryInWords = !string.IsNullOrWhiteSpace(payslip.NetSalaryInWords)
                    ? payslip.NetSalaryInWords
                    : ConvertAmountToWords(payslip.NetSalary),

                // ===== Attendance =====
                TotalDays = DateTime.DaysInMonth(payslip.Year, payslip.Month),
                TotalWorkingDays = payslip.TotalWorkingDays,
                PresentDays = (int?)payslip.PresentDays,
                PaidLeaveDays = (int?)payslip.PaidLeaveDays,
                LopDays = payslip.LOPDays,
                PaidDays = payslip.PresentDays + payslip.PaidLeaveDays,
                LeaveDays = (int?)payslip.PaidLeaveDays,
                AbsentDays = (int?)payslip.LOPDays,

                // ===== Bank Details =====
                BankName = payslip.BankName,
                BankAccountNumber = payslip.AccountNumber,
                IfscCode = payslip.IFSCCode,

                // ===== Components =====
                Earnings = payslip.Earnings?
                    .OrderBy(x => x.DisplayOrder)
                    .Select(c => MapComponent(c, "Earning"))
                    .ToList() ?? new List<SlipComponentResponse>(),

                Deductions = payslip.Deductions?
                    .OrderBy(x => x.DisplayOrder)
                    .Select(c => MapComponent(c, "Deduction"))
                    .ToList() ?? new List<SlipComponentResponse>(),

                // ===== Company Details =====
                CompanyDetails = new CompanyDetailsResponse
                {
                    CompanyName = _configuration["CompanyDetails:CompanyName"]
                                  ?? "Employee Management System",
                    Address = _configuration["CompanyDetails:Address"]
                              ?? "Company Address",
                    Email = _configuration["CompanyDetails:Email"],
                    Phone = _configuration["CompanyDetails:Phone"],
                    Website = _configuration["CompanyDetails:Website"],
                    PAN = _configuration["CompanyDetails:PAN"],
                    CIN = _configuration["CompanyDetails:CIN"],
                    GSTIN = _configuration["CompanyDetails:GSTIN"],
                    GstNumber = _configuration["CompanyDetails:GSTIN"],
                    PanNumber = _configuration["CompanyDetails:PAN"],
                    LogoPath = _configuration["CompanyDetails:LogoPath"],
                    LogoUrl = _configuration["CompanyDetails:LogoUrl"]
                }
            };
        }
        private static string ConvertAmountToWords(decimal? amount)
        {
            if (!amount.HasValue || amount.Value == 0)
                return "Zero Rupees Only";

            var value = Math.Floor(amount.Value);
            var paise = Math.Round((amount.Value - value) * 100);

            var words = NumberToWords((long)value) + " Rupees";

            if (paise > 0)
                words += " and " + NumberToWords((long)paise) + " Paise";

            return words + " Only";
        }

        private static string NumberToWords(long number)
        {
            if (number == 0) return "Zero";
            if (number < 0) return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 10000000) > 0)
            {
                words += NumberToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += NumberToWords(number / 100000) + " Lakh ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five",
            "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve",
            "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
            "Eighteen", "Nineteen" };

                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty",
            "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }

        private static SlipComponentResponse MapComponent(
            PayrollComponentResponse component,
            string componentType)
        {
            return new SlipComponentResponse
            {
                ComponentId = component.ComponentId,            // ✅ Use actual ID
                ComponentName = component.ComponentName,
                ComponentCode = component.ComponentCode,
                ComponentType = componentType,
                Amount = component.Amount,
                DisplayOrder = component.DisplayOrder,
                IsTaxable = component.IsTaxable                 // ✅ Use actual value
            };
        }
      

        private static string GetMonthName(int month)
        {
            return month is >= 1 and <= 12
                ? new DateTime(2000, month, 1).ToString("MMMM")
                : string.Empty;
        }
        //private static string GetMonthName(int month)
        //{
        //    return month is >= 1 and <= 12
        //        ? new DateTime(2000, month, 1).ToString("MMMM")
        //        : string.Empty;
        //}
        //private static string ConvertAmountToWords(decimal amount)
        //{
        //    return $"Rupees {amount:N2} Only";
        //}
        //private static string GetMonthName(int month)
        //{
        //    return month is >= 1 and <= 12
        //        ? new DateTime(2000, month, 1).ToString("MMMM")
        //        : string.Empty;
        //}

        private static string BuildSalarySlipEmailBody(PayslipData payslip, string? customMessage)
        {
            var message = string.IsNullOrWhiteSpace(customMessage)
                ? "Please find your salary slip details below."
                : customMessage;

            return $@"
<html>
<body style='font-family: Arial, sans-serif; color: #222;'>
    <h2>Salary Slip - {GetMonthName(payslip.Month)} {payslip.Year}</h2>
    <p>Hello {payslip.EmployeeName},</p>
    <p>{message}</p>
    <table style='border-collapse: collapse; width: 100%; max-width: 560px;'>
        <tr><td style='padding: 8px; border: 1px solid #ddd;'>Gross Salary</td><td style='padding: 8px; border: 1px solid #ddd; text-align: right;'>{payslip.GrossSalary:N2}</td></tr>
        <tr><td style='padding: 8px; border: 1px solid #ddd;'>Total Deductions</td><td style='padding: 8px; border: 1px solid #ddd; text-align: right;'>{payslip.TotalDeductions:N2}</td></tr>
        <tr><td style='padding: 8px; border: 1px solid #ddd; font-weight: bold;'>Net Salary</td><td style='padding: 8px; border: 1px solid #ddd; text-align: right; font-weight: bold;'>{payslip.NetSalary:N2}</td></tr>
    </table>
    <p style='font-size: 12px; color: #666;'>This is an automated email from Employee Management System.</p>
</body>
</html>";
        }
    }
}
