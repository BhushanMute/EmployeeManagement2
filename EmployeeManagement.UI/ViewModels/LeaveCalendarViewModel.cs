namespace EmployeeManagement.UI.ViewModels
{
    public class LeaveCalendarViewModel
    {
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }
        public int? SelectedDepartmentId { get; set; }
        public List<LeaveCalendarItemViewModel> LeaveItems { get; set; } = new();
        public List<HolidayViewModel> Holidays { get; set; } = new();
        public List<DepartmentViewModel>? Departments { get; set; }
    }
}
