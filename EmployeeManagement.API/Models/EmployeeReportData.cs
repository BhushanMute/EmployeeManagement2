namespace EmployeeManagement.API.Models
{
    public class EmployeeReportData
    {
        public EmployeeReportSummary Summary { get; set; } = new();
        public List<EmployeeReportItem> Employees { get; set; } = new();
        public List<DepartmentDistribution> DepartmentDistribution { get; set; } = new();
        public List<RoleDistribution> RoleDistribution { get; set; } = new();
    }
}
