namespace EmployeeManagement.API.Models
{
    public class EmployeeReportSummary
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal TotalSalary { get; set; }
        public int NewJoinersThisMonth { get; set; }
        public int UpcomingBirthdays { get; set; }
    }
}
