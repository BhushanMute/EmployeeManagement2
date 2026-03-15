namespace EmployeeManagement.API.Models
{
    public class EmployeeAttendanceItem
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? DepartmentName { get; set; }
        public string? Role { get; set; }
        public int TotalWorkingDays { get; set; }
        public decimal LeaveDays { get; set; }
        public decimal PresentDays { get; set; }
        public decimal CasualLeave { get; set; }
        public decimal SickLeave { get; set; }
        public decimal EarnedLeave { get; set; }
        public decimal LWP { get; set; }
        public decimal AttendancePercentage { get; set; }
    }
}
