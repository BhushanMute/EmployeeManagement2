namespace EmployeeManagement.API.Models.Payroll
{
    public class SalaryComponentDetailResponse
    {
        public int ComponentId { get; set; }
        public string ComponentCode { get; set; } = string.Empty;
        public string ComponentName { get; set; } = string.Empty;
        public string CalculationType { get; set; } = string.Empty;
        public decimal? Percentage { get; set; }
        public string? CalculationBase { get; set; }
        public decimal? MonthlyAmount { get; set; }
        public bool IsStatutory { get; set; }
        public bool IsTaxable { get; set; }
        public int DisplayOrder { get; set; }
        public string ComponentType { get; set; }  // ✅ ADD THIS


    }
}
