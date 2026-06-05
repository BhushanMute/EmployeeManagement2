using System;

namespace EmployeeManagement.API.Models.Payroll
{
    public class PTReportData
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;

        // PT Details
        public decimal GrossSalary { get; set; }
        public decimal PTAmount { get; set; }

        public string? State { get; set; } // Important for PT rules (MH, KA, etc.)

        // Payroll संदर्भ
        public int CycleId { get; set; }
        public string? CycleName { get; set; }
        public DateTime? SalaryMonth { get; set; }
    }
}