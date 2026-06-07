namespace EmployeeManagement.API.Models
{
    public class UserListItem
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? Roles { get; set; }
        public string? RoleIds { get; set; }
    }

}
