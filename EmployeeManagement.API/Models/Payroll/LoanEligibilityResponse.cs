namespace EmployeeManagement.API.Models.Payroll
{
    public class LoanEligibilityResponse
    {
        public bool IsEligible { get; set; }

        public decimal MaxEligibleAmount { get; set; }
        public int MaxTenureMonths { get; set; }

        public string? Reason { get; set; }
    }
}