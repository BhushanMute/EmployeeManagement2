using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Ticket;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EmployeeManagement.API.services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink)
        {
            try
            {
                var subject = "Password Reset Request - Employee Management System";

                var htmlBody = GetPasswordResetEmailTemplate(userName, resetLink);

                return await SendEmailAsync(toEmail, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendPasswordChangedNotificationAsync(string toEmail, string userName)
        {
            try
            {
                var subject = "Password Changed Successfully - Employee Management System";

                var htmlBody = GetPasswordChangedEmailTemplate(userName);

                return await SendEmailAsync(toEmail, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password changed notification to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = _emailSettings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error sending email to {Email}. Status: {StatusCode}",
                    toEmail, smtpEx.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }

        private string GetPasswordResetEmailTemplate(string userName, string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table role='presentation' style='width: 100%; border-collapse: collapse;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table role='presentation' style='width: 600px; border-collapse: collapse; background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='padding: 40px 40px 20px 40px; text-align: center; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 10px 10px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px;'>🔐 Password Reset</h1>
                        </td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style='padding: 40px;'>
                            <p style='font-size: 16px; color: #333333; margin: 0 0 20px 0;'>
                                Hello <strong>{userName}</strong>,
                            </p>
                            
                            <p style='font-size: 16px; color: #666666; line-height: 1.6; margin: 0 0 20px 0;'>
                                We received a request to reset your password for your Employee Management System account. 
                                Click the button below to create a new password:
                            </p>
                            
                            <!-- Button -->
                            <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                                <tr>
                                    <td align='center' style='padding: 30px 0;'>
                                        <a href='{resetLink}' 
                                           style='display: inline-block; padding: 15px 40px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; text-decoration: none; border-radius: 8px; font-size: 16px; font-weight: bold; box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);'>
                                            Reset My Password
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- Warning Box -->
                            <div style='background-color: #fff3cd; border: 1px solid #ffc107; border-radius: 8px; padding: 15px; margin: 20px 0;'>
                                <p style='font-size: 14px; color: #856404; margin: 0;'>
                                    ⚠️ <strong>This link will expire in 30 minutes.</strong><br>
                                    If you didn't request this password reset, please ignore this email or contact support if you have concerns.
                                </p>
                            </div>
                            
                            <!-- Alternative Link -->
                            <p style='font-size: 14px; color: #999999; margin: 20px 0 0 0;'>
                                If the button doesn't work, copy and paste this link into your browser:
                            </p>
                            <p style='font-size: 12px; color: #667eea; word-break: break-all; margin: 10px 0;'>
                                {resetLink}
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='padding: 30px 40px; background-color: #f8f9fa; border-radius: 0 0 10px 10px; text-align: center;'>
                            <p style='font-size: 12px; color: #999999; margin: 0;'>
                                This is an automated message from Employee Management System.<br>
                                Please do not reply to this email.
                            </p>
                            <p style='font-size: 12px; color: #999999; margin: 10px 0 0 0;'>
                                © {DateTime.Now.Year} Employee Management System. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        private string GetPasswordChangedEmailTemplate(string userName)
        {
                                        return $@"
                            <!DOCTYPE html>
                            <html lang='en'>
                            <head>
                                <meta charset='UTF-8'>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                <title>Password Changed</title>
                            </head>
                            <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                                <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                                    <tr>
                                        <td align='center' style='padding: 40px 0;'>
                                            <table role='presentation' style='width: 600px; border-collapse: collapse; background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                                                <!-- Header -->
                                                <tr>
                                                    <td style='padding: 40px 40px 20px 40px; text-align: center; background: linear-gradient(135deg, #28a745 0%, #20c997 100%); border-radius: 10px 10px 0 0;'>
                                                        <h1 style='color: #ffffff; margin: 0; font-size: 28px;'>✅ Password Changed</h1>
                                                    </td>
                                                </tr>
                    
                                                <!-- Body -->
                                                <tr>
                                                    <td style='padding: 40px;'>
                                                        <p style='font-size: 16px; color: #333333; margin: 0 0 20px 0;'>
                                                            Hello <strong>{userName}</strong>,
                                                        </p>
                            
                                                        <p style='font-size: 16px; color: #666666; line-height: 1.6; margin: 0 0 20px 0;'>
                                                            Your password has been changed successfully. You can now log in with your new password.
                                                        </p>
                            
                                                        <!-- Security Alert -->
                                                        <div style='background-color: #f8d7da; border: 1px solid #f5c6cb; border-radius: 8px; padding: 15px; margin: 20px 0;'>
                                                            <p style='font-size: 14px; color: #721c24; margin: 0;'>
                                                                🔒 <strong>Security Notice:</strong><br>
                                                                If you did not make this change, please contact our support team immediately and secure your account.
                                                            </p>
                                                        </div>
                            
                                                        <p style='font-size: 14px; color: #666666; margin: 20px 0 0 0;'>
                                                            <strong>Changed at:</strong> {DateTime.UtcNow:MMMM dd, yyyy HH:mm} UTC
                                                        </p>
                                                    </td>
                                                </tr>
                    
                                                <!-- Footer -->
                                                <tr>
                                                    <td style='padding: 30px 40px; background-color: #f8f9fa; border-radius: 0 0 10px 10px; text-align: center;'>
                                                        <p style='font-size: 12px; color: #999999; margin: 0;'>
                                                            © {DateTime.Now.Year} Employee Management System. All rights reserved.
                                                        </p>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </body>
                            </html>";
             }

        public async Task SendLeaveAppliedNotification(string managerEmail, string employeeName,
    string leaveType, DateTime startDate, DateTime endDate, decimal totalDays, string reason)
        {
            var subject = $"🔔 New Leave Request - {employeeName}";
            var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
        <div style='background: linear-gradient(135deg, #667eea, #764ba2); padding: 20px; text-align: center; border-radius: 10px 10px 0 0;'>
            <h2 style='color: white; margin: 0;'>📋 New Leave Request</h2>
        </div>
        <div style='padding: 25px; background: #f8f9fa; border: 1px solid #e9ecef;'>
            <p>Hello,</p>
            <p><strong>{employeeName}</strong> has applied for leave:</p>
            <table style='width: 100%; border-collapse: collapse; margin: 15px 0;'>
                <tr style='border-bottom: 1px solid #dee2e6;'>
                    <td style='padding: 10px; font-weight: bold; width: 40%;'>Leave Type:</td>
                    <td style='padding: 10px;'>{leaveType}</td>
                </tr>
                <tr style='border-bottom: 1px solid #dee2e6;'>
                    <td style='padding: 10px; font-weight: bold;'>From:</td>
                    <td style='padding: 10px;'>{startDate:dd MMM yyyy}</td>
                </tr>
                <tr style='border-bottom: 1px solid #dee2e6;'>
                    <td style='padding: 10px; font-weight: bold;'>To:</td>
                    <td style='padding: 10px;'>{endDate:dd MMM yyyy}</td>
                </tr>
                <tr style='border-bottom: 1px solid #dee2e6;'>
                    <td style='padding: 10px; font-weight: bold;'>Total Days:</td>
                    <td style='padding: 10px;'><strong>{totalDays}</strong></td>
                </tr>
                <tr>
                    <td style='padding: 10px; font-weight: bold;'>Reason:</td>
                    <td style='padding: 10px;'>{reason}</td>
                </tr>
            </table>
            <p>Please review and take action on this request.</p>
            <div style='text-align: center; margin-top: 20px;'>
                <a href='#' style='background: #28a745; color: white; padding: 10px 25px; text-decoration: none; border-radius: 5px; margin-right: 10px;'>✅ Approve</a>
                <a href='#' style='background: #dc3545; color: white; padding: 10px 25px; text-decoration: none; border-radius: 5px;'>❌ Reject</a>
            </div>
        </div>
        <div style='padding: 15px; text-align: center; font-size: 12px; color: #6c757d; border-radius: 0 0 10px 10px; background: #e9ecef;'>
            Employee Management System &copy; {DateTime.Now.Year}
        </div>
    </body>
    </html>";

            await SendEmailAsync(managerEmail, subject, body);
        }

        public async Task SendLeaveApprovedNotification(string employeeEmail, string employeeName,
            string leaveType, DateTime startDate, DateTime endDate, decimal totalDays, string? remarks)
        {
            var subject = $"✅ Leave Approved - {leaveType}";
            var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
        <div style='background: linear-gradient(135deg, #28a745, #20c997); padding: 20px; text-align: center; border-radius: 10px 10px 0 0;'>
            <h2 style='color: white; margin: 0;'>✅ Leave Approved!</h2>
        </div>
        <div style='padding: 25px; background: #f8f9fa; border: 1px solid #e9ecef;'>
            <p>Hello <strong>{employeeName}</strong>,</p>
            <p>Great news! Your leave request has been <strong style='color: #28a745;'>APPROVED</strong>.</p>
            <table style='width: 100%; border-collapse: collapse; margin: 15px 0;'>
                <tr style='border-bottom: 1px solid #dee2e6;'>
                    <td style='padding: 10px; font-weight: bold;'>Leave Type:</td>
                    <td style='padding: 10px;'>{leaveType}</td>
                </tr>
                <tr style='border-bottom: 1px solid #dee2e6;'>
                    <td style='padding: 10px; font-weight: bold;'>Duration:</td>
                    <td style='padding: 10px;'>{startDate:dd MMM yyyy} - {endDate:dd MMM yyyy} ({totalDays} days)</td>
                </tr>
                {(string.IsNullOrEmpty(remarks) ? "" : $@"
                <tr>
                    <td style='padding: 10px; font-weight: bold;'>Remarks:</td>
                    <td style='padding: 10px;'>{remarks}</td>
                </tr>")}
            </table>
            <p>Enjoy your time off! 🎉</p>
        </div>
        <div style='padding: 15px; text-align: center; font-size: 12px; color: #6c757d; border-radius: 0 0 10px 10px; background: #e9ecef;'>
            Employee Management System &copy; {DateTime.Now.Year}
        </div>
    </body>
    </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task SendLeaveRejectedNotification(string employeeEmail, string employeeName,
            string leaveType, DateTime startDate, DateTime endDate, string? remarks)
        {
            var subject = $"❌ Leave Rejected - {leaveType}";
            var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
        <div style='background: linear-gradient(135deg, #dc3545, #c0392b); padding: 20px; text-align: center; border-radius: 10px 10px 0 0;'>
            <h2 style='color: white; margin: 0;'>❌ Leave Rejected</h2>
        </div>
        <div style='padding: 25px; background: #f8f9fa; border: 1px solid #e9ecef;'>
            <p>Hello <strong>{employeeName}</strong>,</p>
            <p>Unfortunately, your leave request has been <strong style='color: #dc3545;'>REJECTED</strong>.</p>
            <table style='width: 100%; border-collapse: collapse; margin: 15px 0;'>
                <tr style='border-bottom: 1px solid #dee2e6;'>
                    <td style='padding: 10px; font-weight: bold;'>Leave Type:</td>
                    <td style='padding: 10px;'>{leaveType}</td>
                </tr>
                <tr style='border-bottom: 1px solid #dee2e6;'>
                    <td style='padding: 10px; font-weight: bold;'>Duration:</td>
                    <td style='padding: 10px;'>{startDate:dd MMM yyyy} - {endDate:dd MMM yyyy}</td>
                </tr>
                {(string.IsNullOrEmpty(remarks) ? "" : $@"
                <tr>
                    <td style='padding: 10px; font-weight: bold;'>Reason:</td>
                    <td style='padding: 10px;'>{remarks}</td>
                </tr>")}
            </table>
            <p>Please contact your manager for more details.</p>
        </div>
        <div style='padding: 15px; text-align: center; font-size: 12px; color: #6c757d; border-radius: 0 0 10px 10px; background: #e9ecef;'>
            Employee Management System &copy; {DateTime.Now.Year}
        </div>
    </body>
    </html>";

            await SendEmailAsync(employeeEmail, subject, body);
        }

        public async Task SendLeaveCancelledNotification(string managerEmail, string employeeName,
            string leaveType, DateTime startDate, DateTime endDate, string? cancelReason)
        {
            var subject = $"⚠️ Leave Cancelled - {employeeName}";
            var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
        <div style='background: linear-gradient(135deg, #6c757d, #495057); padding: 20px; text-align: center; border-radius: 10px 10px 0 0;'>
            <h2 style='color: white; margin: 0;'>⚠️ Leave Cancelled</h2>
        </div>
        <div style='padding: 25px; background: #f8f9fa; border: 1px solid #e9ecef;'>
            <p>Hello,</p>
            <p><strong>{employeeName}</strong> has cancelled their leave request:</p>
            <table style='width: 100%; border-collapse: collapse; margin: 15px 0;'>
                <tr style='border-bottom: 1px solid #dee2e6;'>
                    <td style='padding: 10px; font-weight: bold;'>Leave Type:</td>
                    <td style='padding: 10px;'>{leaveType}</td>
                </tr>
                <tr style='border-bottom: 1px solid #dee2e6;'>
                    <td style='padding: 10px; font-weight: bold;'>Duration:</td>
                    <td style='padding: 10px;'>{startDate:dd MMM yyyy} - {endDate:dd MMM yyyy}</td>
                </tr>
                {(string.IsNullOrEmpty(cancelReason) ? "" : $@"
                <tr>
                    <td style='padding: 10px; font-weight: bold;'>Cancel Reason:</td>
                    <td style='padding: 10px;'>{cancelReason}</td>
                </tr>")}
            </table>
        </div>
    </body>
    </html>";

            await SendEmailAsync(managerEmail, subject, body);
        }
        public async Task<bool> SendEmailWithAttachmentAsync(string toEmail, string subject, string htmlBody, byte[] fileBytes, string fileName)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = _emailSettings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                // 🌟 MAIN MAGIC: Attach PDF from Byte Array
                if (fileBytes != null && fileBytes.Length > 0)
                {
                    var memoryStream = new MemoryStream(fileBytes);
                    var attachment = new Attachment(memoryStream, fileName, "application/pdf");
                    mailMessage.Attachments.Add(attachment);
                }

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email with attachment sent successfully to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email with attachment to {Email}", toEmail);
                return false;
            }
        }
        public async Task SendLeaveAppliedNotificationAsync( List<string> recipientEmails, string employeeName, string leaveType, DateTime startDate, DateTime endDate, decimal totalDays, string reason, int requestId)
        {
            var subject = $"🔔 New Leave Request - {employeeName} (ID: {requestId})";

            // Professional HTML Body
            var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
        <div style='background: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 10px;'>
            <h3 style='color: #dc3545;'>New Leave Application Received</h3>
            <p>Hello Admin/HR Team,</p>
            <p><strong>{employeeName}</strong> has applied for leave.</p>
            <table style='width: 100%; border-collapse: collapse; margin: 15px 0;'>
                <tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'><strong>Request ID:</strong></td><td style='padding: 8px;'>#{requestId}</td></tr>
                <tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'><strong>Type:</strong></td><td style='padding: 8px;'>{leaveType}</td></tr>
                <tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'><strong>Duration:</strong></td><td style='padding: 8px;'>{startDate:dd-MMM-yyyy} to {endDate:dd-MMM-yyyy}</td></tr>
                <tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'><strong>Total Days:</strong></td><td style='padding: 8px;'>{totalDays}</td></tr>
                <tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'><strong>Reason:</strong></td><td style='padding: 8px;'>{reason}</td></tr>
            </table>
            <div style='text-align: center; margin-top: 20px;'>
                <a href='https://localhost:44354/Leave/Details/{requestId}' 
                   style='background: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                   View & Approve Request
                </a>
            </div>
        </div>
    </body>
    </html>";

            foreach (var email in recipientEmails)
            {
                await SendEmailAsync(email, subject, body);
            }
        }
        #region ===== TICKET EMAIL NOTIFICATIONS =====

        /// <summary>
        /// 🎫 Send notification when a new ticket is created
        /// </summary>
        public async Task SendTicketCreatedNotificationAsync(
            List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string description,
            string ticketType, string priority, string createdByName, DateTime? dueDate)
        {
            try
            {
                var subject = $"🎫 New Ticket Created: {ticketNumber} - {title}";

                foreach (var recipient in recipients)
                {
                    var body = BuildTicketCreatedBody(recipient.FullName, ticketId, ticketNumber,
                        title, description, ticketType, priority, createdByName, dueDate);

                    await SendEmailAsync(recipient.Email, subject, body);
                }

                _logger.LogInformation("Ticket created notifications sent to {Count} recipients for {TicketNumber}",
                    recipients.Count, ticketNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send ticket created notifications");
            }
        }

        /// <summary>
        /// 👤 Send notification when ticket is assigned
        /// </summary>
        public async Task SendTicketAssignedNotificationAsync(
            List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string priority,
            string assignedByName, string assignedToName, DateTime? dueDate)
        {
            try
            {
                var subject = $"👤 Ticket Assigned: {ticketNumber} - {title}";

                foreach (var recipient in recipients)
                {
                    var body = BuildTicketAssignedBody(recipient.FullName, ticketId, ticketNumber,
                        title, priority, assignedByName, assignedToName, dueDate);

                    await SendEmailAsync(recipient.Email, subject, body);
                }

                _logger.LogInformation("Ticket assignment notifications sent for {TicketNumber}", ticketNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send assignment notifications");
            }
        }

        /// <summary>
        /// 🔄 Send notification when ticket status changes
        /// </summary>
        public async Task SendTicketStatusChangedNotificationAsync(
            List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string oldStatus,
            string newStatus, string changedByName, string? remarks)
        {
            try
            {
                var subject = $"🔄 Status Changed: {ticketNumber} → {newStatus}";

                foreach (var recipient in recipients)
                {
                    var body = BuildTicketStatusChangedBody(recipient.FullName, ticketId, ticketNumber,
                        title, oldStatus, newStatus, changedByName, remarks);

                    await SendEmailAsync(recipient.Email, subject, body);
                }

                _logger.LogInformation("Status change notifications sent for {TicketNumber}: {OldStatus} -> {NewStatus}",
                    ticketNumber, oldStatus, newStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send status change notifications");
            }
        }

        /// <summary>
        /// 💬 Send notification when comment is added
        /// </summary>
        public async Task SendTicketCommentAddedNotificationAsync(
            List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string commentByName,
            string comment, bool isInternal)
        {
            try
            {
                var subject = $"💬 New Comment: {ticketNumber} - {title}";

                // Filter out non-internal users if comment is internal
                var filteredRecipients = isInternal
                    ? recipients.Where(r => r.NotificationType != "Creator" || r.NotificationType == "Assignee").ToList()
                    : recipients;

                foreach (var recipient in filteredRecipients)
                {
                    var body = BuildTicketCommentBody(recipient.FullName, ticketId, ticketNumber,
                        title, commentByName, comment, isInternal);

                    await SendEmailAsync(recipient.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send comment notifications");
            }
        }

        /// <summary>
        /// 🧪 Send notification when ticket is ready for QA
        /// </summary>
        public async Task SendTicketReadyForQANotificationAsync(
            List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string description,
            string priority, string developerName)
        {
            try
            {
                var subject = $"🧪 Ready for QA Testing: {ticketNumber} - {title}";

                foreach (var recipient in recipients)
                {
                    var body = BuildTicketReadyForQABody(recipient.FullName, ticketId, ticketNumber,
                        title, description, priority, developerName);

                    await SendEmailAsync(recipient.Email, subject, body);
                }

                _logger.LogInformation("QA notifications sent for {TicketNumber}", ticketNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send QA notifications");
            }
        }

        /// <summary>
        /// ✅ Send notification when ticket is closed
        /// </summary>
        public async Task SendTicketClosedNotificationAsync(
            List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string closedByName,
            string? remarks, int resolutionMinutes)
        {
            try
            {
                var subject = $"✅ Ticket Closed: {ticketNumber} - {title}";

                foreach (var recipient in recipients)
                {
                    var body = BuildTicketClosedBody(recipient.FullName, ticketId, ticketNumber,
                        title, closedByName, remarks, resolutionMinutes);

                    await SendEmailAsync(recipient.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send closed notifications");
            }
        }

        /// <summary>
        /// 🔁 Send notification when ticket is reopened
        /// </summary>
        public async Task SendTicketReopenedNotificationAsync(
            List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string reopenedByName, string? reason)
        {
            try
            {
                var subject = $"🔁 Ticket Reopened: {ticketNumber} - {title}";

                foreach (var recipient in recipients)
                {
                    var body = BuildTicketReopenedBody(recipient.FullName, ticketId, ticketNumber,
                        title, reopenedByName, reason);

                    await SendEmailAsync(recipient.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reopened notifications");
            }
        }

        /// <summary>
        /// ⚠️ Send notification when ticket is overdue
        /// </summary>
        public async Task SendTicketOverdueNotificationAsync(
            List<TicketEmailRecipient> recipients,
            int ticketId, string ticketNumber, string title, string priority, DateTime dueDate)
        {
            try
            {
                var subject = $"⚠️ OVERDUE: {ticketNumber} - {title}";

                foreach (var recipient in recipients)
                {
                    var body = BuildTicketOverdueBody(recipient.FullName, ticketId, ticketNumber,
                        title, priority, dueDate);

                    await SendEmailAsync(recipient.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send overdue notifications");
            }
        }

        #endregion

        #region ===== TICKET EMAIL TEMPLATES =====

        private string BuildTicketCreatedBody(string recipientName, int ticketId, string ticketNumber,
            string title, string description, string ticketType, string priority, string createdByName, DateTime? dueDate)
        {
            var priorityColor = GetPriorityColor(priority);
            var ticketUrl = GetTicketUrl(ticketId);

            return $@"
<!DOCTYPE html>
<html>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: #f4f6f9;'>
    <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1);'>
        <div style='background: linear-gradient(135deg, #667eea, #764ba2); color: white; padding: 30px; text-align: center;'>
            <h1 style='margin: 0; font-size: 26px;'>🎫 New Ticket Created</h1>
            <p style='margin: 8px 0 0; opacity: 0.9; font-family: monospace; font-size: 14px;'>{ticketNumber}</p>
        </div>

        <div style='padding: 30px;'>
            <p style='font-size: 16px;'>Hi <strong>{recipientName}</strong>,</p>
            <p>A new ticket has been created and requires your attention.</p>

            <div style='background: #f8fafc; padding: 20px; border-radius: 10px; border-left: 4px solid #667eea; margin: 20px 0;'>
                <h2 style='margin: 0 0 12px; color: #1e293b; font-size: 18px;'>{title}</h2>
                <p style='color: #64748b; margin: 10px 0; line-height: 1.6;'>{TruncateText(description, 200)}</p>

                <table style='width: 100%; margin-top: 15px; font-size: 14px; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 8px 0; color: #64748b; width: 35%;'><strong>📌 Type:</strong></td>
                        <td style='padding: 8px 0;'>
                            <span style='background: #dbeafe; color: #1e40af; padding: 4px 12px; border-radius: 12px; font-weight: 600;'>{ticketType}</span>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 0; color: #64748b;'><strong>🚩 Priority:</strong></td>
                        <td style='padding: 8px 0;'>
                            <span style='background: {priorityColor}22; color: {priorityColor}; padding: 4px 12px; border-radius: 12px; font-weight: 600;'>{priority}</span>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 0; color: #64748b;'><strong>👤 Created By:</strong></td>
                        <td style='padding: 8px 0; color: #334155;'>{createdByName}</td>
                    </tr>
                    {(dueDate.HasValue ? $@"
                    <tr>
                        <td style='padding: 8px 0; color: #64748b;'><strong>📅 Due Date:</strong></td>
                        <td style='padding: 8px 0; color: #dc2626; font-weight: 600;'>{dueDate.Value:dd MMM yyyy}</td>
                    </tr>" : "")}
                </table>
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{ticketUrl}' style='background: linear-gradient(135deg, #667eea, #764ba2); color: white; padding: 14px 32px; border-radius: 8px; text-decoration: none; font-weight: 600; display: inline-block; box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);'>
                    📋 View Ticket Details
                </a>
            </div>
        </div>

        <div style='background: #f8fafc; padding: 20px; text-align: center; border-top: 1px solid #e2e8f0; color: #64748b; font-size: 12px;'>
            <p style='margin: 0;'>📧 Automated notification from HRMS Ticket System</p>
            <p style='margin: 5px 0 0;'>© {DateTime.Now.Year} Employee Management System</p>
        </div>
    </div>
</body>
</html>";
        }

        private string BuildTicketAssignedBody(string recipientName, int ticketId, string ticketNumber,
            string title, string priority, string assignedByName, string assignedToName, DateTime? dueDate)
        {
            var priorityColor = GetPriorityColor(priority);
            var ticketUrl = GetTicketUrl(ticketId);

            return $@"
<!DOCTYPE html>
<html>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: #f4f6f9;'>
    <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1);'>
        <div style='background: linear-gradient(135deg, #fa709a, #fee140); color: white; padding: 30px; text-align: center;'>
            <h1 style='margin: 0; font-size: 26px;'>👤 Ticket Assigned</h1>
            <p style='margin: 8px 0 0; opacity: 0.9; font-family: monospace;'>{ticketNumber}</p>
        </div>

        <div style='padding: 30px;'>
            <p style='font-size: 16px;'>Hi <strong>{recipientName}</strong>,</p>
            <p><strong>{assignedByName}</strong> has assigned a ticket to <strong>{assignedToName}</strong>.</p>

            <div style='background: linear-gradient(135deg, #fef3c7, #fef9c3); padding: 20px; border-radius: 10px; border-left: 4px solid #f59e0b; margin: 20px 0;'>
                <h3 style='margin: 0 0 10px; color: #92400e;'>{title}</h3>
                <p style='margin: 10px 0; color: #78350f;'>
                    <strong>🚩 Priority:</strong>
                    <span style='background: {priorityColor}22; color: {priorityColor}; padding: 3px 10px; border-radius: 10px; font-weight: 600;'>{priority}</span>
                </p>
                {(dueDate.HasValue ? $@"<p style='margin: 10px 0; color: #92400e;'><strong>📅 Due:</strong> {dueDate.Value:dd MMM yyyy}</p>" : "")}
            </div>

            <div style='background: #e0f2fe; padding: 15px; border-radius: 8px; margin: 20px 0; border-left: 3px solid #0284c7;'>
                <p style='margin: 0; color: #075985; font-size: 14px;'>
                    💡 <strong>Action Required:</strong> Please review the ticket and start working on it.
                </p>
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{ticketUrl}' style='background: linear-gradient(135deg, #fa709a, #fee140); color: white; padding: 14px 32px; border-radius: 8px; text-decoration: none; font-weight: 600; display: inline-block;'>
                    🔍 Start Working
                </a>
            </div>
        </div>

        <div style='background: #f8fafc; padding: 20px; text-align: center; font-size: 12px; color: #64748b;'>
            HRMS Ticket System © {DateTime.Now.Year}
        </div>
    </div>
</body>
</html>";
        }

        private string BuildTicketStatusChangedBody(string recipientName, int ticketId, string ticketNumber,
            string title, string oldStatus, string newStatus, string changedByName, string? remarks)
        {
            var ticketUrl = GetTicketUrl(ticketId);
            var statusColor = GetStatusGradient(newStatus);

            return $@"
<!DOCTYPE html>
<html>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: #f4f6f9;'>
    <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1);'>
        <div style='background: {statusColor}; color: white; padding: 30px; text-align: center;'>
            <h1 style='margin: 0; font-size: 26px;'>🔄 Status Updated</h1>
            <p style='margin: 8px 0 0; opacity: 0.9; font-family: monospace;'>{ticketNumber}</p>
        </div>

        <div style='padding: 30px;'>
            <p style='font-size: 16px;'>Hi <strong>{recipientName}</strong>,</p>
            <p>The status of a ticket has been updated by <strong>{changedByName}</strong>.</p>

            <div style='text-align: center; margin: 25px 0; padding: 20px; background: #f8fafc; border-radius: 10px;'>
                <span style='background: #fee2e2; color: #991b1b; padding: 10px 20px; border-radius: 25px; font-weight: 700; display: inline-block; margin-right: 10px;'>
                    {oldStatus}
                </span>
                <span style='font-size: 24px; color: #64748b; margin: 0 10px;'>→</span>
                <span style='background: #d1fae5; color: #065f46; padding: 10px 20px; border-radius: 25px; font-weight: 700; display: inline-block;'>
                    {newStatus}
                </span>
            </div>

            <div style='background: #f8fafc; padding: 20px; border-radius: 10px; margin: 20px 0;'>
                <h3 style='margin: 0 0 10px; color: #1e293b;'>{title}</h3>
                {(!string.IsNullOrEmpty(remarks) ? $@"
                <div style='background: white; padding: 15px; border-radius: 8px; border-left: 4px solid #667eea; margin-top: 15px;'>
                    <strong style='color: #475569;'>📝 Remarks:</strong>
                    <p style='margin: 5px 0 0; color: #334155; font-style: italic;'>""{remarks}""</p>
                </div>" : "")}
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{ticketUrl}' style='background: {statusColor}; color: white; padding: 14px 32px; border-radius: 8px; text-decoration: none; font-weight: 600; display: inline-block;'>
                    👁️ View Ticket
                </a>
            </div>
        </div>

        <div style='background: #f8fafc; padding: 20px; text-align: center; font-size: 12px; color: #64748b;'>
            HRMS Ticket System © {DateTime.Now.Year}
        </div>
    </div>
</body>
</html>";
        }

        private string BuildTicketCommentBody(string recipientName, int ticketId, string ticketNumber,
            string title, string commentByName, string comment, bool isInternal)
        {
            var ticketUrl = GetTicketUrl(ticketId);
            var headerColor = isInternal
                ? "linear-gradient(135deg, #f7971e, #ffd200)"
                : "linear-gradient(135deg, #4facfe, #00f2fe)";
            var internalBadge = isInternal
                ? "<span style='background: #fbbf24; color: #78350f; padding: 3px 10px; border-radius: 10px; font-size: 12px; font-weight: 700;'>🔒 INTERNAL</span>"
                : "";

            return $@"
<!DOCTYPE html>
<html>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: #f4f6f9;'>
    <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden;'>
        <div style='background: {headerColor}; color: white; padding: 25px; text-align: center;'>
            <h1 style='margin: 0; font-size: 24px;'>💬 New Comment Added</h1>
            <p style='margin: 8px 0 0; opacity: 0.9; font-family: monospace;'>{ticketNumber}</p>
        </div>

        <div style='padding: 30px;'>
            <p style='font-size: 16px;'>Hi <strong>{recipientName}</strong>,</p>
            <p><strong>{commentByName}</strong> added a new comment {internalBadge}</p>

            <div style='background: #f8fafc; padding: 20px; border-radius: 10px; margin: 20px 0;'>
                <h4 style='margin: 0 0 15px; color: #1e293b;'>{title}</h4>

                <div style='background: white; padding: 18px; border-radius: 8px; border-left: 4px solid {(isInternal ? "#f59e0b" : "#4facfe")};'>
                    <div style='display: flex; align-items: center; margin-bottom: 10px;'>
                        <div style='width: 36px; height: 36px; border-radius: 50%; background: linear-gradient(135deg, #667eea, #764ba2); color: white; display: inline-flex; align-items: center; justify-content: center; font-weight: 700; margin-right: 10px;'>
                            {commentByName.Substring(0, 1).ToUpper()}
                        </div>
                        <strong style='color: #334155;'>{commentByName}</strong>
                    </div>
                    <p style='margin: 0; color: #475569; line-height: 1.6;'>""{comment}""</p>
                </div>
            </div>

            <div style='text-align: center; margin: 25px 0;'>
                <a href='{ticketUrl}' style='background: {headerColor}; color: white; padding: 12px 28px; border-radius: 8px; text-decoration: none; font-weight: 600;'>
                    💭 Join Discussion
                </a>
            </div>
        </div>

        <div style='background: #f8fafc; padding: 20px; text-align: center; font-size: 12px; color: #64748b;'>
            HRMS Ticket System © {DateTime.Now.Year}
        </div>
    </div>
</body>
</html>";
        }

        private string BuildTicketReadyForQABody(string recipientName, int ticketId, string ticketNumber,
            string title, string description, string priority, string developerName)
        {
            var ticketUrl = GetTicketUrl(ticketId);
            var priorityColor = GetPriorityColor(priority);

            return $@"
<!DOCTYPE html>
<html>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: #f4f6f9;'>
    <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden;'>
        <div style='background: linear-gradient(135deg, #a18cd1, #fbc2eb); color: white; padding: 30px; text-align: center;'>
            <h1 style='margin: 0; font-size: 26px;'>🧪 Ready for QA Testing</h1>
            <p style='margin: 8px 0 0; opacity: 0.9; font-family: monospace;'>{ticketNumber}</p>
        </div>

        <div style='padding: 30px;'>
            <p style='font-size: 16px;'>Hi <strong>{recipientName}</strong>,</p>
            <p>A ticket has been marked as <strong style='color: #7c3aed;'>Ready for QA Testing</strong> by <strong>{developerName}</strong>.</p>

            <div style='background: linear-gradient(135deg, #ede9fe, #f3e8ff); padding: 20px; border-radius: 10px; border-left: 4px solid #7c3aed; margin: 20px 0;'>
                <h3 style='margin: 0 0 10px; color: #5b21b6;'>{title}</h3>
                <p style='color: #6b21a8; margin: 10px 0;'>{TruncateText(description, 150)}</p>

                <table style='width: 100%; margin-top: 15px; font-size: 14px;'>
                    <tr>
                        <td style='padding: 6px 0; color: #6b21a8;'><strong>👨‍💻 Developer:</strong></td>
                        <td style='padding: 6px 0;'>{developerName}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; color: #6b21a8;'><strong>🚩 Priority:</strong></td>
                        <td style='padding: 6px 0;'>
                            <span style='background: {priorityColor}22; color: {priorityColor}; padding: 4px 12px; border-radius: 12px; font-weight: 600;'>{priority}</span>
                        </td>
                    </tr>
                </table>
            </div>

            <div style='background: #fef3c7; padding: 15px; border-radius: 8px; margin: 20px 0; border-left: 3px solid #f59e0b;'>
                <p style='margin: 0; color: #78350f; font-size: 14px;'>
                    🎯 <strong>QA Action Required:</strong> Please test thoroughly and update the ticket status.
                </p>
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{ticketUrl}' style='background: linear-gradient(135deg, #a18cd1, #fbc2eb); color: white; padding: 14px 32px; border-radius: 8px; text-decoration: none; font-weight: 600; display: inline-block;'>
                    🧪 Start Testing
                </a>
            </div>
        </div>

        <div style='background: #f8fafc; padding: 20px; text-align: center; font-size: 12px; color: #64748b;'>
            HRMS Ticket System © {DateTime.Now.Year}
        </div>
    </div>
</body>
</html>";
        }

        private string BuildTicketClosedBody(string recipientName, int ticketId, string ticketNumber,
            string title, string closedByName, string? remarks, int resolutionMinutes)
        {
            var ticketUrl = GetTicketUrl(ticketId);
            var resolutionTime = FormatResolutionTime(resolutionMinutes);

            return $@"
<!DOCTYPE html>
<html>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: #f4f6f9;'>
    <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden;'>
        <div style='background: linear-gradient(135deg, #11998e, #38ef7d); color: white; padding: 30px; text-align: center;'>
            <h1 style='margin: 0; font-size: 26px;'>✅ Ticket Closed</h1>
            <p style='margin: 8px 0 0; opacity: 0.9; font-family: monospace;'>{ticketNumber}</p>
        </div>

        <div style='padding: 30px;'>
            <p style='font-size: 16px;'>Hi <strong>{recipientName}</strong>,</p>
            <p>The following ticket has been successfully closed by <strong>{closedByName}</strong>.</p>

            <div style='background: linear-gradient(135deg, #d1fae5, #ecfdf5); padding: 20px; border-radius: 10px; border-left: 4px solid #10b981; margin: 20px 0;'>
                <h3 style='margin: 0 0 10px; color: #065f46;'>✓ {title}</h3>

                <div style='background: white; padding: 15px; border-radius: 8px; margin-top: 15px;'>
                    <p style='margin: 5px 0; color: #047857;'>
                        <strong>⏱️ Resolution Time:</strong> {resolutionTime}
                    </p>
                    <p style='margin: 5px 0; color: #047857;'>
                        <strong>👤 Closed By:</strong> {closedByName}
                    </p>
                    <p style='margin: 5px 0; color: #047857;'>
                        <strong>📅 Closed On:</strong> {DateTime.Now:dd MMM yyyy hh:mm tt}
                    </p>
                </div>

                {(!string.IsNullOrEmpty(remarks) ? $@"
                <div style='background: white; padding: 15px; border-radius: 8px; margin-top: 10px; border-left: 3px solid #10b981;'>
                    <strong style='color: #065f46;'>📝 Closing Remarks:</strong>
                    <p style='margin: 5px 0 0; color: #047857; font-style: italic;'>""{remarks}""</p>
                </div>" : "")}
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{ticketUrl}' style='background: linear-gradient(135deg, #11998e, #38ef7d); color: white; padding: 12px 28px; border-radius: 8px; text-decoration: none; font-weight: 600;'>
                    📋 View Details
                </a>
            </div>
        </div>

        <div style='background: #f8fafc; padding: 20px; text-align: center; font-size: 12px; color: #64748b;'>
            HRMS Ticket System © {DateTime.Now.Year}
        </div>
    </div>
</body>
</html>";
        }

        private string BuildTicketReopenedBody(string recipientName, int ticketId, string ticketNumber,
            string title, string reopenedByName, string? reason)
        {
            var ticketUrl = GetTicketUrl(ticketId);

            return $@"
<!DOCTYPE html>
<html>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: #f4f6f9;'>
    <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden;'>
        <div style='background: linear-gradient(135deg, #eb3349, #f45c43); color: white; padding: 30px; text-align: center;'>
            <h1 style='margin: 0; font-size: 26px;'>🔁 Ticket Reopened</h1>
            <p style='margin: 8px 0 0; opacity: 0.9; font-family: monospace;'>{ticketNumber}</p>
        </div>

        <div style='padding: 30px;'>
            <p style='font-size: 16px;'>Hi <strong>{recipientName}</strong>,</p>
            <p><strong>{reopenedByName}</strong> has reopened a ticket that requires your attention.</p>

            <div style='background: linear-gradient(135deg, #fee2e2, #fef2f2); padding: 20px; border-radius: 10px; border-left: 4px solid #ef4444; margin: 20px 0;'>
                <h3 style='margin: 0 0 10px; color: #991b1b;'>{title}</h3>
                {(!string.IsNullOrEmpty(reason) ? $@"
                <div style='background: white; padding: 15px; border-radius: 8px; margin-top: 15px; border-left: 3px solid #ef4444;'>
                    <strong style='color: #991b1b;'>📝 Reason for Reopening:</strong>
                    <p style='margin: 5px 0 0; color: #7f1d1d;'>""{reason}""</p>
                </div>" : "")}
            </div>

            <div style='background: #fef3c7; padding: 15px; border-radius: 8px; margin: 20px 0; border-left: 3px solid #f59e0b;'>
                <p style='margin: 0; color: #78350f; font-size: 14px;'>
                    ⚠️ <strong>Urgent Action Required:</strong> Please review and resolve this issue.
                </p>
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{ticketUrl}' style='background: linear-gradient(135deg, #eb3349, #f45c43); color: white; padding: 14px 32px; border-radius: 8px; text-decoration: none; font-weight: 600; display: inline-block;'>
                    🔧 Take Action
                </a>
            </div>
        </div>

        <div style='background: #f8fafc; padding: 20px; text-align: center; font-size: 12px; color: #64748b;'>
            HRMS Ticket System © {DateTime.Now.Year}
        </div>
    </div>
</body>
</html>";
        }

        private string BuildTicketOverdueBody(string recipientName, int ticketId, string ticketNumber,
            string title, string priority, DateTime dueDate)
        {
            var ticketUrl = GetTicketUrl(ticketId);
            var daysOverdue = (DateTime.Now - dueDate).Days;
            var priorityColor = GetPriorityColor(priority);

            return $@"
<!DOCTYPE html>
<html>
<body style='margin: 0; padding: 20px; font-family: Arial, sans-serif; background: #f4f6f9;'>
    <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; border: 3px solid #ef4444;'>
        <div style='background: linear-gradient(135deg, #dc2626, #ef4444); color: white; padding: 30px; text-align: center;'>
            <h1 style='margin: 0; font-size: 28px;'>⚠️ OVERDUE ALERT</h1>
            <p style='margin: 8px 0 0; opacity: 0.95; font-family: monospace; font-size: 14px;'>{ticketNumber}</p>
        </div>

        <div style='padding: 30px;'>
            <p style='font-size: 16px;'>Hi <strong>{recipientName}</strong>,</p>
            <p style='color: #dc2626; font-weight: 600;'>⏰ This ticket is now <strong>{daysOverdue} day(s) overdue</strong>!</p>

            <div style='background: linear-gradient(135deg, #fee2e2, #fef2f2); padding: 20px; border-radius: 10px; border: 2px solid #ef4444; margin: 20px 0;'>
                <h3 style='margin: 0 0 15px; color: #991b1b;'>{title}</h3>

                <table style='width: 100%; font-size: 14px;'>
                    <tr>
                        <td style='padding: 8px 0; color: #991b1b;'><strong>🚩 Priority:</strong></td>
                        <td style='padding: 8px 0;'>
                            <span style='background: {priorityColor}22; color: {priorityColor}; padding: 4px 12px; border-radius: 12px; font-weight: 700;'>{priority}</span>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 0; color: #991b1b;'><strong>📅 Due Date:</strong></td>
                        <td style='padding: 8px 0; color: #dc2626; font-weight: 700;'>{dueDate:dd MMM yyyy}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 0; color: #991b1b;'><strong>⏱️ Overdue By:</strong></td>
                        <td style='padding: 8px 0; color: #dc2626; font-weight: 700;'>{daysOverdue} day(s)</td>
                    </tr>
                </table>
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{ticketUrl}' style='background: linear-gradient(135deg, #dc2626, #ef4444); color: white; padding: 15px 35px; border-radius: 8px; text-decoration: none; font-weight: 700; display: inline-block; box-shadow: 0 4px 15px rgba(220, 38, 38, 0.4);'>
                    🚨 Resolve Now
                </a>
            </div>
        </div>

        <div style='background: #f8fafc; padding: 20px; text-align: center; font-size: 12px; color: #64748b;'>
            HRMS Ticket System © {DateTime.Now.Year}
        </div>
    </div>
</body>
</html>";
        }

        #endregion

        #region ===== HELPER METHODS =====

        private string GetTicketUrl(int ticketId)
        {
            // ✅ Update this to your actual UI URL
            var baseUrl = _emailSettings.AppBaseUrl ?? "http://localhost:5000";
            return $"{baseUrl}/Ticket/Details/{ticketId}";
        }

        private string GetPriorityColor(string? priority) => priority?.ToLower() switch
        {
            "critical" => "#dc2626",
            "high" => "#ea580c",
            "medium" => "#ca8a04",
            "low" => "#16a34a",
            _ => "#64748b"
        };

        private string GetStatusGradient(string? status) => status?.ToLower() switch
        {
            "new" => "linear-gradient(135deg, #4facfe, #00f2fe)",
            "assigned" => "linear-gradient(135deg, #667eea, #764ba2)",
            "inprogress" => "linear-gradient(135deg, #f7971e, #ffd200)",
            "onhold" => "linear-gradient(135deg, #6c757d, #495057)",
            "fixedbydev" => "linear-gradient(135deg, #a18cd1, #fbc2eb)",
            "readyforqa" => "linear-gradient(135deg, #667eea, #764ba2)",
            "intesting" => "linear-gradient(135deg, #2193b0, #6dd5ed)",
            "resolved" => "linear-gradient(135deg, #11998e, #38ef7d)",
            "closed" => "linear-gradient(135deg, #2c3e50, #4a5568)",
            "reopened" => "linear-gradient(135deg, #eb3349, #f45c43)",
            _ => "linear-gradient(135deg, #667eea, #764ba2)"
        };

        private string TruncateText(string? text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
        }

        private string FormatResolutionTime(int minutes)
        {
            if (minutes < 60) return $"{minutes} minute(s)";
            if (minutes < 1440) return $"{minutes / 60} hour(s)";
            return $"{minutes / 1440} day(s)";
        }

        #endregion
    }
}
