using System;

namespace EmployeeManagement.API.Models.Payroll
{
    public class YTDSummary
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;

        public int FinancialYear { get; set; }

        // Earnings (YTD)
        public decimal TotalBasic { get; set; }
        public decimal TotalHRA { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalOvertime { get; set; }

        public decimal TotalEarnings { get; set; }

        // Deductions (YTD)
        public decimal TotalPF { get; set; }
        public decimal TotalESI { get; set; }
        public decimal TotalPT { get; set; }
        public decimal TotalTDS { get; set; }

        public decimal TotalDeductions { get; set; }

        // Net
        public decimal NetSalaryYTD { get; set; }

        // Taxation
        public decimal TaxableIncome { get; set; }
        public decimal TaxPaid { get; set; }
        public decimal RemainingTax { get; set; }

        // Additional Info
        public int MonthsProcessed { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}