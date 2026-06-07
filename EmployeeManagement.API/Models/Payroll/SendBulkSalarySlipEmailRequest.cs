using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class SendBulkSalarySlipEmailRequest
    {
        [Required]
        public int CycleId { get; set; }

        public List<int>? SlipIds { get; set; }

        public bool SendToAll { get; set; }

        public string? CustomMessage { get; set; }

        public bool IncludePayslipPDF { get; set; } = true;
    }
}
