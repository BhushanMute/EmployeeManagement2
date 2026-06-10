namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketDropdowns
    {
        // Old properties - existing UI compatibility ke liye
        public List<string> Statuses { get; set; } = new();
        public List<string> Priorities { get; set; } = new();
        public List<string> TicketTypes { get; set; } = new();

        // New dynamic master items
        public List<TicketMasterItem> TicketTypeItems { get; set; } = new();
        public List<TicketPriorityItem> PriorityItems { get; set; } = new();
        public List<TicketStatusItem> StatusItems { get; set; } = new();
        public List<TicketMasterItem> CategoryItems { get; set; } = new();
        public List<TicketModuleItem> ModuleItems { get; set; } = new();
        public List<TicketMasterItem> SeverityItems { get; set; } = new();

        // Users / Roles / Departments
        public List<UserDropdownItem> Developers { get; set; } = new();
        public List<UserDropdownItem> QAUsers { get; set; } = new();
        public List<RoleItem> Roles { get; set; } = new();
        public List<DepartmentItem> Departments { get; set; } = new();
    }

  
}