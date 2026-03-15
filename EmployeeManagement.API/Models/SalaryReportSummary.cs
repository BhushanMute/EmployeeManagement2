namespace EmployeeManagement.API.Models
{
    public class SalaryReportSummary
    {
        public int TotalEmployees { get; set; }
        public decimal TotalMonthlySalary { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal HighestSalary { get; set; }
        public decimal LowestSalary { get; set; }
        public int WorkingDays { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int ReportYear { get; set; }
    }
}
