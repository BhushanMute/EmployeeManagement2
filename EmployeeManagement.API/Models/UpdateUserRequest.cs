namespace EmployeeManagement.API.Models
{
    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string RoleIds { get; set; } = "";
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public string? EmployeeCode { get; set; }
    }
}
