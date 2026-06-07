namespace EmployeeManagement.API.Models
{
    public class CreateRoleRequest
    {
        public string RoleName { get; set; } = "";
        public string? RoleDescription { get; set; }
    }
}
