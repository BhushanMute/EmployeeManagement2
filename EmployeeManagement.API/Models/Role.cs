using System.Security;

namespace EmployeeManagement.API.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<Permission> Permissions { get; set; } = new();
    }
}
