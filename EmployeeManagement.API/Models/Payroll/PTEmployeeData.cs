namespace EmployeeManagement.API.Models.Payroll
{
    public class PTEmployeeData
    {
        public string EmployeeName { get; set; } = string.Empty;
        public decimal GrossSalary { get; set; }
        public decimal PTAmount { get; set; }
    }
}
