namespace EmployeeManagement.UI.ViewModels
{
    public class LeaveDashboardViewModel
    {
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public List<LeaveBalanceViewModel> Balances { get; set; } = new();
        public List<LeaveRequestViewModel> RecentRequests { get; set; } = new();
        public List<HolidayViewModel> UpcomingHolidays { get; set; } = new();
        public int TotalPendingRequests { get; set; }
        public decimal TotalLeaveTaken { get; set; }
        public decimal TotalLeaveAvailable { get; set; }
    }
}
