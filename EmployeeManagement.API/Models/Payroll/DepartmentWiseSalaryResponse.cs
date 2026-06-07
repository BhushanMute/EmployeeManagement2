namespace EmployeeManagement.API.Models.Payroll
{
    public class DepartmentWiseSalaryResponse
    {
        public int PayrollCycleId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;

        public decimal GrandTotalGross { get; set; }
        public decimal GrandTotalDeductions { get; set; }
        public decimal GrandTotalNet { get; set; }

        public List<DepartmentSalaryData> Departments { get; set; } = new();

        public DateTime GeneratedDate { get; set; }
    }
}
