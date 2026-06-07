// File: EmployeeManagement.API/Repositories/Ticket/ITicketRepository.cs
using EmployeeManagement.API.Models.Ticket;

namespace EmployeeManagement.API.Repositories.Ticket
{
    public interface ITicketRepository
    {
        // ===== EXISTING METHODS =====
        Task<CreateTicketResponse> CreateTicketAsync(CreateTicketRequest request, int createdBy);
        Task<Models.Ticket.Ticket?> GetTicketByIdAsync(int ticketId);
        Task<TicketListResponse> GetTicketsAsync(TicketFilterRequest filter);
        Task<TicketOperationResult> UpdateTicketAsync(UpdateTicketRequest request, int updatedBy);
        Task<TicketOperationResult> DeleteTicketAsync(int ticketId, int deletedBy);
        Task<TicketOperationResult> UpdateTicketStatusAsync(UpdateTicketStatusRequest request, int changedBy);
        Task<TicketOperationResult> AssignTicketAsync(AssignTicketRequest request, int assignedBy);
        Task<AddCommentResponse> AddCommentAsync(AddCommentRequest request, int userId);
        Task<List<TicketComment>> GetTicketCommentsAsync(int ticketId);
        Task<TicketOperationResult> DeleteCommentAsync(int commentId, int deletedBy);
        Task<AddAttachmentResponse> AddAttachmentAsync(AddAttachmentRequest request, int uploadedBy);
        Task<List<TicketAttachment>> GetTicketAttachmentsAsync(int ticketId);
        Task<TicketAttachment?> GetAttachmentByIdAsync(int attachmentId);
        Task<TicketOperationResult> DeleteAttachmentAsync(int attachmentId, int deletedBy);
        Task<List<TicketHistory>> GetTicketHistoryAsync(int ticketId);
        Task<TicketDashboard> GetDashboardAsync(int? userId = null, string? userRole = null);
        Task<List<TicketListItem>> GetMyTicketsAsync(MyTicketsRequest request);
        Task<int> UpdateOverdueTicketsAsync();
        Task<TicketDropdowns> GetDropdownsAsync();
        Task<List<UserDropdownItem>> GetDevelopersAsync();
        Task<List<UserDropdownItem>> GetQAUsersAsync();

        // ===== ✅ NEW METHODS =====

        /// <summary>
        /// Get users dynamically by role names (comma-separated)
        /// </summary>
        Task<List<UserDropdownItem>> GetUsersByRolesAsync(string roleNames, int? departmentId = null);

        /// <summary>
        /// Get all roles for dropdowns
        /// </summary>
        Task<List<RoleItem>> GetAllRolesAsync();

        /// <summary>
        /// Get all departments for filters
        /// </summary>
        Task<List<DepartmentItem>> GetAllDepartmentsAsync();

        /// <summary>
        /// Get email notification recipients for ticket events
        /// </summary>
        Task<List<TicketEmailRecipient>> GetEmailRecipientsAsync(int ticketId, string eventType);
    }
}