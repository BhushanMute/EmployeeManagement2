namespace EmployeeManagement.API.Models
{
    public class SalaryTemplateComponent
    {
        public int Id { get; set; }

        public int TemplateId { get; set; }

        public int ComponentId { get; set; }

        public string? ComponentName { get; set; }

        public string? CalculationType { get; set; }   // e.g., Fixed / Percentage

        public string? CalculationBase { get; set; }   // e.g., Basic Salary

        public decimal? Percentage { get; set; }

        public decimal? FixedAmount { get; set; }

        public decimal? MonthlyAmount { get; set; }

        public decimal? AnnualAmount { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int CreatedBy { get; set; }
    }
}