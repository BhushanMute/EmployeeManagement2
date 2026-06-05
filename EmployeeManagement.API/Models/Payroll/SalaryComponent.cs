namespace EmployeeManagement.API.Models.Payroll
{
    public class SalaryComponent
    {
        public int Id { get; set; }
        public string ComponentCode { get; set; } = string.Empty;
        public string ComponentName { get; set; } = string.Empty;
        public string ComponentType { get; set; } = string.Empty; // Earning / Deduction
        public string? Category { get; set; } // Allowance, Statutory, Tax, Loan

        // Calculation Settings
        public string CalculationType { get; set; } = string.Empty; // Fixed, Percentage, Formula, Attendance
        public string? CalculationBase { get; set; } // Basic, Gross, CTC
        public decimal? DefaultPercentage { get; set; }
        public decimal? DefaultAmount { get; set; }

        // Display & Behavior
        public int DisplayOrder { get; set; }
        public bool IsStatutory { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsActive { get; set; }

        // Formula
        public string? FormulaExpression { get; set; }

        // Limits
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        // Metadata
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
