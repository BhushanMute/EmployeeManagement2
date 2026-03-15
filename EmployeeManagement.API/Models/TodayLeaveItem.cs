namespace EmployeeManagement.API.Models
{
    public class TodayLeaveItem
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
    }
}
