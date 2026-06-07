using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class LoanApplicationRequest
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int LoanTypeId { get; set; }

        [Required(ErrorMessage = "Loan amount is required")]
        [Range(1000, 10000000, ErrorMessage = "Loan amount must be between 1,000 and 1,00,00,000")]
        public decimal RequestedAmount { get; set; }

        [Required(ErrorMessage = "Tenure is required")]
        [Range(1, 240, ErrorMessage = "Tenure must be between 1 and 240 months")]
        public int TenureMonths { get; set; }

        [Required(ErrorMessage = "Purpose is required")]
        [StringLength(500, ErrorMessage = "Purpose cannot exceed 500 characters")]
        public string Purpose { get; set; } = string.Empty;

        public int? GuarantorEmployeeId { get; set; }
        public string? GuarantorName { get; set; }
        public string? GuarantorRelation { get; set; }

        public IFormFile? SupportingDocument { get; set; }
    }
}
