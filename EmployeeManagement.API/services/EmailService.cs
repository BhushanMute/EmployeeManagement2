using EmployeeManagement.API.Models;
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
    }
}
