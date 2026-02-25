namespace EmployeeManagement.API.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Module { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
