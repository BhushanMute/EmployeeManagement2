using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models.Ticket;
using EmployeeManagement.API.Repositories.Ticket;
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
        private readonly ILogger<TicketController> _logger;
        private readonly IWebHostEnvironment _environment;

        public TicketController(
            ITicketRepository ticketRepo,
            ILogger<TicketController> logger,
            IWebHostEnvironment environment)
        {
            _ticketRepo = ticketRepo;
            _logger = logger;
            _environment = environment;
        }

        #region COMMON HELPERS

        private int UserId =>
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        private string UserRole =>
            User.FindFirst(ClaimTypes.Role)?.Value ?? "Employee";

        private ActionResult<ApiResponse<T>> HandleValidation<T>()
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<T>.Fail("Validation failed", errors));
        }

        private ActionResult<ApiResponse<T>> HandleException<T>(Exception ex, string message)
        {
            _logger.LogError(ex, message);
            return StatusCode(500, ApiResponse<T>.Fail(message));
        }

        #endregion

        #region CRUD

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<CreateTicketResponse>>> CreateTicket(CreateTicketRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return HandleValidation<CreateTicketResponse>();

                if (UserId == 0)
                    return Unauthorized(ApiResponse<CreateTicketResponse>.Fail("Unauthorized"));

                var result = await _ticketRepo.CreateTicketAsync(request, UserId);

                return Ok(ApiResponse<CreateTicketResponse>.Success(result, "Ticket created"));
            }
            catch (Exception ex)
            {
                return HandleException<CreateTicketResponse>(ex, "Error creating ticket");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Ticket>>> GetTicket(int id)
        {
            try
            {
                var ticket = await _ticketRepo.GetTicketByIdAsync(id);
                if (ticket == null)
                    return NotFound(ApiResponse<Ticket>.Fail("Ticket not found"));

                ticket.Comments = await _ticketRepo.GetTicketCommentsAsync(id);
                ticket.Attachments = await _ticketRepo.GetTicketAttachmentsAsync(id);
                ticket.History = await _ticketRepo.GetTicketHistoryAsync(id);

                return Ok(ApiResponse<Ticket>.Success(ticket));
            }
            catch (Exception ex)
            {
                return HandleException<Ticket>(ex, "Error fetching ticket");
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse<TicketListResponse>>> GetTickets([FromQuery] TicketFilterRequest filter)
        {
            try
            {
                var result = await _ticketRepo.GetTicketsAsync(filter);
                return Ok(ApiResponse<TicketListResponse>.Success(result));
            }
            catch (Exception ex)
            {
                return HandleException<TicketListResponse>(ex, "Error fetching tickets");
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<TicketOperationResult>>> UpdateTicket(UpdateTicketRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return HandleValidation<TicketOperationResult>();

                var result = await _ticketRepo.UpdateTicketAsync(request, UserId);

                return result.Success
                    ? Ok(ApiResponse<TicketOperationResult>.Success(result, result.Message))
                    : BadRequest(ApiResponse<TicketOperationResult>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                return HandleException<TicketOperationResult>(ex, "Error updating ticket");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<TicketOperationResult>>> Delete(int id)
        {
            try
            {
                var result = await _ticketRepo.DeleteTicketAsync(id, UserId);

                return result.Success
                    ? Ok(ApiResponse<TicketOperationResult>.Success(result))
                    : BadRequest(ApiResponse<TicketOperationResult>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                return HandleException<TicketOperationResult>(ex, "Error deleting ticket");
            }
        }

        #endregion

        #region STATUS

        [HttpPut("status")]
        public async Task<ActionResult<ApiResponse<TicketOperationResult>>> UpdateStatus(UpdateTicketStatusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return HandleValidation<TicketOperationResult>();

                var result = await _ticketRepo.UpdateTicketStatusAsync(request, UserId);

                return result.Success
                    ? Ok(ApiResponse<TicketOperationResult>.Success(result))
                    : BadRequest(ApiResponse<TicketOperationResult>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                return HandleException<TicketOperationResult>(ex, "Error updating status");
            }
        }

        #endregion

        #region COMMENT

        [HttpPost("comment")]
        public async Task<ActionResult<ApiResponse<AddCommentResponse>>> AddComment(AddCommentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return HandleValidation<AddCommentResponse>();

                var result = await _ticketRepo.AddCommentAsync(request, UserId);

                return Ok(ApiResponse<AddCommentResponse>.Success(result));
            }
            catch (Exception ex)
            {
                return HandleException<AddCommentResponse>(ex, "Error adding comment");
            }
        }

        #endregion

        #region ATTACHMENT

        [HttpPost("attachment")]
        public async Task<ActionResult<ApiResponse<AddAttachmentResponse>>> Upload(int ticketId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(ApiResponse<AddAttachmentResponse>.Fail("Invalid file"));

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var path = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);

                var result = await _ticketRepo.AddAttachmentAsync(new AddAttachmentRequest
                {
                    TicketId = ticketId,
                    FileName = file.FileName,
                    FilePath = "/uploads/" + fileName,
                    FileSize = file.Length,
                    FileType = file.ContentType
                }, UserId);

                return Ok(ApiResponse<AddAttachmentResponse>.Success(result));
            }
            catch (Exception ex)
            {
                return HandleException<AddAttachmentResponse>(ex, "Upload failed");
            }
        }

        #endregion

        #region DASHBOARD

        [HttpGet("dashboard")]
        public async Task<ActionResult<ApiResponse<TicketDashboard>>> Dashboard(bool myTicketsOnly = false)
        {
            try
            {
                var result = await _ticketRepo.GetDashboardAsync(
                    myTicketsOnly ? UserId : null,
                    myTicketsOnly ? UserRole : null
                );

                return Ok(ApiResponse<TicketDashboard>.Success(result));
            }
            catch (Exception ex)
            {
                return HandleException<TicketDashboard>(ex, "Dashboard error");
            }
        }

        #endregion
    }
}