 

using EmployeeManagement.API.Models;
using EmployeeManagement.API.services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace EmployeeManagement.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string TokenKey = "jwt_token";
        private const string RefreshTokenKey = "refresh_token";

        public TokenService(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetTokenAsync()
        {
            var session = _httpContextAccessor.HttpContext.Session;

            var token = session.GetString(TokenKey);

            if (string.IsNullOrEmpty(token))
                return string.Empty;

            if (IsTokenExpired(token))
            {
                var newToken = await RefreshTokenAsync();

                if (!string.IsNullOrEmpty(newToken))
                    return newToken;

                await RemoveTokenAsync();
                return string.Empty;
            }

            return token;
        }

        public async Task<string> RefreshTokenAsync()
        {
            var session = _httpContextAccessor.HttpContext.Session;

            var refreshToken = session.GetString(RefreshTokenKey);

            if (string.IsNullOrEmpty(refreshToken))
                return string.Empty;

            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync(
                "api/auth/refresh",
                new { RefreshToken = refreshToken });

            if (!response.IsSuccessStatusCode)
                return string.Empty;

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            await SetTokenAsync(result.Token);
            await SetRefreshTokenAsync(result.RefreshToken);

            return result.Token;
        }

        public async Task SetTokenAsync(string token)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString(TokenKey, token);

            await Task.CompletedTask;
        }

        public async Task SetRefreshTokenAsync(string refreshToken)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString(RefreshTokenKey, refreshToken);

            await Task.CompletedTask;
        }

        public async Task RemoveTokenAsync()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Remove(TokenKey);
            session.Remove(RefreshTokenKey);

            await Task.CompletedTask;
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                return jwt.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }


}
