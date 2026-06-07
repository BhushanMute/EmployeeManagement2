namespace EmployeeManagement.API.Models.Payroll
{
    public class EmployeeTaxDeclaration
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string FinancialYear { get; set; } = string.Empty;

        // Tax Regime Selection
        public string SelectedTaxRegime { get; set; } = "New"; // Old / New
        public DateTime? RegimeSelectionDate { get; set; }

        // Section 80C (Max 1,50,000)
        public decimal LIC { get; set; }
        public decimal PPF { get; set; }
        public decimal ELSS { get; set; }
        public decimal HomeLoanPrincipal { get; set; }
        public decimal ChildrenTuitionFees { get; set; }
        public decimal NSC { get; set; }
        public decimal FD_5Year { get; set; }
        public decimal Other80C { get; set; }
        public decimal Total80C { get; set; }

        // Section 80D (Health Insurance)
        public decimal HealthInsurance_Self { get; set; }
        public decimal HealthInsurance_Parents { get; set; }
        public decimal PreventiveHealthCheckup { get; set; }
        public decimal Total80D { get; set; }

        // Section 80E (Education Loan Interest)
        public decimal EducationLoanInterest { get; set; }

        // Section 24 (Home Loan Interest)
        public decimal HomeLoanInterest { get; set; }

        // HRA Exemption Details
        public decimal HRA_Received { get; set; }
        public decimal Rent_Paid { get; set; }
        public string? LandlordPAN { get; set; }
        public bool IsMetroCity { get; set; }

        // Other Deductions
        public decimal Section80G_Donation { get; set; }
        public decimal Section80TTA_SavingsInterest { get; set; }

        // Standard Deduction
        public decimal StandardDeduction { get; set; }

        // Total Deductions
        public decimal TotalDeductions { get; set; }

        // Declaration Status
        public string Status { get; set; } = "Draft";
        public DateTime? SubmittedDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }

        // Proof Submission
        public bool ProofSubmitted { get; set; }
        public DateTime? ProofSubmissionDate { get; set; }
        public string? ProofAttachmentPath { get; set; }

        // Lock
        public bool IsLocked { get; set; }
        public DateTime? LockedDate { get; set; }

        // Metadata
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
