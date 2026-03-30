using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Ticket
{
    public class AssignTicketRequest
    {
        [Required]
        public int TicketId { get; set; }

        [Required(ErrorMessage = "Assignee is required")]
        public int AssignedTo { get; set; }

        public string? Remarks { get; set; }
    }
}
