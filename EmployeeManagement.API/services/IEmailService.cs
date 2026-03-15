namespace EmployeeManagement.API.services
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink);
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task<bool> SendPasswordChangedNotificationAsync(string toEmail, string userName);
        // Add these methods to IEmailService
        Task SendLeaveAppliedNotification(string managerEmail, string employeeName, string leaveType,
            DateTime startDate, DateTime endDate, decimal totalDays, string reason);
        Task SendLeaveApprovedNotification(string employeeEmail, string employeeName, string leaveType,
            DateTime startDate, DateTime endDate, decimal totalDays, string? remarks);
        Task SendLeaveRejectedNotification(string employeeEmail, string employeeName, string leaveType,
            DateTime startDate, DateTime endDate, string? remarks);
        Task SendLeaveCancelledNotification(string managerEmail, string employeeName, string leaveType,
            DateTime startDate, DateTime endDate, string? cancelReason);
    }
}
