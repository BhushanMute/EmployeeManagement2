// Controllers/ProfileController.cs
using EmployeeManagement.API.Attributes;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IProfileService profileService, ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ProfileResponse>>> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();

                if (userId == 0)
                {
                    _logger.LogWarning("Unauthorized access attempt - UserId is 0");
                    return Unauthorized(ApiResponse<ProfileResponse>.Fail("User not authenticated"));
                }

                _logger.LogInformation("Getting profile for user {UserId}", userId);

                var result = await _profileService.GetProfileAsync(userId);

                if (!result.Status)
                {
                    _logger.LogWarning("Profile not found for user {UserId}", userId);
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile");
                return StatusCode(500, ApiResponse<ProfileResponse>.Fail("An error occurred while retrieving profile"));
            }
        }

        // Add this endpoint in API ProfileController.cs

        /// <summary>
        /// Get profile by ID
        /// </summary>

        [HttpGet("{id}")]
        [AuthorizePermission("Employee.Update")]

        public async Task<ActionResult<ApiResponse<ProfileResponse>>> GetProfileById(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (currentUserId == 0)
                {
                    _logger.LogWarning("Unauthorized access attempt - UserId is 0");
                    return Unauthorized(ApiResponse<ProfileResponse>.Fail("User not authenticated"));
                }

                // ✅ Security: User can only access their own profile OR admin can access any
                if (currentUserId != id && !User.IsInRole("Admin"))
                {
                    _logger.LogWarning("User {CurrentUserId} attempted to access profile {Id}", currentUserId, id);
                    return Forbid();
                }

                _logger.LogInformation("Getting profile by ID: {Id}", id);

                var result = await _profileService.GetProfileByIdAsync(id);

                if (!result.Status)
                {
                    _logger.LogWarning("Profile not found for ID: {Id}", id);
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile by ID: {Id}", id);
                return StatusCode(500, ApiResponse<ProfileResponse>.Fail("An error occurred while retrieving profile"));
            }
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
         
        [HttpPut("UpdateProfile")]
        public async Task<ActionResult<ApiResponse<ProfileResponse>>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning("Invalid model state for profile update: {Errors}", string.Join(", ", errors));
                    return BadRequest(ApiResponse<ProfileResponse>.Fail("Validation failed", errors));
                }

                var userId = GetCurrentUserId();

                if (userId == 0)
                {
                    _logger.LogWarning("Unauthorized profile update attempt");
                    return Unauthorized(ApiResponse<ProfileResponse>.Fail("User not authenticated"));
                }

                _logger.LogInformation("Updating profile for user {UserId}", userId);

                var result = await _profileService.UpdateProfileAsync(userId, request);

                if (!result.Status)
                {
                    _logger.LogWarning("Profile update failed for user {UserId}: {Message}", userId, result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("Profile updated successfully for user {UserId}", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", GetCurrentUserId());
                return StatusCode(500, ApiResponse<ProfileResponse>.Fail("An error occurred while updating profile"));
            }
        }
 
        [HttpPost("UploadProfilePicture")]
        [RequestSizeLimit(5242880)] // 5MB limit
        public async Task<ActionResult<ApiResponse<string>>> UploadProfilePicture([FromForm] IFormFile file)
        {
            try
            {
                _logger.LogInformation("=== API UploadProfilePicture Called ===");

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("No file received in API");
                    return BadRequest(ApiResponse<string>.Fail("No file uploaded"));
                }

                _logger.LogInformation("File received: {FileName}, Size: {Size} bytes, ContentType: {ContentType}",
                    file.FileName, file.Length, file.ContentType);

                // Validate file size (5MB max)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(ApiResponse<string>.Fail("File size must be less than 5MB"));
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(ApiResponse<string>.Fail("Only JPG, PNG, and GIF files are allowed"));
                }

                var userId = GetCurrentUserId();

                if (userId == 0)
                {
                    _logger.LogWarning("User not authenticated");
                    return Unauthorized(ApiResponse<string>.Fail("User not authenticated"));
                }

                _logger.LogInformation("Uploading profile picture for user {UserId}", userId);

                var result = await _profileService.UploadProfilePictureAsync(userId, file);

                if (!result.Status)
                {
                    _logger.LogWarning("Upload failed: {Message}", result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("Profile picture uploaded successfully: {Url}", result.Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture");
                return StatusCode(500, ApiResponse<string>.Fail("An error occurred while uploading profile picture"));
            }
        }
      
        [HttpGet("activity-log")]
        public async Task<ActionResult<ApiResponse<List<UserActivityLog>>>> GetActivityLog(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (userId == 0)
                {
                    _logger.LogWarning("Unauthorized activity log access attempt");
                    return Unauthorized(ApiResponse<List<UserActivityLog>>.Fail("User not authenticated"));
                }

                // Validate pagination parameters
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                _logger.LogInformation("Getting activity log for user {UserId}, Page: {PageNumber}, Size: {PageSize}",
                    userId, pageNumber, pageSize);

                var result = await _profileService.GetActivityLogAsync(userId, pageNumber, pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activity log for user {UserId}", GetCurrentUserId());
                return StatusCode(500, ApiResponse<List<UserActivityLog>>.Fail("An error occurred while retrieving activity log"));
            }
        }
 
        [HttpGet("sessions")]
        public async Task<ActionResult<ApiResponse<List<UserSession>>>> GetActiveSessions()
        {
            try
            {
                var userId = GetCurrentUserId();

                if (userId == 0)
                {
                    _logger.LogWarning("Unauthorized sessions access attempt");
                    return Unauthorized(ApiResponse<List<UserSession>>.Fail("User not authenticated"));
                }

                _logger.LogInformation("Getting active sessions for user {UserId}", userId);

                var result = await _profileService.GetActiveSessionsAsync(userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for user {UserId}", GetCurrentUserId());
                return StatusCode(500, ApiResponse<List<UserSession>>.Fail("An error occurred while retrieving sessions"));
            }
        }

        
        [HttpDelete("sessions/{sessionToken}")]
        public async Task<ActionResult<ApiResponse<bool>>> RevokeSession(string sessionToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sessionToken))
                {
                    _logger.LogWarning("Invalid session token provided for revocation");
                    return BadRequest(ApiResponse<bool>.Fail("Invalid session token"));
                }

                var userId = GetCurrentUserId();

                if (userId == 0)
                {
                    _logger.LogWarning("Unauthorized session revocation attempt");
                    return Unauthorized(ApiResponse<bool>.Fail("User not authenticated"));
                }

                _logger.LogInformation("Revoking session for user {UserId}", userId);

                var result = await _profileService.RevokeSessionAsync(userId, sessionToken);

                if (!result.Status)
                {
                    _logger.LogWarning("Session revocation failed for user {UserId}: {Message}",
                        userId, result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("Session revoked successfully for user {UserId}", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking session for user {UserId}", GetCurrentUserId());
                return StatusCode(500, ApiResponse<bool>.Fail("An error occurred while revoking session"));
            }
        }
 
        [HttpGet("settings")]
        public async Task<ActionResult<ApiResponse<UserSettings>>> GetSettings()
        {
            try
            {
                var userId = GetCurrentUserId();

                if (userId == 0)
                {
                    _logger.LogWarning("Unauthorized settings access attempt");
                    return Unauthorized(ApiResponse<UserSettings>.Fail("User not authenticated"));
                }

                _logger.LogInformation("Getting settings for user {UserId}", userId);

                var result = await _profileService.GetSettingsAsync(userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting settings for user {UserId}", GetCurrentUserId());
                return StatusCode(500, ApiResponse<UserSettings>.Fail("An error occurred while retrieving settings"));
            }
        }

       
        [HttpPut("settings")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateSettings([FromBody] UpdateUserSettingsRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning("Invalid model state for settings update: {Errors}", string.Join(", ", errors));
                    return BadRequest(ApiResponse<bool>.Fail("Validation failed", errors));
                }

                var userId = GetCurrentUserId();

                if (userId == 0)
                {
                    _logger.LogWarning("Unauthorized settings update attempt");
                    return Unauthorized(ApiResponse<bool>.Fail("User not authenticated"));
                }

                _logger.LogInformation("Updating settings for user {UserId}", userId);

                var result = await _profileService.UpdateSettingsAsync(userId, request);

                if (!result.Status)
                {
                    _logger.LogWarning("Settings update failed for user {UserId}: {Message}",
                        userId, result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("Settings updated successfully for user {UserId}", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating settings for user {UserId}", GetCurrentUserId());
                return StatusCode(500, ApiResponse<bool>.Fail("An error occurred while updating settings"));
            }
        }

     
        private int GetCurrentUserId()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("User ID claim not found in token");
                    return 0;
                }

                if (int.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }

                _logger.LogWarning("Failed to parse user ID claim: {Claim}", userIdClaim);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting user ID from claims");
                return 0;
            }
        }
        /// <summary>
        /// Upload profile picture
          
    }
}