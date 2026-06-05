namespace EmployeeManagement.API.Models.Payroll
{
    public class PayrollRegisterResponse
    {
        public int PayrollCycleId { get; set; }
        public string CycleName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;

        // Summary
        public int TotalEmployees { get; set; }
        public decimal TotalBasic { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNet { get; set; }

        // Statutory Summary
        public decimal TotalPFEmployee { get; set; }
        public decimal TotalPFEmployer { get; set; }
        public decimal TotalESIEmployee { get; set; }
        public decimal TotalESIEmployer { get; set; }
        public decimal TotalPT { get; set; }
        public decimal TotalTDS { get; set; }

        // Employee-wise data
        public List<PayrollRegisterEmployeeData> Employees { get; set; } = new();

        public DateTime GeneratedDate { get; set; }
    }
}
