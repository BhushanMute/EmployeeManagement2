namespace EmployeeManagement.API.Models.Payroll
{
    public class EmployeeSalaryComponent
    {
        public int Id { get; set; }
        public int EmployeeSalaryStructureId { get; set; }
        public int ComponentId { get; set; }

        // Component Calculation Settings
        public string CalculationType { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public decimal? Percentage { get; set; }
        public string? CalculationBase { get; set; }
        public string? FormulaExpression { get; set; }

        // Component Type
        public string ComponentType { get; set; } = string.Empty;

        // Calculated Monthly Amount
        public decimal MonthlyAmount { get; set; }

        // Display & Behavior
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsMandatory { get; set; }

        // Effective Period
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        // Metadata
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Property
        public virtual SalaryComponent? Component { get; set; }
    }
}
