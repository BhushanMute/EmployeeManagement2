using Dapper;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PayrollEmailBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PayrollEmailBackgroundService> _logger;
    private readonly IConfiguration _config;

    public PayrollEmailBackgroundService(IServiceProvider serviceProvider, ILogger<PayrollEmailBackgroundService> logger, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Payroll Email Background Service Started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessEmailQueueAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background email processing.");
            }

            // Har 5 second me check karo
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task ProcessEmailQueueAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbHelper = scope.ServiceProvider.GetRequiredService<DbHelper>(); // Tumhara existing DbHelper
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var ssrsService = scope.ServiceProvider.GetRequiredService<ISsrsReportService>(); // PDF generate karne ke liye

        using var connection = dbHelper.GetConnection();

        // 1. Fetch 10 pending emails
        var pendingEmails = await connection.QueryAsync<EmailQueueDto>(
            "sp_GetPendingEmailQueue",
            new { BatchSize = 10 },
            commandType: CommandType.StoredProcedure);

        if (!pendingEmails.Any()) return;

        _logger.LogInformation($"Processing {pendingEmails.Count()} emails...");

        // 2. Process each email
        foreach (var email in pendingEmails)
        {
            try
            {
                // Generate PDF from SSRS (Tumhara existing logic)
                byte[] pdfBytes = await ssrsService.GenerateSalarySlipPdfAsync(
                    email.EmployeeId, email.Month, email.Year, email.PayrollProcessId);

                string fileName = $"SalarySlip_{email.EmployeeId}_{email.Month}_{email.Year}.pdf";

                // Save PDF to disk if needed, or directly attach from byte array
                string savedPath = await SavePdfToDiskAsync(pdfBytes, fileName);

                // Send Email
                string subject = $"Salary Slip for {GetMonthName(email.Month)} {email.Year} - Your Company Name";
                string body = $"Dear {email.EmployeeName},<br><br>Please find attached your salary slip...";

                await emailService.SendEmailWithAttachmentAsync(
                    email.EmailAddress, subject, body, pdfBytes, fileName);

                // Update DB: Success
                await connection.ExecuteAsync("sp_UpdateEmailQueueStatus", new
                {
                    QueueId = email.QueueId,
                    Status = "Sent",
                    PdfFilePath = savedPath
                }, commandType: CommandType.StoredProcedure);

                _logger.LogInformation($"✅ Email sent to {email.EmailAddress}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send email to {email.EmailAddress}");

                // Update DB: Failed
                await connection.ExecuteAsync("sp_UpdateEmailQueueStatus", new
                {
                    QueueId = email.QueueId,
                    Status = "Failed",
                    ErrorMessage = ex.Message
                }, commandType: CommandType.StoredProcedure);
            }
        }
    }

    private string GetMonthName(int month) => new DateTime(2000, month, 1).ToString("MMMM");
    private Task<string> SavePdfToDiskAsync(byte[] bytes, string name) => Task.FromResult($"wwwroot/payslips/{name}"); // Dummy logic
}

public class EmailQueueDto
{
    public long QueueId { get; set; }
    public int EmployeeId { get; set; }
    public string EmailAddress { get; set; }
    public string EmployeeName { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int PayrollProcessId { get; set; }
}