namespace EmployeeManagement.UI.ViewModels
{
    public class LeaveReportPageViewModel
    {
        public int SelectedYear { get; set; }
        public int? SelectedDepartmentId { get; set; }
        public int SelectedMonth { get; set; }
        public List<EmployeeLeaveReportViewModel> EmployeeReport { get; set; } = new();
        public List<DepartmentLeaveReportViewModel> DepartmentReport { get; set; } = new();
        public List<MonthlyLeaveReportViewModel> MonthlyReport { get; set; } = new();
        public DashboardStatsViewModel? DashboardStats { get; set; }
        public List<DepartmentViewModel>? Departments { get; set; }
    }
}
