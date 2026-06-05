using System;

namespace EmployeeManagement.API.Models.Payroll
{
    public class BankFileData
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;

        // Bank Details
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string IFSCCode { get; set; } = string.Empty;

        // Payment Details  
        public decimal NetSalary { get; set; }
        public decimal PaymentAmount { get; set; } // Usually same as NetSalary

        // Transaction Info
        public string? PaymentMode { get; set; } // NEFT / RTGS / IMPS
        public string? ReferenceNo { get; set; }

        // Payroll संदर्भ
        public int CycleId { get; set; }
        public string? CycleName { get; set; }

        // Optional (Bank file formatting)
        public string? Narration { get; set; } // e.g., Salary for March 2026
        public DateTime? PaymentDate { get; set; }
    }
}