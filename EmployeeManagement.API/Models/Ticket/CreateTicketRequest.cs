using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Ticket
{
    public class CreateTicketRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 5000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ticket type is required")]
        public string TicketType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; } = string.Empty;

        public int? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }

        // Bug specific fields
        public string? StepsToReproduce { get; set; }
        public string? ExpectedResult { get; set; }
        public string? ActualResult { get; set; }
        public string? Environment { get; set; }
        public int? TicketTypeId { get; set; }
        public int? PriorityId { get; set; }
        public int? CategoryId { get; set; }
        public int? ModuleId { get; set; }
        public int? SeverityId { get; set; }

        

        public string? Category { get; set; }
        public string? Module { get; set; }
        public string? Severity { get; set; }
    }
}
