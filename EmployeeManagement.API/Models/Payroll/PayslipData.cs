using System;
using System.Collections.Generic;

namespace EmployeeManagement.API.Models.Payroll
{
    public class PayslipData
    {
        public int ProcessingId { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal? BasicSalary { get; set; }
        public decimal? GrossSalary { get; set; }
        public decimal? TotalEarnings { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? NetSalary { get; set; }
        public string? NetSalaryInWords { get; set; }

        public int? TotalWorkingDays { get; set; }
        public decimal? PresentDays { get; set; }
        public decimal? PaidLeaveDays { get; set; }
        public decimal? LOPDays { get; set; }

        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }

        public List<PayrollComponentResponse> Earnings { get; set; } = new();
        public List<PayrollComponentResponse> Deductions { get; set; } = new();
    }

    public class PayrollComponentResponse
    {
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? ComponentCode { get; set; }
        public decimal Amount { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsTaxable { get; set; }
        public int PayrollCycleId { get; set; }
    }
}
