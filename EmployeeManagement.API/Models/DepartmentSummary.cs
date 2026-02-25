namespace EmployeeManagement.API.Models
{
    public class DepartmentSummary
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal AverageSalary { get; set; }

    }
}
