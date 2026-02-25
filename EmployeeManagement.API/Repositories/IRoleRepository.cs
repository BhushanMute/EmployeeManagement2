using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByIdAsync(int id);
        Task<List<Role>> GetUserRolesAsync(int userId);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId, int assignedBy);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
    }
}
