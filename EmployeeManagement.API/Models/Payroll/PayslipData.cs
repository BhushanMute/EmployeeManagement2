using System;
using System.Collections.Generic;

namespace EmployeeManagement.API.Models.Payroll
{
    public class PayslipData
    {
        public int ProcessingId { get; set; }

        // Employee Info
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Designation { get; set; }

        // Salary Info
        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetSalary { get; set; }

        public string? NetSalaryInWords { get; set; }

        // Period
        public int Month { get; set; }
        public int Year { get; set; }

        // Attendance
        public int TotalWorkingDays { get; set; }
        public decimal PresentDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal LOPDays { get; set; }

        // Bank Info
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }

        // Lists (IMPORTANT for QueryMultiple)
        public List<PayrollComponentResponse> Earnings { get; set; } = new();
        public List<PayrollComponentResponse> Deductions { get; set; } = new();
    }
}