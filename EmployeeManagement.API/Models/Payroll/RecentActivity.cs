namespace EmployeeManagement.API.Models.Payroll
{
    public class RecentActivity
    {
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
    }
}
