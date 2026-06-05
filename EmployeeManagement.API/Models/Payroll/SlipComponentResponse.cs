namespace EmployeeManagement.API.Models.Payroll
{
    public class SlipComponentResponse
    {
        public string ComponentName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int DisplayOrder { get; set; }
    }
}
