using EmployeeManagement.API.Models;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized login attempt");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Refresh token endpoint - validates refresh token and generates new tokens
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized refresh token attempt");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new { message = "An error occurred during token refresh" });
            }
        }

        /// <summary>
        /// Revoke refresh token endpoint
        /// </summary>
        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _authService.RevokeTokenAsync(request.RefreshToken);
                return Ok(new { message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token");
                return StatusCode(500, new { message = "An error occurred while revoking token" });
            }
        }

        /// <summary>
        /// Logout endpoint - revokes all user tokens
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                await _authService.RevokeAllUserTokensAsync(userId);
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("social-login")]
        [AllowAnonymous]
        public async Task<IActionResult> SocialLogin(SocialLoginModel request)
        {
            var result = await _authService.SocialLoginAsync(
                request.Email,
                request.Name,
                request.Provider,
                request.SocialId);

            return Ok(result);
        }
    }
}