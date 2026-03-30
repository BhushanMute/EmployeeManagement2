using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Ticket
{
    public class AddCommentRequest
    {
        [Required]
        public int TicketId { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 5000 characters")]
        public string Comment { get; set; } = string.Empty;
    }
}
