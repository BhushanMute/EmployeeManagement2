namespace EmployeeManagement.UI.Models
{
    public class PayrollDashboardResponse
    {
        public string? ActiveCycleName { get; set; }
        public decimal? TotalPayout { get; set; }
        public int? TotalEmployeesProcessed { get; set; }
        public int? PendingApprovals { get; set; }
        public int? OnHoldCount { get; set; }
        public int? PaidCount { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? TotalGrossSalary { get; set; }
        public decimal? TotalNetSalary { get; set; }
    }
}
