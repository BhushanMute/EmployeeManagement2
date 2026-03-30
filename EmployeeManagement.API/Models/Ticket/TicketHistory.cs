namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketHistory
    {
        public int HistoryId { get; set; }
        public int TicketId { get; set; }
        public int ChangedBy { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ChangeDate { get; set; }
        public string? Remarks { get; set; }

        // User Info (from JOIN)
        public string? ChangedByName { get; set; }
        public string? ChangedByEmail { get; set; }

        // Computed Properties
        public string ChangeDescription => GetChangeDescription();

        public string ChangeIcon => ChangeType switch
        {
            "Created" => "bi-plus-circle text-success",
            "Updated" => "bi-pencil text-info",
            "Status Changed" => "bi-arrow-repeat text-warning",
            "Assigned" => "bi-person-plus text-primary",
            "Reassigned" => "bi-person-lines-fill text-info",
            "Comment Added" => "bi-chat-dots text-secondary",
            "Attachment Added" => "bi-paperclip text-primary",
            "Attachment Deleted" => "bi-trash text-danger",
            "Deleted" => "bi-trash text-danger",
            _ => "bi-circle"
        };

        private string GetChangeDescription()
        {
            return ChangeType switch
            {
                "Created" => "created this ticket",
                "Updated" => "updated ticket details",
                "Status Changed" => $"changed status from '{OldValue}' to '{NewValue}'",
                "Assigned" => $"assigned to {NewValue}",
                "Reassigned" => $"reassigned from '{OldValue}' to '{NewValue}'",
                "Comment Added" => "added a comment",
                "Attachment Added" => $"uploaded attachment '{NewValue}'",
                "Attachment Deleted" => $"deleted attachment '{OldValue}'",
                "Deleted" => "deleted this ticket",
                _ => ChangeType
            };
        }
    }
}
