namespace EmployeeManagement.UI.ViewModels
{
    public class MonthlyBreakdownViewModel
    {
        public int MonthNumber { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Pending { get; set; }
        public decimal ApprovedDays { get; set; }
    }
}
