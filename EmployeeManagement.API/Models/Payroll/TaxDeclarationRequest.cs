using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class TaxDeclarationRequest
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public string FinancialYear { get; set; } = string.Empty;

        [Required]
        public string SelectedTaxRegime { get; set; } = "New"; // Old / New

        // Section 80C
        [Range(0, 150000, ErrorMessage = "LIC amount cannot exceed 1,50,000")]
        public decimal LIC { get; set; } = 0;

        [Range(0, 150000, ErrorMessage = "PPF amount cannot exceed 1,50,000")]
        public decimal PPF { get; set; } = 0;

        [Range(0, 150000, ErrorMessage = "ELSS amount cannot exceed 1,50,000")]
        public decimal ELSS { get; set; } = 0;

        [Range(0, 150000, ErrorMessage = "Home loan principal cannot exceed 1,50,000")]
        public decimal HomeLoanPrincipal { get; set; } = 0;

        [Range(0, 150000, ErrorMessage = "Tuition fees cannot exceed 1,50,000")]
        public decimal ChildrenTuitionFees { get; set; } = 0;

        [Range(0, 150000)]
        public decimal NSC { get; set; } = 0;

        [Range(0, 150000)]
        public decimal FD_5Year { get; set; } = 0;

        [Range(0, 150000)]
        public decimal Other80C { get; set; } = 0;

        // Section 80D
        [Range(0, 25000, ErrorMessage = "Self health insurance cannot exceed 25,000")]
        public decimal HealthInsurance_Self { get; set; } = 0;

        [Range(0, 50000, ErrorMessage = "Parents health insurance cannot exceed 50,000")]
        public decimal HealthInsurance_Parents { get; set; } = 0;

        [Range(0, 5000, ErrorMessage = "Preventive checkup cannot exceed 5,000")]
        public decimal PreventiveHealthCheckup { get; set; } = 0;

        // Section 80E
        public decimal EducationLoanInterest { get; set; } = 0;

        // Section 24
        [Range(0, 200000, ErrorMessage = "Home loan interest cannot exceed 2,00,000")]
        public decimal HomeLoanInterest { get; set; } = 0;

        // HRA Details
        public decimal HRA_Received { get; set; } = 0;
        public decimal Rent_Paid { get; set; } = 0;

        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format")]
        public string? LandlordPAN { get; set; }

        public bool IsMetroCity { get; set; } = false;

        // Other
        public decimal Section80G_Donation { get; set; } = 0;
        public decimal Section80TTA_SavingsInterest { get; set; } = 0;

        // Proof Documents
        public List<IFormFile>? ProofDocuments { get; set; }
    }
}
