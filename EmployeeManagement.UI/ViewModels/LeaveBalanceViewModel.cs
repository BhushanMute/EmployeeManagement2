namespace EmployeeManagement.UI.ViewModels
{
    public class LeaveBalanceViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int Year { get; set; }
        public decimal TotalAllocated { get; set; }
        public decimal TotalUsed { get; set; }
        public decimal TotalPending { get; set; }
        public decimal CarryForward { get; set; }
        public decimal TotalAvailable { get; set; }
        public string? LeaveTypeName { get; set; }
        public string? LeaveTypeCode { get; set; }
        public bool IsPaid { get; set; }
        public string? EmployeeName { get; set; }
    }
}
