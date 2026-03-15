using EmployeeManagement.UI.Models;

namespace EmployeeManagement.UI.ViewModels  // 👈 Changed
{
    public class UserApprovalPageViewModel
    {
        public List<PendingUserViewModel> PendingUsers { get; set; } = new();
        public List<PendingUserViewModel> AllUsers { get; set; } = new();
        public List<RoleInfoViewModel> Roles { get; set; } = new();
        public List<DepartmentViewModel>? Departments { get; set; }
        public int TotalPending { get; set; }
        public int TotalApproved { get; set; }
        public int TotalRejected { get; set; }
    }
}