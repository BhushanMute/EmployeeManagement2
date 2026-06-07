namespace EmployeeManagement.API.Models.Payroll
{
    public class ReimbursementType
    {
        public int Id { get; set; }
        public string ReimbursementName { get; set; } = string.Empty;
        public string ReimbursementCode { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Limits
        public decimal? MaxAmountPerClaim { get; set; }
        public decimal? MaxAmountPerMonth { get; set; }
        public decimal? MaxAmountPerYear { get; set; }

        // Rules
        public bool RequiresBill { get; set; }
        public bool RequiresApproval { get; set; }
        public int ApprovalLevels { get; set; }

        // Tax Treatment
        public bool IsTaxable { get; set; }
        public decimal? TaxExemptionLimit { get; set; }

        // Processing
        public bool IncludeInSalarySlip { get; set; }
        public string? PaymentMode { get; set; }

        // Status
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }

        // Metadata
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
