namespace EmployeeManagement.API.Models
{
    public class SalaryReportData
    {
        public SalaryReportSummary Summary { get; set; } = new();
        public List<EmployeeSalaryItem> Employees { get; set; } = new();
        public List<DepartmentSalarySummary> DepartmentSummary { get; set; } = new();
    }
}
