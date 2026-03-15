using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IUserManagementRepository
    {
        Task<List<PendingUser>> GetPendingUsers();
        Task<List<PendingUser>> GetAllUsersWithRoles();
        Task ApproveUserAndAssignRole(int userId, int roleId, int? departmentId, int approvedBy);
        Task RejectUser(int userId, int rejectedBy, string? reason);
        Task<List<RoleInfo>> GetAllRoles();
    }
}