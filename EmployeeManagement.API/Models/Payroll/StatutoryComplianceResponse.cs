namespace EmployeeManagement.API.Models.Payroll
{
    public class StatutoryComplianceResponse
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;

        // PF Summary
        public PFSummaryData PFSummary { get; set; } = new();

        // ESI Summary
        public ESISummaryData ESISummary { get; set; } = new();

        // PT Summary
        public PTSummaryData PTSummary { get; set; } = new();

        // TDS Summary
        public TDSSummaryData TDSSummary { get; set; } = new();

        public DateTime GeneratedDate { get; set; }
    }
}
