namespace EmployeeManagement.API.Models.Ticket
{
    public class UserDropdownItem
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Role { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? ProfilePicture { get; set; }

        public List<RoleWithCountResponse> Roles { get; set; } = new();
        public List<DropdownItem> Departments { get; set; } = new();
        public List<DropdownItem> Designations { get; set; } = new();
    }
}
