using EmployeeManagement.API.Models.Payroll;

public class SalaryStructureResponse
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;

    public string EmployeeCode { get; set; } = string.Empty;
    public string? DepartmentName { get; set; } = string.Empty;

    public string? TemplateName { get; set; } = string.Empty;
    public string? TemplateCode { get; set; } = string.Empty;

    // Salary Breakdown
    public decimal? CTC { get; set; }
    public decimal? GrossSalary { get; set; }
    public decimal? NetSalary { get; set; }
    public decimal? BasicSalary { get; set; }

    // Effective Period
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsCurrentStructure { get; set; }

    // Revision Info
    public int RevisionNumber { get; set; }
    public string? RevisionReason { get; set; }

    // Components
    public List<SalaryComponentDetailResponse> Earnings { get; set; } = new();
    public List<SalaryComponentDetailResponse> Deductions { get; set; } = new();

    // Summary
    public decimal TotalEarnings { get; set; }
    public decimal TotalDeductions { get; set; }

    public DateTime CreatedDate { get; set; }

    // Optional
    public decimal? EmployerContributions { get; set; }
}