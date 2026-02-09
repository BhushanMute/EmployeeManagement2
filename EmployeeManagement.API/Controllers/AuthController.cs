using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;
        private readonly IJwtTokenService _jwt;

        public AuthController(IUserRepository repo, IConfiguration config, IJwtTokenService jwt)
        {
            _repo = repo;
            _config = config;
             _jwt = jwt;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid login request");

            try
            {
                //var passwordhash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                var (user, roles, message) =
                await _repo.LoginAsync(model.Username, model.Password);

                if (user == null)
                    return Unauthorized(new AuthResponse
                    {
                        Token = null,
                        Message = message
                    });

                // ✅ CALL JWT SERVICE HERE
                var token = _jwt.GenerateToken(user, roles);

                return Ok(new AuthResponse
                {
                    Token = token,
                    Message = "Login successful"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error"
                });
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int result = await _repo.RegisterAsync(model);

                if (result == -1)
                    return Conflict(new { success = false, message = "Username already exists" });

                if (result == 0)
                    return BadRequest(new { success = false, message = "Insert failed" });

                return Ok(new { success = true, message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                
                // Log exception if you have a logger
                return StatusCode(500, new { success = false, message = "Unexpected error: " + ex.Message });
            }
        }
    }
}