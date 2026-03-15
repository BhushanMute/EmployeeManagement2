namespace EmployeeManagement.UI.ViewModels
{
    public class LeaveRequestViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public bool IsHalfDay { get; set; }
        public string? HalfDayType { get; set; }
        public string? AttachmentPath { get; set; }
        public string? EmergencyContact { get; set; }
        public string? Remarks { get; set; }
        public DateTime AppliedDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CancelReason { get; set; }

        // Navigation
        public string? EmployeeName { get; set; }
        public string? EmployeeEmail { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? LeaveTypeName { get; set; }
        public string? LeaveTypeCode { get; set; }

        // For dropdowns
        public List<LeaveTypeViewModel>? LeaveTypes { get; set; }
    }
}
