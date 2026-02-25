using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IAuthRepository
    {
        // 🔹 Authentication
        Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task<(int UserId, string Message)> RegisterUserAsync(User user, int roleId, int? createdBy);

        // 🔹 Roles & Permissions
        Task<List<Role>> GetRolesAsync(int userId);
        Task<List<Permission>> GetUserPermissionsAsync(int userId);
        Task<bool> CheckUserPermissionAsync(int userId, string permissionName);

        // 🔹 Login & Security
        Task UpdateLoginStatusAsync(int userId, bool isSuccess, string? ipAddress);
        Task UpdateUserLastLoginAsync(int userId, DateTime lastLoginDate, string? ipAddress);

        // 🔹 Refresh Token
        Task SaveRefreshTokenAsync(int userId, string token, DateTime expiryDate, string? ipAddress);
        Task<(RefreshToken? Token, User? User)> ValidateRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token, string? ipAddress, string? reason, string? replacedByToken);

        // 🔹 Password
        Task<string?> GetUserPasswordHashAsync(int userId);
         Task SavePasswordResetTokenAsync(int userId, string token, DateTime expiryDate);
        Task<PasswordResetToken?> ValidatePasswordResetTokenAsync(string token);
        Task MarkPasswordResetTokenUsedAsync(string token);

        // 🔹 Audit
        Task LogAuditAsync(int? userId, string action, string? tableName, int? recordId,
            string? oldValues, string? newValues, string? ipAddress, string? userAgent);

        // 🔹 User Methods
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
        Task<int> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<bool> CheckUsernameExistsAsync(string username);
        Task<bool> CheckEmailExistsAsync(string email);

        Task RevokeAllUserTokensAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string newPasswordHash, string newPasswordSalt, int? updatedBy);
    }
}