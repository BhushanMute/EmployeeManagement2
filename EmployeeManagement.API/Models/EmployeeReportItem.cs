namespace EmployeeManagement.API.Models
{
    public class EmployeeReportItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DepartmentName { get; set; }
        public string? Role { get; set; }
        public decimal Salary { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public string? ProfileImagePath { get; set; }
        public int YearsOfService { get; set; }
        public int Age { get; set; }
        public int LeavesThisYear { get; set; }
        public decimal LeaveDaysThisYear { get; set; }
    }
}
