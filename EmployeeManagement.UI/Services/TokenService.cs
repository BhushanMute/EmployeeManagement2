 

using EmployeeManagement.API.Models;
using EmployeeManagement.API.services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;


namespace EmployeeManagement.UI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TokenKey = "jwt_token";
        private const string RefreshTokenKey = "refresh_token";

        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetTokenAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
                return string.Empty;

            var token = session.GetString(TokenKey);

            if (string.IsNullOrEmpty(token))
                return string.Empty;

            // Check if token is expired
            if (IsTokenExpired(token))
            {
                await RemoveTokenAsync();
                return string.Empty;
            }

            return token;
        }

        public async Task SetTokenAsync(string token)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString(TokenKey, token);
            }
            await Task.CompletedTask;
        }

        public async Task<string> GetRefreshTokenAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString(RefreshTokenKey) ?? string.Empty;
        }

        public async Task SetRefreshTokenAsync(string token)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString(RefreshTokenKey, token);
            }
            await Task.CompletedTask;
        }

        public async Task RemoveTokenAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.Remove(TokenKey);
                session.Remove(RefreshTokenKey);
            }
            await Task.CompletedTask;
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }


}
