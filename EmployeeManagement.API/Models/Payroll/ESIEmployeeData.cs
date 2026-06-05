namespace EmployeeManagement.API.Models.Payroll
{
    public class ESIEmployeeData
    {
        public string ESICNumber { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public decimal Wages { get; set; }
        public decimal EmployeeShare { get; set; }
        public decimal EmployerShare { get; set; }
    }
}
