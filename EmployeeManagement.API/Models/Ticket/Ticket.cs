namespace EmployeeManagement.API.Models.Ticket
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Type & Priority
        public string TicketType { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Assignment
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }

        // User Info (from JOIN)
        public string? CreatedByName { get; set; }
        public string? CreatedByEmail { get; set; }
        public string? AssignedToName { get; set; }
        public string? AssignedToEmail { get; set; }

        // Dates
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public DateTime? ClosedDate { get; set; }

        // Bug Details
        public string? StepsToReproduce { get; set; }
        public string? ExpectedResult { get; set; }
        public string? ActualResult { get; set; }
        public string? Environment { get; set; }

        // Flags
        public bool IsOverdue { get; set; }
        public bool IsDeleted { get; set; }

        // Counts (f
        // rom subqueries)
        public int CommentCount { get; set; }
        public int AttachmentCount { get; set; }

        // Navigation Properties (for detailed view)
        public List<TicketComment>? Comments { get; set; }
        public List<TicketAttachment>? Attachments { get; set; }
        public List<TicketHistory>? History { get; set; }
    }
}