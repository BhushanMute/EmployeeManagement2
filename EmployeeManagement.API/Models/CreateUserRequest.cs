namespace EmployeeManagement.API.Models.UserManagement
{
    public class CreateUserRequest
    {
        public string Username { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }

        public string Password { get; set; } = "";
        public string PasswordHash { get; set; } = "";

        // comma separated roles from UI: "1,2,3"
        public string RoleIds { get; set; } = "";

        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public string? EmployeeCode { get; set; }
    }
}