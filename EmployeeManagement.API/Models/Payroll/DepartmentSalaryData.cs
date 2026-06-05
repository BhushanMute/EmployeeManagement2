namespace EmployeeManagement.API.Models.Payroll
{
    public class DepartmentSalaryData
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNet { get; set; }
        public decimal AverageSalary { get; set; }
    }
}
