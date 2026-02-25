using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Services
{
    public interface IAuthService
    {
        // 🔐 Authentication
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress);
        Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, int? createdBy = null);
        Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress);
        Task<ApiResponse<bool>> RevokeTokenAsync(string token, string? ipAddress);

        // 🔎 Permission Check
        Task<bool> CheckPermissionAsync(int userId, string permissionName);

        // 🔑 Password Management
        Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ApiResponse<bool>> ResetPasswordWithTokenAsync(ResetPasswordWithTokenRequest request);
        Task<ApiResponse<bool>> ResetPasswordByAdminAsync(ResetPasswordRequest request, int adminUserId);
        Task<ApiResponse<bool>> ValidatePasswordResetTokenAsync(string token);
    }
}