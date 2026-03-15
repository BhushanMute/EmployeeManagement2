namespace EmployeeManagement.UI.ViewModels
{
    public class DashboardStatsViewModel
    {
        public int TotalRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int CancelledRequests { get; set; }
        public decimal TotalApprovedDays { get; set; }
        public int EmployeesWithLeave { get; set; }
        public int TotalActiveEmployees { get; set; }
        public List<MonthlyBreakdownViewModel> MonthlyData { get; set; } = new();
    }
}
