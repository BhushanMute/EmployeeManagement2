namespace EmployeeManagement.UI.ViewModels
{
    public class HolidayPageViewModel
    {
        public List<HolidayViewModel> Holidays { get; set; } = new();
        public HolidayStatsViewModel Stats { get; set; } = new();
        public int SelectedYear { get; set; }
    }
}
