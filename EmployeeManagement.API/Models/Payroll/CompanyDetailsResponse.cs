namespace EmployeeManagement.API.Models.Payroll
{
    public class CompanyDetailsResponse
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? GSTIN { get; set; }
        public string? PAN { get; set; }
        public string? CIN { get; set; }
        public string? LogoPath { get; set; }
    }
}
