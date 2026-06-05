using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class ApproveLoanRequest
    {
        [Required]
        public int LoanId { get; set; }

        [Required]
        public decimal ApprovedAmount { get; set; }

        [Required]
        public int ApprovedTenureMonths { get; set; }

        public string? Remarks { get; set; }
        public bool IsApproved { get; set; }
    }
}