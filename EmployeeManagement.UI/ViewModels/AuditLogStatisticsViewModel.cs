namespace EmployeeManagement.UI.ViewModels
{
    public class AuditLogStatisticsViewModel
    {
        public int TotalLogs { get; set; }
        public int SelectedDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<ActionCountViewModel> ActionCounts { get; set; } = new();
        public List<EntityCountViewModel> EntityCounts { get; set; } = new();
        public List<UserCountViewModel> UserCounts { get; set; } = new();
        public List<DailyCountViewModel> DailyCounts { get; set; } = new();
        public List<AuditLogViewModel> RecentLogs { get; set; } = new();
    }
}
