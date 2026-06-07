using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class PrepayLoanRequest
    {
        [Required]
        public int LoanId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Prepayment amount must be greater than 0")]
        public decimal PrepaymentAmount { get; set; }

        [Required]
        public string PrepaymentType { get; set; } = "Partial";
        // Partial / Full

        [Required]
        public string PaymentMode { get; set; } = "Bank";
        // Bank / Cash / Cheque

        public string? ReferenceNo { get; set; }

        public string? Remarks { get; set; }
    }
}