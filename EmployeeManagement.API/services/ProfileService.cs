// Services/ProfileService.cs
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;

namespace EmployeeManagement.API.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger<ProfileService> _logger;
        private readonly IWebHostEnvironment _environment;

        public ProfileService(
            IProfileRepository profileRepository,
            ILogger<ProfileService> logger,
            IWebHostEnvironment environment)
        {
            _profileRepository = profileRepository;
            _logger = logger;
            _environment = environment;
        }

        public async Task<ApiResponse<ProfileResponse>> GetProfileAsync(int userId)
        {
            try
            {
                var profile = await _profileRepository.GetUserProfileAsync(userId);

                if (profile == null)
                {
                    return ApiResponse<ProfileResponse>.Fail("Profile not found");
                }

                return ApiResponse<ProfileResponse>.Success(profile, "Profile retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile for user {UserId}", userId);
                return ApiResponse<ProfileResponse>.Fail("An error occurred while retrieving profile");
            }
        }

        public async Task<ApiResponse<ProfileResponse>> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            try
            {
                var (success, message) = await _profileRepository.UpdateUserProfileAsync(userId, request, userId);

                if (!success)
                {
                    return ApiResponse<ProfileResponse>.Fail(message);
                }

                // Get updated profile
                var profile = await _profileRepository.GetUserProfileAsync(userId);

                return ApiResponse<ProfileResponse>.Success(profile!, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                return ApiResponse<ProfileResponse>.Fail("An error occurred while updating profile");
            }
        }

        public async Task<ApiResponse<string>> UploadProfilePictureAsync(int userId, IFormFile file)
        {
            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                {
                    return ApiResponse<string>.Fail("No file uploaded");
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    return ApiResponse<string>.Fail("Invalid file type. Only JPG, PNG, and GIF are allowed");
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return ApiResponse<string>.Fail("File size must be less than 5MB");
                }

                // Generate unique filename
                var fileName = $"user_{userId}_{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profiles");

                // Create directory if not exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Delete old profile picture if exists
                var user = await _profileRepository.GetUserProfileAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.ProfilePicture))
                {
                    var oldFileName = Path.GetFileName(user.ProfilePicture);
                    var oldFilePath = Path.Combine(uploadsFolder, oldFileName);

                    if (File.Exists(oldFilePath))
                    {
                        try
                        {
                            File.Delete(oldFilePath);
                            _logger.LogInformation("Deleted old profile picture: {OldFile}", oldFileName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete old profile picture: {OldFile}", oldFileName);
                        }
                    }
                }

                // Save new file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("Profile picture saved: {FileName}", fileName);

                // Get relative URL (without domain)
                var fileUrl = $"/uploads/profiles/{fileName}";

                // Update database
                var (success, message) = await _profileRepository.UpdateProfilePictureAsync(userId, fileUrl, userId);

                if (!success)
                {
                    // Delete file if database update failed
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    return ApiResponse<string>.Fail(message);
                }

                // Return full URL for UI
                var fullUrl = $"{GetBaseUrl()}{fileUrl}";

                return ApiResponse<string>.Success(fullUrl, "Profile picture uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture for user {UserId}", userId);
                return ApiResponse<string>.Fail("An error occurred while uploading profile picture");
            }
        }

        public async Task<ApiResponse<List<UserActivityLog>>> GetActivityLogAsync(int userId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var logs = await _profileRepository.GetUserActivityLogAsync(userId, pageNumber, pageSize);
                return ApiResponse<List<UserActivityLog>>.Success(logs, "Activity log retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activity log for user {UserId}", userId);
                return ApiResponse<List<UserActivityLog>>.Fail("An error occurred while retrieving activity log");
            }
        }

        public async Task<ApiResponse<List<UserSession>>> GetActiveSessionsAsync(int userId)
        {
            try
            {
                var sessions = await _profileRepository.GetUserSessionsAsync(userId);
                return ApiResponse<List<UserSession>>.Success(sessions, "Sessions retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for user {UserId}", userId);
                return ApiResponse<List<UserSession>>.Fail("An error occurred while retrieving sessions");
            }
        }

        public async Task<ApiResponse<bool>> RevokeSessionAsync(int userId, string sessionToken)
        {
            try
            {
                var (success, message) = await _profileRepository.RevokeUserSessionAsync(userId, sessionToken);

                if (!success)
                {
                    return ApiResponse<bool>.Fail(message);
                }

                return ApiResponse<bool>.Success(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking session for user {UserId}", userId);
                return ApiResponse<bool>.Fail("An error occurred while revoking session");
            }
        }

        public async Task<ApiResponse<UserSettings>> GetSettingsAsync(int userId)
        {
            try
            {
                var settings = await _profileRepository.GetUserSettingsAsync(userId);

                if (settings == null)
                {
                    settings = new UserSettings { UserId = userId };
                }

                return ApiResponse<UserSettings>.Success(settings, "Settings retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting settings for user {UserId}", userId);
                return ApiResponse<UserSettings>.Fail("An error occurred while retrieving settings");
            }
        }

        public async Task<ApiResponse<bool>> UpdateSettingsAsync(int userId, UpdateUserSettingsRequest request)
        {
            try
            {
                var (success, message) = await _profileRepository.UpdateUserSettingsAsync(userId, request);

                if (!success)
                {
                    return ApiResponse<bool>.Fail(message);
                }

                return ApiResponse<bool>.Success(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating settings for user {UserId}", userId);
                return ApiResponse<bool>.Fail("An error occurred while updating settings");
            }
        }
        // Add this method in ProfileService.cs

        public async Task<ApiResponse<ProfileResponse>> GetProfileByIdAsync(int id)
        {
            try
            {
                var profile = await _profileRepository.GetProfileByIdAsync(id);

                if (profile == null)
                {
                    return ApiResponse<ProfileResponse>.Fail("Profile not found");
                }

                return ApiResponse<ProfileResponse>.Success(profile, "Profile retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile by ID: {Id}", id);
                return ApiResponse<ProfileResponse>.Fail("An error occurred while retrieving profile");
            }
        }
        // Helper method to get base URL
        private string GetBaseUrl()
        {
            // You can inject IHttpContextAccessor to get current request URL
            // For now, return empty string (relative URL)
            return "";
        }

    }
}