namespace EmployeeManagement.API.Models
{
    public class CreateUserRequest
    {
        public string Username { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = "";
        public string PasswordHash { get; set; } = ""; // Set in controller
        public string RoleIds { get; set; } = "";       // "1,2,3"
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public string? EmployeeCode { get; set; }
    }
}
