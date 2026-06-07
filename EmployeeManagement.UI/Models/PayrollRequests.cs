using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.UI.Models
{
    // ===========================================================
    // Create Payroll Cycle Request
    // ===========================================================
    public class CreatePayrollCycleRequest
    {
        [Required]
        public string CycleName { get; set; } = string.Empty;

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Range(2020, 2100)]
        public int Year { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime? PayDate { get; set; }
        public string? Notes { get; set; }
    }

    // ===========================================================
    // Process Payroll Request
    // ===========================================================
    public class ProcessPayrollRequest
    {
        [Required]
        public int PayrollCycleId { get; set; }

        public List<int>? EmployeeIds { get; set; }
        public bool RecalculateAll { get; set; } = false;
    }

    // ===========================================================
    // Approve Payroll Request
    // ===========================================================
    public class ApprovePayrollRequest
    {
        [Required]
        public int PayrollCycleId { get; set; }

        public string? ApprovalRemarks { get; set; }
    }
}