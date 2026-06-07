namespace EmployeeManagement.API.Models.Payroll
{
    public class PendingActions
    {
        public int PendingPayrollApprovals { get; set; }
        public int PendingLoanApprovals { get; set; }
        public int PendingAdvanceApprovals { get; set; }
        public int PendingReimbursementApprovals { get; set; }
        public int PendingTaxDeclarations { get; set; }
    }
}
