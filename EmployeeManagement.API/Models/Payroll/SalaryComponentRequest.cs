using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class SalaryComponentRequest
    {
        [Required]
        public int ComponentId { get; set; }

        [Required]
        public string CalculationType { get; set; } = string.Empty; // Fixed, Percentage, Formula

        public decimal? Amount { get; set; }
        public decimal? Percentage { get; set; }
        public string? CalculationBase { get; set; }
        public int DisplayOrder { get; set; }
    }
}
