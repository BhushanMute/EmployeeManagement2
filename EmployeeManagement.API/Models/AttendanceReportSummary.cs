namespace EmployeeManagement.API.Models
{
    public class AttendanceReportSummary
    {
        public int TotalWorkingDays { get; set; }
        public int HolidaysInMonth { get; set; }
        public int ReportMonth { get; set; }
        public int ReportYear { get; set; }
        public string MonthName { get; set; } = string.Empty;
    }
}
