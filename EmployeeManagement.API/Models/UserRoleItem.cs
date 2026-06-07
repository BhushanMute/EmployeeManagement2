namespace EmployeeManagement.API.Models
{
    public class UserRoleItem
    {
        public int UserRoleId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public string? RoleDescription { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
