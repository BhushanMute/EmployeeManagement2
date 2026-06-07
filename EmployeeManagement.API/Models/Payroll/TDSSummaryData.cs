namespace EmployeeManagement.API.Models.Payroll
{
    public class TDSSummaryData
    {
        public int TotalEmployees { get; set; }
        public decimal TotalTDS { get; set; }

        public List<TDSEmployeeData> Employees { get; set; } = new();
    }
}
