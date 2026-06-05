namespace EmployeeManagement.UI.Models
{
    public class PayrollSummaryResponse
    {
        public int CycleId { get; set; }
        public string? CycleName { get; set; }
        public string? Status { get; set; }

        public decimal TotalGross { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetPay { get; set; }
    }
}
