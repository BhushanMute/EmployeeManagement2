using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class DownloadSalarySlipRequest
    {
        [Required]
        public int SlipId { get; set; }

        public string Format { get; set; } = "PDF"; // PDF, Excel

        public string? Password { get; set; } // For password-protected slips
    }
}
