namespace EmployeeManagement.UI.ViewModels
{
    public class LeaveApprovalDashboardViewModel
    {
        public List<LeaveRequestViewModel> PendingRequests { get; set; } = new();
        public int TotalPending { get; set; }
        public int TotalApprovedToday { get; set; }
        public int TotalRejectedToday { get; set; }
    }
}
