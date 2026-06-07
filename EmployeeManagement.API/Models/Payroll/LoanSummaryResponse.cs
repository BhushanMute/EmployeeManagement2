namespace EmployeeManagement.API.Models.Payroll
{
    public class LoanSummaryResponse
    {
        public int TotalLoans { get; set; }
        public int ActiveLoans { get; set; }
        public int ClosedLoans { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal MonthlyEMIDeduction { get; set; }

        public List<LoanDetailsResponse> Loans { get; set; } = new();
    }
}
