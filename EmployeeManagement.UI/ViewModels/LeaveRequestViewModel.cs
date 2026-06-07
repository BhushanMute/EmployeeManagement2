namespace EmployeeManagement.UI.ViewModels
{
    public class LeaveRequestViewModel
    {
        // ===== Basic Info =====
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeEmail { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Designation { get; set; }
        public string? DepartmentName { get; set; }
        public int? DepartmentId { get; set; }

        // ===== Leave Info =====
        public int LeaveTypeId { get; set; }
        public string? LeaveTypeName { get; set; }
        public string? LeaveTypeCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime AppliedDate { get; set; }
        public bool IsHalfDay { get; set; }
        public string? HalfDayType { get; set; }
        public string? EmergencyContact { get; set; }
        public string? AttachmentPath { get; set; }

        // ===== Approval Info =====
        public int? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApproverRemarks { get; set; }

        // ✅ FIX: Add 'Remarks' as ALIAS for ApproverRemarks
        // This supports views that use Model.Remarks
        public string? Remarks
        {
            get => ApproverRemarks;
            set => ApproverRemarks = value;
        }

        // ===== Cancellation Info =====
        public DateTime? CancelledDate { get; set; }
        public string? CancelReason { get; set; }
        public int? CancelledBy { get; set; }

        // ===== Audit Fields =====
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        // ===== Computed Properties =====
        public string StatusBadgeClass => Status?.ToLower() switch
        {
            "approved" => "success",
            "rejected" => "danger",
            "pending" => "warning",
            "cancelled" => "secondary",
            _ => "info"
        };

        public string StatusIcon => Status?.ToLower() switch
        {
            "approved" => "check-circle",
            "rejected" => "x-circle",
            "pending" => "clock",
            "cancelled" => "ban",
            _ => "info-circle"
        };

        public bool IsUrgent => Status == "Pending" && (StartDate - DateTime.Today).TotalDays <= 3;

        public int DaysUntilLeave => (int)(StartDate - DateTime.Today).TotalDays;

        public bool HasAttachment => !string.IsNullOrEmpty(AttachmentPath);

        public string AttachmentFileName =>
            HasAttachment ? System.IO.Path.GetFileName(AttachmentPath!) : "";
    }
}