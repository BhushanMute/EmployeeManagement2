using EmployeeManagement.API.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement.API.services
{
    public class SsrsReportService : ISsrsReportService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SsrsReportService> _logger;

        public SsrsReportService(IConfiguration config, ILogger<SsrsReportService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<byte[]> GenerateSalarySlipPdfAsync(int employeeId, int month = 0, int year = 0, int payrollCycleId = 0  )
        {
            try
            {
                // appsettings.json se SSRS URL lo
                var reportServerUrl = _config["SSRS:ReportServerUrl"]; // e.g., http://localhost/ReportServer
                var reportPath = _config["SSRS:SalarySlipPath"];       // e.g., /HRMS_Reports/SalarySlip

                if (string.IsNullOrEmpty(reportServerUrl) || string.IsNullOrEmpty(reportPath))
                    throw new Exception("SSRS configuration missing in appsettings.json");

                // SSRS URL format: ?/Path&rs:Command=Render&rs:Format=PDF&Param1=Value1
                var ssrsUrl = $"{reportServerUrl}?{reportPath}" +
                              $"&rs:Command=Render&rs:Format=PDF" +
                              $"&EmployeeId={employeeId}" +
                              $"&Month={month}&Year={year}" +
                              $"&PayrollCycleId={payrollCycleId}";

                _logger.LogInformation("Calling SSRS for Employee {EmpId}", employeeId);

                // Windows Authentication use karna hai kyunki tumhara SQL Server Windows Auth pe hai
                var handler = new HttpClientHandler { UseDefaultCredentials = true };
                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(60); // PDF me time lagta hai

                var response = await client.GetAsync(ssrsUrl);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"SSRS failed with status: {response.StatusCode}");

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SSRS PDF Generation Failed for Emp {EmpId}", employeeId);
                throw;
            }
        }
    }
}