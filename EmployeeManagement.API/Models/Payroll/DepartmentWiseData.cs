namespace EmployeeManagement.API.Models.Payroll
{
    public class DepartmentWiseData
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal TotalSalary { get; set; }
    }
}
