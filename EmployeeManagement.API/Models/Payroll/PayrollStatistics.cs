namespace EmployeeManagement.API.Models.Payroll
{
    public class PayrollStatistics
    {

        public int TotalActiveEmployees { get; set; }
    public decimal AverageSalary { get; set; }
    public decimal TotalMonthlySalaryBurden { get; set; }
    public decimal TotalAnnualSalaryBurden { get; set; }

    public int ActiveLoans { get; set; }
    public decimal TotalLoanOutstanding { get; set; }

    public int PendingAdvances { get; set; }
    public decimal TotalAdvanceAmount { get; set; }

    public int PendingReimbursements { get; set; }
    public decimal TotalReimbursementAmount { get; set; }
}
}
