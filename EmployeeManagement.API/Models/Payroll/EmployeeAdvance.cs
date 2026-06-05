namespace EmployeeManagement.API.Models.Payroll
{
    public class EmployeeAdvance
    {
        public int Id { get; set; }
        public string AdvanceNumber { get; set; } = string.Empty;

        public int EmployeeId { get; set; }
        public int AdvanceTypeId { get; set; }

        // Advance Details
        public decimal RequestedAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public int RecoveryMonths { get; set; }
        public decimal MonthlyRecoveryAmount { get; set; }

        // Request
        public DateTime RequestDate { get; set; }
        public string? Reason { get; set; }

        // Approval
        public string Status { get; set; } = "Pending";
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string? RejectionReason { get; set; }

        // Disbursement
        public DateTime? DisbursementDate { get; set; }
        public decimal DisbursedAmount { get; set; }

        // Recovery Tracking
        public decimal TotalRecovered { get; set; }
        public decimal OutstandingAmount { get; set; }
        public DateTime? RecoveryStartDate { get; set; }
        public DateTime? RecoveryEndDate { get; set; }
        public bool IsFullyRecovered { get; set; }

        // Metadata
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
