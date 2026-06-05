namespace EmployeeManagement.API.Models.Payroll
{
    public class PayrollDashboardResponse
    {
        // Current Month Summary
        public CurrentMonthSummary CurrentMonth { get; set; } = new();

        // Statistics
        public PayrollStatistics Statistics { get; set; } = new();

        // Pending Actions
        public PendingActions PendingActions { get; set; } = new();

        // Recent Activities
        public List<RecentActivity> RecentActivities { get; set; } = new();

        // Charts Data
        public List<MonthlyTrendData> MonthlyTrend { get; set; } = new();
        public List<DepartmentWiseData> DepartmentWise { get; set; } = new();

        public DateTime GeneratedDate { get; set; }
    }
}
