namespace EmployeeManagement.UI.ViewModels
{
    public class MonthlyLeaveReportViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public decimal TotalDays { get; set; }
        public decimal ApprovedDays { get; set; }
        public decimal RejectedDays { get; set; }
        public decimal PendingDays { get; set; }
        public decimal CancelledDays { get; set; }
    }
}
