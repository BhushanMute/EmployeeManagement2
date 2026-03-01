using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IProfileRepository
    {
        Task<ProfileResponse?> GetUserProfileAsync(int userId);
        Task<(bool Success, string Message)> UpdateUserProfileAsync(int userId, UpdateProfileRequest request, int? updatedBy);
        Task<(bool Success, string Message)> UpdateProfilePictureAsync(int userId, string profilePictureUrl, int? updatedBy);
        Task<List<UserActivityLog>> GetUserActivityLogAsync(int userId, int pageNumber, int pageSize);
        Task<List<UserSession>> GetUserSessionsAsync(int userId);
        Task<(bool Success, string Message)> RevokeUserSessionAsync(int userId, string sessionToken);
        Task<UserSettings?> GetUserSettingsAsync(int userId);
        Task<(bool Success, string Message)> UpdateUserSettingsAsync(int userId, UpdateUserSettingsRequest request);
        Task<ProfileResponse?> GetProfileByIdAsync(int id);  // ✅ Add this line

    }

}
