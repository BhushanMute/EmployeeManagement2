namespace EmployeeManagement.API.Models.Ticket
{
    public class PriorityDistribution
    {
        public string Priority { get; set; } = string.Empty;
        public int Count { get; set; }

        public string BadgeClass => Priority switch
        {
            "Critical" => "bg-danger",
            "High" => "bg-warning",
            "Medium" => "bg-info",
            "Low" => "bg-secondary",
            _ => "bg-secondary"
        };
    }
}
