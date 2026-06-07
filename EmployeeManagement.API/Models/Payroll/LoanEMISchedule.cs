namespace EmployeeManagement.API.Models.Payroll
{
    public class LoanEMISchedule
    {
        public int Id { get; set; }
        public int LoanId { get; set; }

        // EMI Details
        public int EMINumber { get; set; }
        public DateTime EMIDueDate { get; set; }
        public decimal EMIAmount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }

        // Balance After EMI
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

        // Payment Status
        public string Status { get; set; } = "Pending";
        public DateTime? PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public int? PayrollCycleId { get; set; }

        // Late Payment
        public bool IsLatePayment { get; set; }
        public decimal LateFee { get; set; }

        // Remarks
        public string? Remarks { get; set; }

        // Metadata
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation Property
        public virtual EmployeeLoan? Loan { get; set; }
    }
}
