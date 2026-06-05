using System;

namespace EmployeeManagement.API.Models.Payroll
{
    public class LoanPrepayment
    {
        public int PrepaymentId { get; set; }
        public int LoanId { get; set; }

        // Amount Details
        public decimal Amount { get; set; }

        // Prepayment Type (Full / Partial)
        public string PrepaymentType { get; set; } = string.Empty;

        // Payment Info
        public string? PaymentMode { get; set; } // Cash, BankTransfer, UPI, Cheque
        public string? ReferenceNo { get; set; }

        // Additional Info
        public string? Remarks { get; set; }

        // Dates
        public DateTime PaymentDate { get; set; }

        // Audit
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Optional (for reporting)
        public string? EmployeeName { get; set; }
        public string? LoanNumber { get; set; }
    }
}