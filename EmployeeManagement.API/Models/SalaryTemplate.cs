namespace EmployeeManagement.API.Models
{
    public class SalaryTemplate
    {
        public int TemplateId { get; set; }

        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }

        public string? GradeLevel { get; set; }

        public decimal TotalCTC { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }

        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }

        public decimal EmployerContributions { get; set; }
    }
}
