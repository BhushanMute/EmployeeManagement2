namespace EmployeeManagement.UI.ViewModels
{
    public class AdminLeaveDashboardViewModel
    {

        public int TotalEmployees { get; set; }
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int CancelledRequests { get; set; }
        public decimal TotalApprovedDays { get; set; }
        public int EmployeesOnLeave { get; set; }
        public int OnLeaveToday { get; set; }
        public int TotalHolidays { get; set; }
        public int UpcomingHolidays { get; set; }
        public int SelectedYear { get; set; }

        public List<MonthlyBreakdownViewModel> MonthlyData { get; set; } = new();
        public List<LeaveTypeBreakdownViewModel> LeaveTypeData { get; set; } = new();
        public List<DepartmentBreakdownViewModel> DepartmentData { get; set; } = new();
        public List<RecentLeaveItemViewModel> RecentRequests { get; set; } = new();
        public List<TodayLeaveItemViewModel> OnLeaveTodayList { get; set; } = new();
    }
}
