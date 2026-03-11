using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// User Login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest  request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AuthResponse >.Fail("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var ipAddress = GetIpAddress();
            var result = await _authService.LoginAsync(request, ipAddress);

            if (!result.Status)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// User Registration
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponse >>> Register([FromBody] RegisterRequest  request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AuthResponse >.Fail("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Register user with specific role (Admin only)
        /// </summary>
        [HttpPost("register-with-role")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ApiResponse<AuthResponse >>> RegisterWithRole([FromBody] RegisterRequest  request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AuthResponse >.Fail("Validation failed",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var createdBy = GetCurrentUserId();
            var result = await _authService.RegisterAsync(request, createdBy);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponse >>> RefreshToken([FromBody] RefreshTokenRequest  request)
        {
            var ipAddress = GetIpAddress();
            var result = await _authService.RefreshTokenAsync(request, ipAddress);

            if (!result.Status)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Revoke Token (Logout)
        /// </summary>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            var ipAddress = GetIpAddress();
            var result = await _authService.RevokeTokenAsync(request.RefreshToken, ipAddress);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get Current User Info
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<ApiResponse<object>> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var fullName = User.FindFirst("FullName")?.Value;
            var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
            var permissions = User.FindAll("Permission").Select(c => c.Value).ToList();

            var userInfo = new
            {
                UserId = userId,
                Username = username,
                Email = email,
                FullName = fullName,
                Roles = roles,
                Permissions = permissions
            };

            return Ok(ApiResponse<object>.Success(userInfo));
        }

        

        private int GetCurrentUserId()
        {

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
        #region Password Management Endpoints

        /// <summary>
        /// Change Password - For authenticated users
        /// </summary>
        [HttpPost("change-password")]
         
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<bool>.Fail("Validation failed", errors));
                }

                var userId = GetCurrentUserId();

                if (userId == 0)
                {
                    return Unauthorized(ApiResponse<bool>.Fail("User not authenticated"));
                }

                _logger.LogInformation("Password change attempt for user: {UserId}", userId);

                var result = await _authService.ChangePasswordAsync(userId, request);

                if (!result.Status)
                {
                    _logger.LogWarning("Password change failed for user: {UserId}", userId);
                    return BadRequest(result);
                }

                _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, ApiResponse<bool>.Fail("An error occurred while changing password"));
            }
        }

        /// <summary>
        /// Forgot Password - Send reset link to email
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<bool>.Fail("Validation failed", errors));
            }

            var ipAddress = GetIpAddress();
            var userAgent = Request.Headers["User-Agent"].ToString();

            var result = await _authService.ForgotPasswordAsync(request);

            // Always return OK to prevent email enumeration
            return Ok(result);
        }


        /// <summary>
        /// Reset Password - Using reset token
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordWithTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<bool>.Fail("Validation failed", errors));
            }

            _logger.LogInformation("Password reset request for email: {Email}", request.Email);

            var result = await _authService.ResetPasswordWithTokenAsync(request);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Reset Password by Admin - Admin can reset any user's password
        /// </summary>
        [HttpPost("reset-password-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPasswordByAdmin([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<bool>.Fail("Validation failed", errors));
                }

                var adminUserId = GetCurrentUserId();

                _logger.LogInformation("Admin {AdminId} attempting to reset password for user {UserId}",
                    adminUserId, request.UserId);

                var result = await _authService.ResetPasswordByAdminAsync(request, adminUserId);

                if (!result.Status)
                {
                    _logger.LogWarning("Admin password reset failed for user {UserId}", request.UserId);
                    return BadRequest(result);
                }

                _logger.LogInformation("Admin {AdminId} reset password for user {UserId}",
                    adminUserId, request.UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in admin password reset for user {UserId}", request.UserId);
                return StatusCode(500, ApiResponse<bool>.Fail("An error occurred while resetting password"));
            }
        }

        /// <summary>
        /// Validate Reset Token - Check if token is valid
        /// </summary>
        [HttpGet("validate-reset-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateResetToken([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(ApiResponse<bool>.Fail("Token is required"));
            }

            var result = await _authService.ValidatePasswordResetTokenAsync(token);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        #endregion

        #region Helper Methods

        private string? GetIpAddress()
        {
            // Check for forwarded IP (when behind proxy/load balancer)
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"].ToString().Split(',').FirstOrDefault()?.Trim();
            }

            // Check for real IP header
            if (Request.Headers.ContainsKey("X-Real-IP"))
            {
                return Request.Headers["X-Real-IP"].ToString();
            }

            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }

        #endregion

    }

}