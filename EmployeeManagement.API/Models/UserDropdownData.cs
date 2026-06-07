namespace EmployeeManagement.API.Models
{
    public class UserDropdownData
    {
        public List<RoleWithCountResponse> Roles { get; set; } = new();
        public List<DropdownItem> Departments { get; set; } = new();
        public List<DropdownItem> Designations { get; set; } = new();
    }
}
