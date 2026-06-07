using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class AssignSalaryRequest
    {
        [Required(ErrorMessage = "Employee ID is required")]
        public int EmployeeId { get; set; }

        public int? TemplateId { get; set; } // NULL if custom salary

        [Required(ErrorMessage = "Effective date is required")]
        public DateTime EffectiveFrom { get; set; }

        [Required(ErrorMessage = "CTC is required")]
        [Range(0.01, double.MaxValue)]
        public decimal CTC { get; set; }

        [Required(ErrorMessage = "Gross salary is required")]
        [Range(0.01, double.MaxValue)]
        public decimal GrossSalary { get; set; }

        [Required(ErrorMessage = "Net salary is required")]
        [Range(0.01, double.MaxValue)]
        public decimal NetSalary { get; set; }

        // ✅ ADD THESE (missing)
        public decimal BasicSalary { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal EmployerContributions { get; set; }

        public string? RevisionReason { get; set; }

        // Component-wise breakdown (if custom)
        public List<SalaryComponentRequest>? Components { get; set; }
    }
}