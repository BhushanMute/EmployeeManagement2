using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    //public class AuthController : ControllerBase
    //{
    //    private readonly IAuthService _authService;
    //    private readonly ILogger<AuthController> _logger;

    //    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    //    {
    //        _authService = authService;
    //        _logger = logger;
    //    }


    //    [HttpPost("login")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }


    //            var response = await _authService.LoginAsync(request);
    //            return Ok(response);
    //        }
    //        catch (UnauthorizedAccessException ex)
    //        {
    //            _logger.LogWarning(ex, "Unauthorized login attempt");
    //            return Unauthorized(new { message = ex.Message });
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error during login");
    //            return StatusCode(500, new { message = "An error occurred during login" });
    //        }
    //    }

    //    /// <summary>
    //    /// Refresh token endpoint - validates refresh token and generates new tokens
    //    /// </summary>
    //    [HttpPost("refresh")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }

    //            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
    //            return Ok(response);
    //        }
    //        catch (UnauthorizedAccessException ex)
    //        {
    //            _logger.LogWarning(ex, "Unauthorized refresh token attempt");
    //            return Unauthorized(new { message = ex.Message });
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error during token refresh");
    //            return StatusCode(500, new { message = "An error occurred during token refresh" });
    //        }
    //    }

    //    /// <summary>
    //    /// Revoke refresh token endpoint
    //    /// </summary>
    //    [HttpPost("revoke")]
    //    [Authorize]
    //    public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }

    //            await _authService.RevokeTokenAsync(request.RefreshToken);
    //            return Ok(new { message = "Token revoked successfully" });
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error revoking token");
    //            return StatusCode(500, new { message = "An error occurred while revoking token" });
    //        }
    //    }

    //    /// <summary>
    //    /// Logout endpoint - revokes all user tokens
    //    /// </summary>
    //    [HttpPost("logout")]
    //    [Authorize]
    //    public async Task<IActionResult> Logout()
    //    {
    //        try
    //        {
    //            var userIdClaim = User.FindFirst("userId")?.Value;

    //            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
    //            {
    //                return Unauthorized(new { message = "Invalid user token" });
    //            }

    //            await _authService.RevokeAllUserTokensAsync(userId);
    //            return Ok(new { message = "Logged out successfully" });
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error during logout");
    //            return StatusCode(500, new { message = "An error occurred during logout" });
    //        }
    //    }

    //    [HttpPost("register")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> Register(RegisterRequest request)
    //    {
    //        var result = await _authService.RegisterAsync(request);
    //        return Ok(result);
    //    }

    //    [HttpPost("social-login")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> SocialLogin(SocialLoginModel request)
    //    {
    //        var result = await _authService.SocialLoginAsync(
    //            request.Email,
    //            request.Name,
    //            request.Provider,
    //            request.SocialId);

    //        return Ok(result);
    //    }
    //}
    //}

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

        private string? GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
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
        
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<bool>.Fail("Please provide a valid email address"));
                }

                _logger.LogInformation("Forgot password request for email: {Email}", request.Email);

                var result = await _authService.ForgotPasswordAsync(request);

                // Always return success to prevent email enumeration
                return Ok(ApiResponse<bool>.Success(true, "If the email exists, a password reset link will be sent"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password for email: {Email}", request.Email);
                // Still return success to prevent email enumeration
                return Ok(ApiResponse<bool>.Success(true, "If the email exists, a password reset link will be sent"));
            }
        }

        /// <summary>
        /// Reset Password - Using reset token
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordWithTokenRequest request)
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

                _logger.LogInformation("Password reset attempt for email: {Email}", request.Email);

                var result = await _authService.ResetPasswordWithTokenAsync(request);

                if (!result.Status)
                {
                    _logger.LogWarning("Password reset failed for email: {Email}", request.Email);
                    return BadRequest(result);
                }

                _logger.LogInformation("Password reset successful for email: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for email: {Email}", request.Email);
                return StatusCode(500, ApiResponse<bool>.Fail("An error occurred while resetting password"));
            }
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
        public async Task<ActionResult<ApiResponse<bool>>> ValidateResetToken([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating reset token");
                return StatusCode(500, ApiResponse<bool>.Fail("An error occurred while validating token"));
            }
        }

        #endregion

    }

}