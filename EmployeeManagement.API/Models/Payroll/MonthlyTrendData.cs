namespace EmployeeManagement.API.Models.Payroll
{
    public class MonthlyTrendData
    {
        public string MonthYear { get; set; } = string.Empty;
        public decimal TotalGross { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNet { get; set; }
    }
}
