namespace EmployeeManagement.API.Models.Payroll
{
    public class EmployeeReimbursement
    {
        public int Id { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;

        public int EmployeeId { get; set; }
        public int ReimbursementTypeId { get; set; }

        // Claim Details
        public DateTime ClaimDate { get; set; }
        public decimal ClaimAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }

        // Details
        public DateTime ExpenseDate { get; set; }
        public string? Description { get; set; }
        public string? BillNumber { get; set; }
        public string? VendorName { get; set; }

        // Attachments
        public string? AttachmentPath { get; set; }
        public string? BillAttachmentPath { get; set; }

        // Approval Workflow
        public string Status { get; set; } = "Pending";
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovalRemarks { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string? RejectionReason { get; set; }

        // Payment
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime? PaymentDate { get; set; }
        public int? PayrollCycleId { get; set; }
        public string? PaymentMode { get; set; }
        public string? PaymentReferenceNo { get; set; }

        // Metadata
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }


}
