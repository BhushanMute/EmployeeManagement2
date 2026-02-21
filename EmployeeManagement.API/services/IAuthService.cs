using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.services
{
    public interface IAuthService
    {
        /// <summary>
        /// Login user with email and password
        /// </summary>
        Task<TokenResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Refresh expired access token using refresh token
        /// </summary>
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Revoke a specific refresh token
        /// </summary>
        Task RevokeTokenAsync(string refreshToken);

        /// <summary>
        /// Revoke all tokens for a user (logout)
        /// </summary>
        Task RevokeAllUserTokensAsync(int userId);

        Task<TokenResponse> RegisterAsync(RegisterRequest request);

        Task<TokenResponse> SocialLoginAsync( string email, string name, string provider, string socialId);
    }
}
