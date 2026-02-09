using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
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

        public AuthController(IUserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var (user, message) = await _repo.LoginAsync(model.Username, model.Password);

                if (user == null)
                    return Ok(new AuthResponse { Token = null, Message = message });

                var jwtKey = _config["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                    return StatusCode(500, "JWT Key missing");

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"])),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new AuthResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                // Log exception if you have a logger
                return StatusCode(500, new { success = false, message = "Unexpected error: " + ex.Message });
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