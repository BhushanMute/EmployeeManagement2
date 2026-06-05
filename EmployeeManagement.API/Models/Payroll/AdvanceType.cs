namespace EmployeeManagement.API.Models.Payroll
{
    public class AdvanceType
    {
        public int Id { get; set; }
        public string AdvanceTypeName { get; set; } = string.Empty;
        public string AdvanceTypeCode { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Rules
        public decimal MaxAmount { get; set; }
        public decimal? MaxPercentageOfSalary { get; set; }
        public int MaxRecoveryMonths { get; set; }

        // Eligibility
        public int MinServiceMonths { get; set; }
        public bool RequiresApproval { get; set; }

        // Deduction
        public string RecoveryStartFrom { get; set; } = "NextMonth";

        // Status
        public bool IsActive { get; set; }

        // Metadata
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
