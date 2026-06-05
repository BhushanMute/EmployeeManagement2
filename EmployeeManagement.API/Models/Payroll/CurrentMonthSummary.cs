namespace EmployeeManagement.API.Models.Payroll
{
    public class CurrentMonthSummary
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public int ProcessedEmployees { get; set; }
        public decimal TotalNetSalary { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? SalaryCreditDate { get; set; }
    }
}
