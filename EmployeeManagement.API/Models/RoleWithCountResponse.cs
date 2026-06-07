namespace EmployeeManagement.API.Models
{
    public class RoleWithCountResponse
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public string? RoleDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserCount { get; set; }
    }
}
