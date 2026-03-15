namespace EmployeeManagement.UI.ViewModels
{
    public class LeaveTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DefaultDays { get; set; }
        public int MaxDays { get; set; }
        public bool IsCarryForward { get; set; }
        public int MaxCarryForward { get; set; }
        public bool IsPaid { get; set; }
        public bool IsActive { get; set; }
    }
}
