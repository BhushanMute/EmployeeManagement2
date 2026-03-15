namespace EmployeeManagement.API.Models
{
    public class HolidayStats
    {
        public int TotalHolidays { get; set; }
        public int PublicHolidays { get; set; }
        public int OptionalHolidays { get; set; }
        public int RestrictedHolidays { get; set; }
        public int UpcomingHolidays { get; set; }
        public int PastHolidays { get; set; }
    }
}
