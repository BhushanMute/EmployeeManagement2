namespace EmployeeManagement.UI.Models
{
    public class SalaryStructureResponse
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }          // ← MISSING - ADDED
        public decimal? GrossSalary { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? NetSalary { get; set; }
        public decimal? AnnualCTC { get; set; }
        public List<SalaryComponentItem> Earnings { get; set; } = new();
        public List<SalaryComponentItem> Deductions { get; set; } = new();
    }
}
