namespace EmployeeManagement.UI.ViewModels
{
    public class DepartmentBreakdownViewModel
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public int TotalRequests { get; set; }
        public decimal ApprovedDays { get; set; }
        public int PendingCount { get; set; }
    }
}
