using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class GenerateSalarySlipRequest
    {
        [Required]
        public int PayrollCycleId { get; set; }

        public List<int>? EmployeeIds { get; set; } // NULL = All employees

        public bool SendEmail { get; set; } = false;
        public bool GeneratePDF { get; set; } = true;
        public bool PasswordProtect { get; set; } = false;
    }
}
