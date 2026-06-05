using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class AdvanceRequest
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int AdvanceTypeId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(100, 1000000, ErrorMessage = "Amount must be between 100 and 10,00,000")]
        public decimal RequestedAmount { get; set; }

        [Required(ErrorMessage = "Recovery months is required")]
        [Range(1, 12, ErrorMessage = "Recovery months must be between 1 and 12")]
        public int RecoveryMonths { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;
    }

}
