// Services/IProfileService.cs
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Services
{
    public interface IProfileService
    {
        Task<ApiResponse<ProfileResponse>> GetProfileAsync(int userId);
        Task<ApiResponse<ProfileResponse>> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<ApiResponse<string>> UploadProfilePictureAsync(int userId, IFormFile file);
        Task<ApiResponse<List<UserActivityLog>>> GetActivityLogAsync(int userId, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<List<UserSession>>> GetActiveSessionsAsync(int userId);
        Task<ApiResponse<bool>> RevokeSessionAsync(int userId, string sessionToken);
        Task<ApiResponse<UserSettings>> GetSettingsAsync(int userId);
        Task<ApiResponse<bool>> UpdateSettingsAsync(int userId, UpdateUserSettingsRequest request);
        Task<ApiResponse<ProfileResponse>> GetProfileByIdAsync(int id);  // ✅ Add this
     }
}