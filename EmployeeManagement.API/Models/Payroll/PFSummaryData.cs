namespace EmployeeManagement.API.Models.Payroll
{
    public class PFSummaryData
    {

        public int TotalEmployees { get; set; }
        public decimal TotalWages { get; set; }
        public decimal EmployeeContribution { get; set; }
        public decimal EmployerContribution { get; set; }
        public decimal EPFContribution { get; set; }
        public decimal EPSContribution { get; set; }
        public decimal AdminCharges { get; set; }
        public decimal EDLICharges { get; set; }
        public decimal TotalContribution { get; set; }

        public List<PFEmployeeData> Employees { get; set; } = new();
    }
}
