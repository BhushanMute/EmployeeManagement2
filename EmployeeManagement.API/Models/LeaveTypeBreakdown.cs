namespace EmployeeManagement.API.Models
{
    public class LeaveTypeBreakdown
    {
        public string LeaveTypeName { get; set; } = string.Empty;
        public string LeaveTypeCode { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public decimal ApprovedDays { get; set; }
        public decimal PendingDays { get; set; }
    }
}
