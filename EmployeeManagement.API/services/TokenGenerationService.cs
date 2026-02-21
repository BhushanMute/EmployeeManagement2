using EmployeeManagement.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagement.API.services
{
    public class TokenGenerationService : ITokenGenerationService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<TokenGenerationService> _logger;

        public TokenGenerationService(IConfiguration configuration, ILogger<TokenGenerationService> logger)
        {
            _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            _logger = logger;
        }

        public string GenerateAccessToken(User user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    
                    new Claim("userId", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: GetAccessTokenExpiration(),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating access token for user {UserId}", user.Id);
                throw new ApplicationException("Failed to generate access token", ex);
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[64];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                }
                return Convert.ToBase64String(randomNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token");
                throw new ApplicationException("Failed to generate refresh token", ex);
            }
        }

        public DateTime GetAccessTokenExpiration()
        {
            return DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
        }

        public DateTime GetRefreshTokenExpiration()
        {
            return DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        }
    }
}
