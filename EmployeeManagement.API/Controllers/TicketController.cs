// File: EmployeeManagement.API/Controllers/TicketController.cs
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models.Ticket;
using EmployeeManagement.API.Repositories.Ticket;
using EmployeeManagement.API.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<TicketController> _logger;
        private readonly IWebHostEnvironment _environment;

        public TicketController(
            ITicketRepository ticketRepo,
            IEmailService emailService,
            ILogger<TicketController> logger,
            IWebHostEnvironment environment)
        {
            _ticketRepo = ticketRepo;
            _emailService = emailService;
            _logger = logger;
            _environment = environment;
        }

        #region HELPERS

        private int UserId =>
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        private string UserRole =>
            User.FindFirst(ClaimTypes.Role)?.Value ?? "Employee";

        private string UserName =>
            User.FindFirst(ClaimTypes.Name)?.Value ??
            User.FindFirst("FullName")?.Value ?? "User";

        private string UserEmail =>
            User.FindFirst(ClaimTypes.Email)?.Value ?? "";

        #endregion

        #region TICKET CRUD

        /// <summary>
        /// Create new ticket
        /// POST: api/Ticket/create
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<object>.Fail("Validation failed", errors));
                }

                if (UserId == 0)
                    return Unauthorized(ApiResponse<object>.Fail("Not authenticated"));

                var result = await _ticketRepo.CreateTicketAsync(request, UserId);

                // Send email notifications (fire and forget)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var recipients = await _ticketRepo.GetEmailRecipientsAsync(result.TicketId, "Created");
                        if (recipients.Any())
                        {
                            await _emailService.SendTicketCreatedNotificationAsync(
                                recipients, result.TicketId, result.TicketNumber,
                                request.Title, request.Description, request.TicketType,
                                request.Priority, UserName, request.DueDate);
                        }

                        if (request.AssignedTo.HasValue)
                        {
                            var assignRecipients = await _ticketRepo.GetEmailRecipientsAsync(result.TicketId, "Assigned");
                            if (assignRecipients.Any())
                            {
                                await _emailService.SendTicketAssignedNotificationAsync(
                                    assignRecipients, result.TicketId, result.TicketNumber,
                                    request.Title, request.Priority, UserName, "Developer", request.DueDate);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Email notification failed for ticket {TicketId}", result.TicketId);
                    }
                });

                return Ok(ApiResponse<CreateTicketResponse>.Success(result, "Ticket created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ticket");
                return StatusCode(500, ApiResponse<object>.Fail("Error creating ticket: " + ex.Message));
            }
        }

        /// <summary>
        /// Get ticket by ID with comments, attachments, history
        /// GET: api/Ticket/{id}
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            try
            {
                var ticket = await _ticketRepo.GetTicketByIdAsync(id);
                if (ticket == null)
                    return NotFound(ApiResponse<object>.Fail("Ticket not found"));

                // Load related data
                ticket.Comments = await _ticketRepo.GetTicketCommentsAsync(id);
                ticket.Attachments = await _ticketRepo.GetTicketAttachmentsAsync(id);
                ticket.History = await _ticketRepo.GetTicketHistoryAsync(id);

                return Ok(ApiResponse<Models.Ticket.Ticket>.Success(ticket));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ticket {Id}", id);
                return StatusCode(500, ApiResponse<object>.Fail("Error fetching ticket"));
            }
        }

        /// <summary>
        /// Get tickets list with filters and pagination
        /// GET: api/Ticket/list
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetTickets([FromQuery] TicketFilterRequest filter)
        {
            try
            {
                var result = await _ticketRepo.GetTicketsAsync(filter);
                return Ok(ApiResponse<TicketListResponse>.Success(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tickets");
                return StatusCode(500, ApiResponse<object>.Fail("Error fetching tickets"));
            }
        }

        /// <summary>
        /// Get my tickets (based on role)
        /// GET: api/Ticket/my-tickets
        /// </summary>
        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets([FromQuery] string? status = null)
        {
            try
            {
                var request = new MyTicketsRequest
                {
                    UserId = UserId,
                    UserRole = UserRole,
                    Status = status
                };

                var result = await _ticketRepo.GetMyTicketsAsync(request);
                return Ok(ApiResponse<List<TicketListItem>>.Success(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching my tickets");
                return StatusCode(500, ApiResponse<object>.Fail("Error fetching tickets"));
            }
        }

        /// <summary>
        /// Update ticket details
        /// PUT: api/Ticket/update
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTicket([FromBody] UpdateTicketRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.Fail("Validation failed"));

                var result = await _ticketRepo.UpdateTicketAsync(request, UserId);

                return result.Success
                    ? Ok(ApiResponse<TicketOperationResult>.Success(result, result.Message))
                    : BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ticket");
                return StatusCode(500, ApiResponse<object>.Fail("Error updating ticket"));
            }
        }

        /// <summary>
        /// Delete ticket (Admin only)
        /// DELETE: api/Ticket/{id}
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (UserRole != "Admin")
                    return StatusCode(403, ApiResponse<object>.Fail("Only Admin can delete tickets"));

                var result = await _ticketRepo.DeleteTicketAsync(id, UserId);
                return result.Success
                    ? Ok(ApiResponse<TicketOperationResult>.Success(result))
                    : BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ticket {Id}", id);
                return StatusCode(500, ApiResponse<object>.Fail("Error deleting ticket"));
            }
        }

        #endregion

        #region STATUS OPERATIONS

        /// <summary>
        /// Update ticket status with workflow validation
        /// PUT: api/Ticket/status
        /// </summary>
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTicketStatusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.Fail("Validation failed"));

                // Get current ticket for email context
                var currentTicket = await _ticketRepo.GetTicketByIdAsync(request.TicketId);
                if (currentTicket == null)
                    return NotFound(ApiResponse<object>.Fail("Ticket not found"));

                var oldStatus = currentTicket.Status;
                var result = await _ticketRepo.UpdateTicketStatusAsync(request, UserId);

                if (result.Success)
                {
                    // Send email notifications
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var eventType = request.NewStatus == "ReadyForQA" ? "ReadyForQA" : "StatusChanged";
                            var recipients = await _ticketRepo.GetEmailRecipientsAsync(request.TicketId, eventType);

                            if (recipients.Any())
                            {
                                await _emailService.SendTicketStatusChangedNotificationAsync(
                                    recipients, request.TicketId, currentTicket.TicketNumber,
                                    currentTicket.Title, oldStatus, request.NewStatus,
                                    UserName, request.Remarks);

                                if (request.NewStatus == "ReadyForQA")
                                {
                                    await _emailService.SendTicketReadyForQANotificationAsync(
                                        recipients, request.TicketId, currentTicket.TicketNumber,
                                        currentTicket.Title, currentTicket.Description,
                                        currentTicket.Priority, UserName);
                                }

                                if (request.NewStatus == "Closed")
                                {
                                    var minutes = currentTicket.ResolvedDate.HasValue
                                        ? (int)(DateTime.Now - currentTicket.CreatedDate).TotalMinutes : 0;
                                    await _emailService.SendTicketClosedNotificationAsync(
                                        recipients, request.TicketId, currentTicket.TicketNumber,
                                        currentTicket.Title, UserName, request.Remarks, minutes);
                                }

                                if (request.NewStatus == "Reopened")
                                {
                                    await _emailService.SendTicketReopenedNotificationAsync(
                                        recipients, request.TicketId, currentTicket.TicketNumber,
                                        currentTicket.Title, UserName, request.Remarks);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Email notification failed");
                        }
                    });
                }

                return result.Success
                    ? Ok(ApiResponse<TicketOperationResult>.Success(result, result.Message))
                    : BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status");
                return StatusCode(500, ApiResponse<object>.Fail("Error updating status"));
            }
        }

        #endregion

        #region ASSIGNMENT

        /// <summary>
        /// Assign ticket to developer
        /// PUT: api/Ticket/assign
        /// </summary>
        [HttpPut("assign")]
        public async Task<IActionResult> AssignTicket([FromBody] AssignTicketRequest request)
        {
            try
            {
                var result = await _ticketRepo.AssignTicketAsync(request, UserId);

                if (result.Success)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var ticket = await _ticketRepo.GetTicketByIdAsync(request.TicketId);
                            var recipients = await _ticketRepo.GetEmailRecipientsAsync(request.TicketId, "Assigned");
                            if (recipients.Any() && ticket != null)
                            {
                                await _emailService.SendTicketAssignedNotificationAsync(
                                    recipients, request.TicketId, ticket.TicketNumber,
                                    ticket.Title, ticket.Priority, UserName,
                                    ticket.AssignedToName ?? "Developer", ticket.DueDate);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Assignment email failed");
                        }
                    });
                }

                return result.Success
                    ? Ok(ApiResponse<TicketOperationResult>.Success(result, result.Message))
                    : BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning ticket");
                return StatusCode(500, ApiResponse<object>.Fail("Error assigning ticket"));
            }
        }

        #endregion

        #region COMMENTS

        /// <summary>
        /// Add comment to ticket
        /// POST: api/Ticket/comment
        /// </summary>
        [HttpPost("comment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Comment))
                    return BadRequest(ApiResponse<object>.Fail("Comment is required"));

                var result = await _ticketRepo.AddCommentAsync(request, UserId);

                if (result.CommentId > 0)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var ticket = await _ticketRepo.GetTicketByIdAsync(request.TicketId);
                            var recipients = await _ticketRepo.GetEmailRecipientsAsync(request.TicketId, "Comment");
                            // Remove self from recipients
                            recipients = recipients.Where(r => r.UserId != UserId).ToList();
                            if (recipients.Any() && ticket != null)
                            {
                                await _emailService.SendTicketCommentAddedNotificationAsync(
                                    recipients, request.TicketId, ticket.TicketNumber,
                                    ticket.Title, UserName, request.Comment,
                                    request.IsInternal);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Comment email failed");
                        }
                    });
                }

                return Ok(ApiResponse<AddCommentResponse>.Success(result, "Comment added"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment");
                return StatusCode(500, ApiResponse<object>.Fail("Error adding comment"));
            }
        }

        /// <summary>
        /// Get ticket comments
        /// GET: api/Ticket/{id}/comments
        /// </summary>
        [HttpGet("{id:int}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            try
            {
                var comments = await _ticketRepo.GetTicketCommentsAsync(id);
                return Ok(ApiResponse<List<TicketComment>>.Success(comments));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        #endregion

        #region ATTACHMENTS

        /// <summary>
        /// Upload attachment
        /// POST: api/Ticket/attachment
        /// </summary>
        [HttpPost("attachment")]
        public async Task<IActionResult> UploadAttachment(
            [FromForm] int ticketId,
            [FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(ApiResponse<object>.Fail("No file provided"));

                if (file.Length > 10 * 1024 * 1024) // 10MB
                    return BadRequest(ApiResponse<object>.Fail("File size exceeds 10MB limit"));

                var allowedExtensions = new[] {
                    ".jpg", ".jpeg", ".png", ".gif", ".bmp",
                    ".pdf", ".doc", ".docx", ".xls", ".xlsx",
                    ".txt", ".csv", ".zip", ".rar", ".mp4"
                };
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(ext))
                    return BadRequest(ApiResponse<object>.Fail($"File type {ext} not allowed"));

                // Save file
                var fileName = $"ticket_{ticketId}_{Guid.NewGuid()}{ext}";
                var uploadPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", "tickets");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fullPath = Path.Combine(uploadPath, fileName);
                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var result = await _ticketRepo.AddAttachmentAsync(new AddAttachmentRequest
                {
                    TicketId = ticketId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/tickets/{fileName}",
                    FileSize = file.Length,
                    FileType = file.ContentType
                }, UserId);

                return Ok(ApiResponse<AddAttachmentResponse>.Success(result, "File uploaded"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading attachment");
                return StatusCode(500, ApiResponse<object>.Fail("Upload failed"));
            }
        }

        /// <summary>
        /// Download attachment
        /// GET: api/Ticket/attachment/{id}/download
        /// </summary>
        [HttpGet("attachment/{id:int}/download")]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            try
            {
                var attachment = await _ticketRepo.GetAttachmentByIdAsync(id);
                if (attachment == null)
                    return NotFound("Attachment not found");

                var rootPath = _environment.WebRootPath ?? "wwwroot";
                var fullPath = Path.Combine(rootPath, attachment.FilePath.TrimStart('/'));

                if (!System.IO.File.Exists(fullPath))
                    return NotFound("File not found on server");

                var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                return File(bytes,
                    attachment.FileType ?? "application/octet-stream",
                    attachment.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Download error");
                return StatusCode(500, "Download failed");
            }
        }

        /// <summary>
        /// Delete attachment
        /// DELETE: api/Ticket/attachment/{id}
        /// </summary>
        [HttpDelete("attachment/{id:int}")]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            try
            {
                var result = await _ticketRepo.DeleteAttachmentAsync(id, UserId);
                return result.Success
                    ? Ok(ApiResponse<TicketOperationResult>.Success(result))
                    : BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        #endregion

        #region DASHBOARD & DROPDOWNS

        /// <summary>
        /// Dashboard statistics
        /// GET: api/Ticket/dashboard
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard([FromQuery] bool myTicketsOnly = false)
        {
            try
            {
                var result = await _ticketRepo.GetDashboardAsync(
                    myTicketsOnly ? UserId : null,
                    myTicketsOnly ? UserRole : null);

                return Ok(ApiResponse<TicketDashboard>.Success(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard error");
                return StatusCode(500, ApiResponse<object>.Fail("Error loading dashboard"));
            }
        }

        /// <summary>
        /// Get dropdown data (dynamic from database)
        /// GET: api/Ticket/dropdowns
        /// </summary>
        [HttpGet("dropdowns")]
        public async Task<IActionResult> GetDropdowns()
        {
            try
            {
                var result = await _ticketRepo.GetDropdownsAsync();
                return Ok(ApiResponse<TicketDropdowns>.Success(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dropdowns");
                return StatusCode(500, ApiResponse<object>.Fail("Error loading dropdowns"));
            }
        }

        /// <summary>
        /// Get users by role (dynamic)
        /// GET: api/Ticket/users-by-role?roles=Developer,QA
        /// </summary>
        [HttpGet("users-by-role")]
        public async Task<IActionResult> GetUsersByRole(
            [FromQuery] string roles,
            [FromQuery] int? departmentId = null)
        {
            try
            {
                var users = await _ticketRepo.GetUsersByRolesAsync(roles, departmentId);
                return Ok(ApiResponse<List<UserDropdownItem>>.Success(users));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        #endregion
    }
}