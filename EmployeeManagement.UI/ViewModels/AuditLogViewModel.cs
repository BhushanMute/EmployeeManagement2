namespace EmployeeManagement.UI.ViewModels
{
    public class AuditLogViewModel
    {
        public int Id { get; set; }  // ✅ Make sure this exists!
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? EntityName { get; set; }
        public int? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }

        // Display helpers
        public string ActionBadgeClass => Action switch
        {
            "Login" => "badge bg-success",
            "Logout" => "badge bg-secondary",
            "Create" => "badge bg-primary",
            "Update" => "badge bg-warning text-dark",
            "Delete" => "badge bg-danger",
            "PasswordChanged" => "badge bg-info",
            "LeaveApplied" => "badge bg-primary",
            "LeaveApproved" => "badge bg-success",
            "LeaveRejected" => "badge bg-danger",
            _ => "badge bg-secondary"
        };

        public string ActionIcon => Action switch
        {
            "Login" => "fas fa-sign-in-alt",
            "Logout" => "fas fa-sign-out-alt",
            "Create" => "fas fa-plus-circle",
            "Update" => "fas fa-edit",
            "Delete" => "fas fa-trash",
            "PasswordChanged" => "fas fa-key",
            "LeaveApplied" => "fas fa-calendar-plus",
            "LeaveApproved" => "fas fa-check-circle",
            "LeaveRejected" => "fas fa-times-circle",
            _ => "fas fa-info-circle"
        };
    }
}