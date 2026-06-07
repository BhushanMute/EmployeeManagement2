namespace EmployeeManagement.API.Models
{
    public class UpdateRoleRequest
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public string? RoleDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
