namespace EmployeeManagement.API.Models.Ticket
{
    public class TypeDistribution
    {
        public string TicketType { get; set; } = string.Empty;
        public int Count { get; set; }

        public string BadgeClass => TicketType switch
        {
            "Bug" => "bg-danger",
            "Feature Request" => "bg-primary",
            "Improvement" => "bg-info",
            "Task" => "bg-secondary",
            _ => "bg-secondary"
        };
    }
}
