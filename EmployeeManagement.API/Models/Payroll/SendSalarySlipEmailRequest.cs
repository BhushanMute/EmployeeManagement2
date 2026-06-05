using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class SendSalarySlipEmailRequest
    {
        [Required]
        public int SlipId { get; set; }

        [EmailAddress]
        public string? EmailTo { get; set; } // NULL = use employee email

        public string? CustomMessage { get; set; }
        public bool IncludePayslipPDF { get; set; } = true;
    }
}
