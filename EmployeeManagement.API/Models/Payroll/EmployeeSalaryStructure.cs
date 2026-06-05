namespace EmployeeManagement.API.Models.Payroll
{
    public class EmployeeSalaryStructure
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int? TemplateId { get; set; }

        // Salary Details
        public decimal CTC { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }

        // ✅ ADD THESE (missing)
        public decimal BasicSalary { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal EmployerContributions { get; set; }

        // Effective Date Management
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsCurrentStructure { get; set; }

        // Revision Tracking
        public int RevisionNumber { get; set; }
        public string? RevisionReason { get; set; }
        public int? PreviousStructureId { get; set; }

        // Metadata
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<EmployeeSalaryComponent> Components { get; set; } = new List<EmployeeSalaryComponent>();
    }
}