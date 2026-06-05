namespace EmployeeManagement.API.Models.Payroll
{
    public class LoanDashboardStats
    {
        public int TotalLoans { get; set; }
        public int ActiveLoans { get; set; }
        public int ClosedLoans { get; set; }
        public int PendingApproval { get; set; }

        public decimal TotalLoanAmount { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public decimal TotalRecoveredAmount { get; set; }

        public int OverdueLoans { get; set; }
        public decimal OverdueAmount { get; set; }

        public int LoansDisbursedThisMonth { get; set; }
        public decimal AmountDisbursedThisMonth { get; set; }
    }
}