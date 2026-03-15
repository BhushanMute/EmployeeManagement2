namespace EmployeeManagement.API.Models
{
    public class DepartmentDistribution
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal AvgSalary { get; set; }
        public decimal TotalSalary { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
    }
}
