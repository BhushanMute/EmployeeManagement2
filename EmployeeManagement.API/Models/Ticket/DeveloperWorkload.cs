namespace EmployeeManagement.API.Models.Ticket
{
    public class DeveloperWorkload
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalAssigned { get; set; }
        public int InProgress { get; set; }
        public int Resolved { get; set; }
        public int Overdue { get; set; }

        public string WorkloadStatus => TotalAssigned switch
        {
            > 10 => "High",
            > 5 => "Medium",
            _ => "Low"
        };

        public string WorkloadBadge => WorkloadStatus switch
        {
            "High" => "badge bg-danger",
            "Medium" => "badge bg-warning",
            "Low" => "badge bg-success",
            _ => "badge bg-secondary"
        };
    }
}
