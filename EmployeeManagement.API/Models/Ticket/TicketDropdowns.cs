namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketDropdowns
    {
        public List<string> Statuses { get; set; } = new();
        public List<string> Priorities { get; set; } = new();
        public List<string> TicketTypes { get; set; } = new();
        public List<UserDropdownItem> Developers { get; set; } = new();
        public List<UserDropdownItem> QAUsers { get; set; } = new();

        // ✅ NEW
        public List<DepartmentItem> Departments { get; set; } = new();
        public List<RoleItem> AllRoles { get; set; } = new();
    }
}
