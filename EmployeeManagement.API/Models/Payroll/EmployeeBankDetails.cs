namespace EmployeeManagement.API.Models.Payroll
{
    public class EmployeeBankDetails
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        // Bank Details
        public string AccountHolderName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string? BranchName { get; set; }
        public string IFSCCode { get; set; } = string.Empty;
        public string? BankCode { get; set; }
        public string? BranchCode { get; set; }

        // Account Type
        public string AccountType { get; set; } = "Savings"; // Savings, Current, NRE, NRO

        // Primary Account
        public bool IsPrimaryAccount { get; set; }

        // Verification
        public bool IsVerified { get; set; }
        public int? VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string? VerificationMethod { get; set; }

        // Document
        public string? ChequeAttachmentPath { get; set; }

        // Effective Dates
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }

        // Metadata
        public string? Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
