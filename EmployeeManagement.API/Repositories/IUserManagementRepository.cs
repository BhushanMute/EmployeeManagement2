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
        Task<UserListResponse> GetAllUsersAsync(string? search, string? role, string? status, int pageNumber, int pageSize);
        Task<UserDetailResponse?> GetUserByIdAsync(int userId);
        Task<UserOperationResult> CreateUserAsync(CreateUserRequest request, int createdBy);
        Task<UserOperationResult> UpdateUserAsync(UpdateUserRequest request, int updatedBy);
        Task<UserOperationResult> DeleteUserAsync(int userId, int deletedBy);
        Task<UserOperationResult> ToggleStatusAsync(int userId, bool isActive, int updatedBy);
        Task<UserOperationResult> ResetPasswordAsync(int userId, string passwordHash, int resetBy);

        Task<List<RoleWithCountResponse>> GetAllRolesAsync();
        Task<UserOperationResult> CreateRoleAsync(CreateRoleRequest request, int createdBy);
        Task<UserOperationResult> UpdateRoleAsync(UpdateRoleRequest request, int updatedBy);
        Task<UserOperationResult> DeleteRoleAsync(int roleId);

        Task<UserDropdownData> GetDropdownDataAsync();
    }
}