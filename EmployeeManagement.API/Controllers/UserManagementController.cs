// File: EmployeeManagement.API/Controllers/UserManagementController.cs
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
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
 
        /// <summary>
        /// GET: api/UserManagement/users
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers( [FromQuery] string? search = null, [FromQuery] string? role = null, [FromQuery] string? status = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("GET users called by user {UserId}", CurrentUserId);

                var result = await _userRepo.GetAllUsersAsync(search, role, status, pageNumber, pageSize);
                return Ok(ApiResponse<UserListResponse>.Success(result, "Users fetched"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
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
                _logger.LogError(ex, "Error fetching user {Id}", id);
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
                _logger.LogInformation("Creating user: {Username}", request?.Username);

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
                    _logger.LogInformation("User created: {Username} (ID: {Id})", request.Username, result.NewId);
                    return Ok(ApiResponse<UserOperationResult>.Success(result, result.Message));
                }

                return BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
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
                _logger.LogError(ex, "Error updating user {Id}", id);
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
                _logger.LogError(ex, "Error fetching roles");
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// ✅ FIXED: POST: api/UserManagement/roles (lowercase, consistent)
        /// </summary>
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.RoleName))
                    return BadRequest(ApiResponse<object>.Fail("Role name is required"));

                var result = await _userRepo.CreateRoleAsync(request, CurrentUserId);
                return result.Success
                    ? Ok(ApiResponse<UserOperationResult>.Success(result, result.Message))
                    : BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
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
                if (request == null)
                    return BadRequest(ApiResponse<object>.Fail("Request body is required"));

                request.RoleId = id;
                var result = await _userRepo.UpdateRoleAsync(request, CurrentUserId);

                return result.Success
                    ? Ok(ApiResponse<UserOperationResult>.Success(result, result.Message))
                    : BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
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
                var result = await _userRepo.DeleteRoleAsync(id);
                return result.Success
                    ? Ok(ApiResponse<UserOperationResult>.Success(result))
                    : BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            catch (Exception ex)
            {
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
                var data = await _userRepo.GetDropdownDataAsync();
                return Ok(ApiResponse<UserDropdownData>.Success(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dropdowns");
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}