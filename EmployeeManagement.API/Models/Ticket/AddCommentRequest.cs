using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Ticket
{
    public class AddCommentRequest
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Comment cannot be empty")]
        [MaxLength(5000, ErrorMessage = "Comment too long")]
        public string Comment { get; set; } = "";

        // ✅ NEW: For internal notes (visible only to staff, not creator)
        public bool IsInternal { get; set; } = false;

        // ✅ Optional: For threaded replies
        public int? ParentCommentId { get; set; }
    }
}
