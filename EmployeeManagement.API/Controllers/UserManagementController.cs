// File: EmployeeManagement.API/Controllers/UserManagementController.cs
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.UserManagement;
using EmployeeManagement.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensure all endpoints require authentication
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementRepository _userRepo;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IUserManagementRepository userRepo,
            ILogger<UserManagementController> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        private int CurrentUserId =>
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        private string CurrentUserEmail =>
            User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value ?? string.Empty;

        /// <summary>
        /// GET: api/UserManagement/users
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string? search = null,
            [FromQuery] string? role = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("GET users called by user {UserId} ({UserEmail})", CurrentUserId, CurrentUserEmail);

                var result = await _userRepo.GetAllUsersAsync(search, role, status, pageNumber, pageSize);
                return Ok(ApiResponse<UserListResponse>.Success(result, "Users fetched"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users for user {UserId}", CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// GET: api/UserManagement/users/{id}
        /// </summary>
        [HttpGet("users/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userRepo.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(ApiResponse<object>.Fail("User not found"));

                return Ok(ApiResponse<UserDetailResponse>.Success(user, "User found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {Id} by user {UserId}", id, CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// POST: api/UserManagement/users
        /// </summary>
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                _logger.LogInformation("Creating user: {Username} by user {CreatorId}",
                    request?.Username, CurrentUserId);

                if (request == null)
                    return BadRequest(ApiResponse<object>.Fail("Request body is required"));

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<object>.Fail("Validation failed", errors));
                }

                // Hash password
                if (!string.IsNullOrEmpty(request.Password))
                {
                    request.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }
                else
                {
                    return BadRequest(ApiResponse<object>.Fail("Password is required"));
                }

                var result = await _userRepo.CreateUserAsync(request, CurrentUserId);

                if (result.Success)
                {
                    _logger.LogInformation("User created: {Username} (ID: {Id}) by user {CreatorId}",
                        request.Username, result.NewId, CurrentUserId);
                    return Ok(ApiResponse<UserOperationResult>.Success(result, result.Message));
                }

                return BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user by user {UserId}", CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// PUT: api/UserManagement/users/{id}
        /// </summary>
        [HttpPut("users/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponse<object>.Fail("Request body is required"));

                request.UserId = id;
                var result = await _userRepo.UpdateUserAsync(request, CurrentUserId);

                return result.Success
                    ? Ok(ApiResponse<UserOperationResult>.Success(result, result.Message))
                    : BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id} by user {UserId}", id, CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// DELETE: api/UserManagement/users/{id}
        /// </summary>
        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                if (id == CurrentUserId)
                    return BadRequest(ApiResponse<object>.Fail("Cannot delete your own account"));

                var result = await _userRepo.DeleteUserAsync(id, CurrentUserId);
                return result.Success
                    ? Ok(ApiResponse<UserOperationResult>.Success(result, result.Message))
                    : BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {Id} by user {UserId}", id, CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// PUT: api/UserManagement/users/{id}/toggle-status
        /// </summary>
        [HttpPut("users/{id:int}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id, [FromBody] ToggleStatusRequest request)
        {
            try
            {
                var result = await _userRepo.ToggleStatusAsync(id, request.IsActive, CurrentUserId);
                return Ok(ApiResponse<UserOperationResult>.Success(result, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for user {Id} by user {UserId}", id, CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// PUT: api/UserManagement/users/{id}/reset-password
        /// </summary>
        [HttpPut("users/{id:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.NewPassword))
                    return BadRequest(ApiResponse<object>.Fail("Password is required"));

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                var result = await _userRepo.ResetPasswordAsync(id, hashedPassword, CurrentUserId);
                return Ok(ApiResponse<UserOperationResult>.Success(result, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user {Id} by user {UserId}", id, CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        // ============================================================
        // 🛡️ ROLE MANAGEMENT ENDPOINTS (Consistent lowercase routes)
        // ============================================================

        /// <summary>
        /// GET: api/UserManagement/roles
        /// </summary>
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _userRepo.GetAllRolesAsync();
                return Ok(ApiResponse<List<RoleWithCountResponse>>.Success(roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching roles by user {UserId}", CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// ✅ FIXED: POST: api/UserManagement/roles (lowercase, consistent)
        /// Now handles cases where CurrentUserId might be 0
        /// </summary>
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                _logger.LogInformation("Creating role: {RoleName} by user {UserId}",
                    request?.RoleName, CurrentUserId);

                if (request == null)
                    return BadRequest(ApiResponse<object>.Fail("Request body is required"));

                if (string.IsNullOrWhiteSpace(request.RoleName))
                    return BadRequest(ApiResponse<object>.Fail("Role name is required"));

                // Validate that the user is authenticated (CurrentUserId should not be 0)
                if (CurrentUserId == 0)
                {
                    _logger.LogWarning("Unauthorized attempt to create role by unauthenticated user");
                    return Unauthorized(ApiResponse<object>.Fail("Authentication required to create role"));
                }

                // Trim whitespace from role name
                request.RoleName = request.RoleName.Trim();

                // Optional: Additional validation for role name length or format
                if (request.RoleName.Length > 100)
                    return BadRequest(ApiResponse<object>.Fail("Role name cannot exceed 100 characters"));

                var result = await _userRepo.CreateRoleAsync(request, CurrentUserId);

                if (result.Success)
                {
                    _logger.LogInformation("Role created: {RoleName} (ID: {Id}) by user {CreatorId}",
                        request.RoleName, result.NewId, CurrentUserId);
                    return Ok(ApiResponse<UserOperationResult>.Success(result, result.Message));
                }

                _logger.LogWarning("Failed to create role: {Message}", result.Message);
                return BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role by user {UserId}. Exception: {Exception}",
                    CurrentUserId, ex);
                return StatusCode(500, ApiResponse<object>.Fail($"Error creating role: {ex.Message}"));
            }
        }

        /// <summary>
        /// PUT: api/UserManagement/roles/{id}
        /// </summary>
        [HttpPut("roles/{id:int}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                _logger.LogInformation("Updating role ID: {RoleId} by user {UserId}", id, CurrentUserId);

                if (request == null)
                    return BadRequest(ApiResponse<object>.Fail("Request body is required"));

                if (id <= 0)
                    return BadRequest(ApiResponse<object>.Fail("Invalid role ID"));

                // Validate that the user is authenticated
                if (CurrentUserId == 0)
                {
                    _logger.LogWarning("Unauthorized attempt to update role by unauthenticated user");
                    return Unauthorized(ApiResponse<object>.Fail("Authentication required to update role"));
                }

                request.RoleId = id;

                // Trim whitespace from role name
                if (!string.IsNullOrWhiteSpace(request.RoleName))
                    request.RoleName = request.RoleName.Trim();

                var result = await _userRepo.UpdateRoleAsync(request, CurrentUserId);

                if (result.Success)
                {
                    _logger.LogInformation("Role updated: ID {RoleId} by user {UpdaterId}", id, CurrentUserId);
                    return Ok(ApiResponse<UserOperationResult>.Success(result, result.Message));
                }

                return BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {Id} by user {UserId}", id, CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// DELETE: api/UserManagement/roles/{id}
        /// </summary>
        [HttpDelete("roles/{id:int}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                _logger.LogInformation("Deleting role ID: {RoleId} by user {UserId}", id, CurrentUserId);

                if (id <= 0)
                    return BadRequest(ApiResponse<object>.Fail("Invalid role ID"));

                // Validate that the user is authenticated
                if (CurrentUserId == 0)
                {
                    _logger.LogWarning("Unauthorized attempt to delete role by unauthenticated user");
                    return Unauthorized(ApiResponse<object>.Fail("Authentication required to delete role"));
                }

                var result = await _userRepo.DeleteRoleAsync(id);

                if (result.Success)
                {
                    _logger.LogInformation("Role deleted: ID {RoleId} by user {DeleterId}", id, CurrentUserId);
                    return Ok(ApiResponse<UserOperationResult>.Success(result));
                }

                return BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {Id} by user {UserId}", id, CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        // ============================================================
        // 📋 DROPDOWNS
        // ============================================================

        /// <summary>
        /// GET: api/UserManagement/dropdowns
        /// </summary>
        [HttpGet("dropdowns")]
        public async Task<IActionResult> GetDropdowns()
        {
            try
            {
                _logger.LogInformation("Fetching dropdowns by user {UserId}", CurrentUserId);
                var data = await _userRepo.GetDropdownDataAsync();
                return Ok(ApiResponse<UserDropdownData>.Success(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dropdowns by user {UserId}", CurrentUserId);
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}