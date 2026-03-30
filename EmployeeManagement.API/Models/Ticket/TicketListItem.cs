namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketListItem
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TicketType { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsOverdue { get; set; }
        public string? CreatedByName { get; set; }
        public string? AssignedToName { get; set; }
        public int CommentCount { get; set; }
        public int AttachmentCount { get; set; }

        // CSS Class helpers
        public string PriorityClass => Priority?.ToLower() switch
        {
            "critical" => "badge bg-danger",
            "high" => "badge bg-warning text-dark",
            "medium" => "badge bg-info",
            "low" => "badge bg-secondary",
            _ => "badge bg-secondary"
        };

        public string StatusClass => Status?.ToLower() switch
        {
            "new" => "badge bg-primary",
            "assigned" => "badge bg-info",
            "in progress" => "badge bg-warning text-dark",
            "blocked" => "badge bg-danger",
            "resolved" => "badge bg-success",
            "closed" => "badge bg-secondary",
            "reopened" => "badge bg-warning",
            _ => "badge bg-secondary"
        };

        public string TypeClass => TicketType?.ToLower() switch
        {
            "bug" => "badge bg-danger",
            "feature request" => "badge bg-primary",
            "improvement" => "badge bg-info",
            "task" => "badge bg-secondary",
            _ => "badge bg-secondary"
        };
    }
}  