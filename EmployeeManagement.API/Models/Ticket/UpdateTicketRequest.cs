using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Ticket
{
    public class UpdateTicketRequest
    {
        [Required]
        public int TicketId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(5000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string TicketType { get; set; } = string.Empty;

        [Required]
        public string Priority { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }
        public string? StepsToReproduce { get; set; }
        public string? ExpectedResult { get; set; }
        public string? ActualResult { get; set; }
        public string? Environment { get; set; }
    }
}
