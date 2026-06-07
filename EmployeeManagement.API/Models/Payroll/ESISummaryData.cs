namespace EmployeeManagement.API.Models.Payroll
{
    public class ESISummaryData
    {
        public int TotalEmployees { get; set; }
        public decimal TotalWages { get; set; }
        public decimal EmployeeContribution { get; set; }
        public decimal EmployerContribution { get; set; }
        public decimal TotalContribution { get; set; }

        public List<ESIEmployeeData> Employees { get; set; } = new();
    }
}
