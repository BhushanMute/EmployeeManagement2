namespace EmployeeManagement.UI.ViewModels
{
    public class EmployeeLeaveAllocationViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public List<LeaveBalanceViewModel> Balances { get; set; } = new();
        public decimal TotalAllocated { get; set; }
        public decimal TotalUsed { get; set; }
        public decimal TotalAvailable { get; set; }
    }
}
