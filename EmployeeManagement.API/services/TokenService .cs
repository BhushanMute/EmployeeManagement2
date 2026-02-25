using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagement.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateAccessToken(User user, List<Role> roles, List<Permission> permissions)
        {
            // ✅ Use YOUR configuration keys
            var jwtKey = _configuration["JwtSettings:SecretKey"];
            var jwtIssuer = _configuration["JwtSettings:Issuer"];
            var jwtAudience = _configuration["JwtSettings:Audience"];
            var expiryMinutes = _configuration["JwtSettings:AccessTokenExpirationMinutes"];

            // Validate configuration
            if (string.IsNullOrEmpty(jwtKey))
            {
                _logger.LogError("JWT SecretKey is not configured in appsettings.json");
                throw new InvalidOperationException("JWT SecretKey is not configured. Please add 'JwtSettings:SecretKey' to appsettings.json");
            }

            if (jwtKey.Length < 32)
            {
                _logger.LogError("JWT SecretKey must be at least 32 characters long");
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
            }

            // Build claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("FullName", $"{user.FirstName ?? ""} {user.LastName ?? ""}".Trim()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            // Add role claims
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (!string.IsNullOrEmpty(role.RoleName))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                    }
                }
            }

            // Add permission claims
            if (permissions != null)
            {
                foreach (var permission in permissions)
                {
                    if (!string.IsNullOrEmpty(permission.PermissionName))
                    {
                        claims.Add(new Claim("Permission", permission.PermissionName));
                    }
                }
            }

            // Create token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(
                double.TryParse(expiryMinutes, out var minutes) ? minutes : 15
            );

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Generated access token for user {UserId}", user.Id);

            return tokenString;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"]
                    ?? throw new InvalidOperationException("JWT SecretKey not configured");

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = false // We want to validate even expired tokens
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogWarning("Invalid token algorithm");
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating expired token");
                return null;
            }
        }

        public DateTime GetAccessTokenExpiry()
        {
            var expiryMinutes = _configuration["JwtSettings:AccessTokenExpirationMinutes"];
            return DateTime.UtcNow.AddMinutes(
                double.TryParse(expiryMinutes, out var minutes) ? minutes : 15
            );
        }

        public DateTime GetRefreshTokenExpiry()
        {
            var expiryDays = _configuration["JwtSettings:RefreshTokenExpirationDays"];
            return DateTime.UtcNow.AddDays(
                double.TryParse(expiryDays, out var days) ? days : 7
            );
        }
    }
}