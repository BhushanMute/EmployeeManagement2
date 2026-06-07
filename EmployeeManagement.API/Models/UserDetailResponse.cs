namespace EmployeeManagement.API.Models
{
    public class UserDetailResponse
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
        public string? EmployeeCode { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public List<UserRoleItem> Roles { get; set; } = new();
    }
}
