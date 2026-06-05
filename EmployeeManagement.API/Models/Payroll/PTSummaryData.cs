namespace EmployeeManagement.API.Models.Payroll
{
    public class PTSummaryData
    {
        public string StateCode { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public decimal TotalPT { get; set; }

        public List<PTEmployeeData> Employees { get; set; } = new();
    }
}
