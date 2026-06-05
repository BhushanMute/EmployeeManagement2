namespace EmployeeManagement.API.Models.Payroll
{
    public class PayrollComponentResponse
    {
        public string ComponentCode { get; set; } = string.Empty;
        public string ComponentName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool AdjustedForLOP { get; set; }
        public decimal? OriginalAmount { get; set; }
        public int DisplayOrder { get; set; }
    }
}
