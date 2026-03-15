namespace EmployeeManagement.UI.ViewModels
{
    public class EmployeeLeaveReportViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? EmployeeEmail { get; set; }
        public string? DepartmentName { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public string LeaveTypeCode { get; set; } = string.Empty;
        public decimal TotalAllocated { get; set; }
        public decimal TotalUsed { get; set; }
        public decimal TotalPending { get; set; }
        public decimal CarryForward { get; set; }
        public decimal TotalAvailable { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int PendingCount { get; set; }
    }
}
