using System.Data;
using System.Security;

namespace EmployeeManagement.API.Models
{
    public class User
    {
        //public int Id { get; set; }

        //// Common fields
        //public string Username { get; set; }

        //public string Email { get; set; }

        //// NULL for Google/Facebook login
        //public string? PasswordHash { get; set; }

        //// Local, Google, Facebook
        //public string Provider { get; set; }

        //// Social login IDs
        //public string? GoogleId { get; set; }

        //public string? FacebookId { get; set; }

        //// Audit field
        //public DateTime CreatedDate { get; set; }
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        public List<UserRole> Roles { get; set; } = new();
        public List<Permission> Permissions { get; set; } = new();
     }
}
