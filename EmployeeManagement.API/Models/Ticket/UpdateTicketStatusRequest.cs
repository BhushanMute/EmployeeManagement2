using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Ticket
{
    public class UpdateTicketStatusRequest
    {
        [Required]
        public int TicketId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string NewStatus { get; set; } = string.Empty;

        public string? Remarks { get; set; }
    }
}
