namespace EmployeeManagement.API.Models.Payroll
{
    public class EmployeeLoan
    {
        public int Id { get; set; }
        public string LoanNumber { get; set; } = string.Empty;

        public int EmployeeId { get; set; }
        public int LoanTypeId { get; set; }

        // Loan Details
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal EMIAmount { get; set; }
        public decimal TotalRepayableAmount { get; set; }

        // Application
        public DateTime ApplicationDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public string? Purpose { get; set; }

        // Approval Workflow
        public string Status { get; set; } = "Pending";
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovalRemarks { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string? RejectionReason { get; set; }

        // Disbursement
        public DateTime? DisbursementDate { get; set; }
        public string? DisbursementMode { get; set; }
        public string? DisbursementReferenceNo { get; set; }
        public int? DisbursedBy { get; set; }

        // Repayment Tracking
        public DateTime? FirstEMIDate { get; set; }
        public DateTime? LastEMIDate { get; set; }
        public int TotalEMIsPaid { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public decimal PrincipalPaid { get; set; }
        public decimal InterestPaid { get; set; }
        public decimal OutstandingPrincipal { get; set; }
        public decimal OutstandingInterest { get; set; }
        public decimal OutstandingAmount { get; set; }

        // Closure
        public bool IsFullyPaid { get; set; }
        public DateTime? ClosureDate { get; set; }
        public string? ClosureType { get; set; }
        public int? ClosedBy { get; set; }

        // Guarantor
        public int? GuarantorEmployeeId { get; set; }
        public string? GuarantorName { get; set; }
        public string? GuarantorRelation { get; set; }

        // Documents
        public string? AttachmentPath { get; set; }

        // Remarks
        public string? Remarks { get; set; }

        // Metadata
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Properties
        public virtual ICollection<LoanEMISchedule> EMISchedule { get; set; } = new List<LoanEMISchedule>();
    }

}
