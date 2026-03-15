namespace EmployeeManagement.UI.ViewModels
{
    public class HolidayViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Day { get; set; } = string.Empty;
        public string Type { get; set; } = "Public";
        public string? Description { get; set; }
        public int Year { get; set; }
    }
}
