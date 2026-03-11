using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IAuthRepository
    {
        // Authentication
        Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task<(int UserId, string Message)> RegisterUserAsync(User user, int roleId, int? createdBy);
        Task UpdateLoginStatusAsync(int userId, bool isSuccess, string? ipAddress);

        // Roles & Permissions
        Task<List<Role>> GetRolesAsync(int userId);
        Task<List<Permission>> GetUserPermissionsAsync(int userId);
        Task<bool> CheckUserPermissionAsync(int userId, string permissionName);

        // Refresh Tokens
        Task SaveRefreshTokenAsync(int userId, string token, DateTime expiryDate, string? ipAddress);
        Task<(RefreshToken? Token, User? User)> ValidateRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token, string? ipAddress, string? reason, string? replacedByToken);
        Task RevokeAllUserTokensAsync(int userId);
        Task<List<RefreshToken>> GetUserActiveTokensAsync(int userId);
        Task CleanupExpiredTokensAsync();

        // Password Management
        Task<string?> GetUserPasswordHashAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string newPasswordHash, string newPasswordSalt, int? updatedBy);

        // ✅ Password Reset Token Management
        Task SavePasswordResetTokenAsync(int userId, string token, DateTime expiryDate,
            string? ipAddress = null, string? userAgent = null);
        Task<PasswordResetToken?> ValidatePasswordResetTokenAsync(string token);
        Task MarkPasswordResetTokenUsedAsync(string token);
        Task InvalidateAllPasswordResetTokensAsync(int userId);
        Task CleanupExpiredPasswordResetTokensAsync();

        // User Methods
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
        Task<int> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task UpdateUserLastLoginAsync(int userId, DateTime lastLoginDate, string? ipAddress);
        Task<bool> CheckUsernameExistsAsync(string username);
        Task<bool> CheckEmailExistsAsync(string email);

        // Audit
        Task LogAuditAsync(int? userId, string action, string? tableName, int? recordId,
            string? oldValues, string? newValues, string? ipAddress, string? userAgent);
    }
}