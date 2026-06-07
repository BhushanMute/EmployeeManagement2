// File: EmployeeManagement.API/Services/IEmailService.cs
using EmployeeManagement.API.Models.Ticket;

namespace EmployeeManagement.API.services
{
    public interface IEmailService
    {
        // ===== EXISTING METHODS =====
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink);
        Task<bool> SendPasswordChangedNotificationAsync(string toEmail, string userName);
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task<bool> SendEmailWithAttachmentAsync(string toEmail, string subject, string htmlBody, byte[] fileBytes, string fileName);

        // ===== LEAVE METHODS (Existing) =====
        Task SendLeaveAppliedNotification(string managerEmail, string employeeName,
            string leaveType, DateTime startDate, DateTime endDate, decimal totalDays, string reason);
        Task SendLeaveApprovedNotification(string employeeEmail, string employeeName,
            string leaveType, DateTime startDate, DateTime endDate, decimal totalDays, string? remarks);
        Task SendLeaveRejectedNotification(string employeeEmail, string employeeName,
            string leaveType, DateTime startDate, DateTime endDate, string? remarks);
        Task SendLeaveCancelledNotification(string managerEmail, string employeeName,
            string leaveType, DateTime startDate, DateTime endDate, string? cancelReason);
        Task SendLeaveAppliedNotificationAsync(List<string> recipientEmails, string employeeName,
            string leaveType, DateTime startDate, DateTime endDate, decimal totalDays, string reason, int requestId);

        // ===== ✅ NEW: TICKET METHODS =====
        Task SendTicketCreatedNotificationAsync(List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string description,
            string ticketType, string priority, string createdByName, DateTime? dueDate);

        Task SendTicketAssignedNotificationAsync(List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string priority,
            string assignedByName, string assignedToName, DateTime? dueDate);

        Task SendTicketStatusChangedNotificationAsync(List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string oldStatus,
            string newStatus, string changedByName, string? remarks);

        Task SendTicketCommentAddedNotificationAsync(List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string commentByName,
            string comment, bool isInternal);

        Task SendTicketReadyForQANotificationAsync(List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string description,
            string priority, string developerName);

        Task SendTicketClosedNotificationAsync(List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string closedByName,
            string? remarks, int resolutionMinutes);

        Task SendTicketReopenedNotificationAsync(List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string reopenedByName, string? reason);

        Task SendTicketOverdueNotificationAsync(List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string priority, DateTime dueDate);
    }
}