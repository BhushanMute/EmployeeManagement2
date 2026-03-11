namespace EmployeeManagement.API.services
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink);
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task<bool> SendPasswordChangedNotificationAsync(string toEmail, string userName);
    }
}
