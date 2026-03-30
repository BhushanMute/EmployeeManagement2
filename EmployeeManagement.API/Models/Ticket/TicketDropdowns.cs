namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketDropdowns
    {
        public List<UserDropdownItem> Developers { get; set; } = new();
        public List<UserDropdownItem> QAUsers { get; set; } = new();
        public List<string> Statuses { get; set; } = new();
        public List<string> Priorities { get; set; } = new();
        public List<string> TicketTypes { get; set; } = new();
        public List<string> Environments { get; set; } = new()
        {
            "Development",
            "Staging",
            "UAT",
            "Production"
        };
    }
}
