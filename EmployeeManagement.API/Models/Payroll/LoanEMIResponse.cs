namespace EmployeeManagement.API.Models.Payroll
{
    public class LoanEMIResponse
    {
        public int EMINumber { get; set; }
        public DateTime EMIDueDate { get; set; }
        public decimal EMIAmount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public bool IsLatePayment { get; set; }
        public decimal LateFee { get; set; }
    }
}
