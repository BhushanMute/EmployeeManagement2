using EmployeeManagement.API.Models.Ticket;
 
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EmployeeManagement.API.Repositories.Ticket
{
    /// <summary>
    /// Interface for Ticket Repository
    /// </summary>
    public interface ITicketRepository
    {
        // =============================================
        // TICKET CRUD OPERATIONS
        // =============================================

        /// <summary>
        /// Create a new ticket
        /// </summary>
        Task<CreateTicketResponse> CreateTicketAsync(CreateTicketRequest request, int createdBy);

        /// <summary>
        /// Get ticket by ID with full details
        /// </summary>
        Task<Models.Ticket.Ticket?> GetTicketByIdAsync(int ticketId);

        /// <summary>
        /// Get all tickets with filters and pagination
        /// </summary>
        Task<TicketListResponse> GetTicketsAsync(TicketFilterRequest filter);

        /// <summary>
        /// Update ticket details
        /// </summary>
        Task<TicketOperationResult> UpdateTicketAsync(UpdateTicketRequest request, int updatedBy);

        /// <summary>
        /// Delete ticket (soft delete)
        /// </summary>
        Task<TicketOperationResult> DeleteTicketAsync(int ticketId, int deletedBy);

        // =============================================
        // STATUS OPERATIONS
        // =============================================

        /// <summary>
        /// Update ticket status
        /// </summary>
        Task<TicketOperationResult> UpdateTicketStatusAsync(UpdateTicketStatusRequest request, int changedBy);

        // =============================================
        // ASSIGNMENT OPERATIONS
        // =============================================

        /// <summary>
        /// Assign or reassign ticket to developer
        /// </summary>
        Task<TicketOperationResult> AssignTicketAsync(AssignTicketRequest request, int assignedBy);

        // =============================================
        // COMMENT OPERATIONS
        // =============================================

        /// <summary>
        /// Add comment to ticket
        /// </summary>
        Task<AddCommentResponse> AddCommentAsync(AddCommentRequest request, int userId);

        /// <summary>
        /// Get all comments for a ticket
        /// </summary>
        Task<List<TicketComment>> GetTicketCommentsAsync(int ticketId);

        /// <summary>
        /// Delete comment (soft delete)
        /// </summary>
        Task<TicketOperationResult> DeleteCommentAsync(int commentId, int deletedBy);

        // =============================================
        // ATTACHMENT OPERATIONS
        // =============================================

        /// <summary>
        /// Add attachment to ticket
        /// </summary>
        Task<AddAttachmentResponse> AddAttachmentAsync(AddAttachmentRequest request, int uploadedBy);

        /// <summary>
        /// Get all attachments for a ticket
        /// </summary>
        Task<List<TicketAttachment>> GetTicketAttachmentsAsync(int ticketId);

        /// <summary>
        /// Get attachment by ID
        /// </summary>
        Task<TicketAttachment?> GetAttachmentByIdAsync(int attachmentId);

        /// <summary>
        /// Delete attachment (soft delete)
        /// </summary>
        Task<TicketOperationResult> DeleteAttachmentAsync(int attachmentId, int deletedBy);

        // =============================================
        // HISTORY OPERATIONS
        // =============================================

        /// <summary>
        /// Get ticket history (audit trail)
        /// </summary>
        Task<List<TicketHistory>> GetTicketHistoryAsync(int ticketId);

        // =============================================
        // DASHBOARD & REPORTS
        // =============================================

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        Task<TicketDashboard> GetDashboardAsync(int? userId = null, string? userRole = null);

        /// <summary>
        /// Get my tickets (based on role)
        /// </summary>
        Task<List<TicketListItem>> GetMyTicketsAsync(MyTicketsRequest request);

        /// <summary>
        /// Update overdue tickets
        /// </summary>
        Task<int> UpdateOverdueTicketsAsync();

        // =============================================
        // DROPDOWN DATA
        // =============================================

        /// <summary>
        /// Get dropdown data for ticket forms
        /// </summary>
        Task<TicketDropdowns> GetDropdownsAsync();

        /// <summary>
        /// Get developers list for assignment
        /// </summary>
        Task<List<UserDropdownItem>> GetDevelopersAsync();

        /// <summary>
        /// Get QA users list
        /// </summary>
        Task<List<UserDropdownItem>> GetQAUsersAsync();
    }
}