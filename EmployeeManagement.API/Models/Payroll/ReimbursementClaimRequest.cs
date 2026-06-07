using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class ReimbursementClaimRequest
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ReimbursementTypeId { get; set; }

        [Required(ErrorMessage = "Claim amount is required")]
        [Range(1, 1000000, ErrorMessage = "Claim amount must be between 1 and 10,00,000")]
        public decimal ClaimAmount { get; set; }

        [Required(ErrorMessage = "Expense date is required")]
        public DateTime ExpenseDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        public string? BillNumber { get; set; }
        public string? VendorName { get; set; }

        public IFormFile? BillAttachment { get; set; }
        public IFormFile? SupportingDocument { get; set; }
    }
}
