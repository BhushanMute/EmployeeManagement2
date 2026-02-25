using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IPermissionRepository
    {
        Task<List<Permission>> GetAllPermissionsAsync();
        Task<List<Permission>> GetUserPermissionsAsync(int userId);
        Task<List<Permission>> GetRolePermissionsAsync(int roleId);
        Task<bool> CheckUserPermissionAsync(int userId, string permissionName);
    }
}
