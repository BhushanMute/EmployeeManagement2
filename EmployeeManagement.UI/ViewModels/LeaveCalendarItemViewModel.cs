namespace EmployeeManagement.UI.ViewModels
{
    public class LeaveCalendarItemViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public string LeaveTypeCode { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsHalfDay { get; set; }
        public string? HalfDayType { get; set; }
        public string? Reason { get; set; }
    }
}
