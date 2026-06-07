namespace EmployeeManagement.API.Models.Payroll
{
    public class SlipComponentResponse
    {
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? ComponentCode { get; set; }
        public decimal Amount { get; set; }
        public string? ComponentType { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsTaxable { get; set; }
    }
}
