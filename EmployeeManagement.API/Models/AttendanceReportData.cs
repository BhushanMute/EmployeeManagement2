namespace EmployeeManagement.API.Models
{
    public class AttendanceReportData
    {
        public AttendanceReportSummary Summary { get; set; } = new();
        public List<EmployeeAttendanceItem> Employees { get; set; } = new();
    }
}
