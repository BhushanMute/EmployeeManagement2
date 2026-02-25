namespace EmployeeManagement.API.Models
{
    public class EmployeeUpdateRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? Role { get; set; }
        public decimal? Salary { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool? IsActive { get; set; }
    }
}
