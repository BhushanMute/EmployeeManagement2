using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.services;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.API.Services
{
    public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenGenerationService _tokenGenerationService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenGenerationService tokenGenerationService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenGenerationService = tokenGenerationService;
        _logger = logger;
    }

        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            bool isValid = BCrypt.Net.BCrypt.EnhancedVerify(
                request.Password,
                user.PasswordHash
            );

            if (!isValid)
                throw new UnauthorizedAccessException("Invalid credentials");

            var accessToken = _tokenGenerationService.GenerateAccessToken(user);
            var refreshToken = _tokenGenerationService.GenerateRefreshToken();

            await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            });

            return new TokenResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(15)
            };
        }
        // ✅ REFRESH TOKEN
        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);

        if (storedToken == null ||
            storedToken.IsRevoked ||
            storedToken.ExpiryDate <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var user = await _userRepository.GetUserByEmailAsync(
            (await _userRepository.GetUserByEmailAsync(storedToken.UserId.ToString()))?.Email
        );

        var newAccessToken = _tokenGenerationService.GenerateAccessToken(user);
        var newRefreshToken = _tokenGenerationService.GenerateRefreshToken();

        // Rotate token
        await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken, newRefreshToken);

        await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
        {
            UserId = storedToken.UserId,
            Token = newRefreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        });

        return new TokenResponse
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(15)
        };
    }

    // ✅ REVOKE ONE TOKEN
    public async Task RevokeTokenAsync(string refreshToken)
    {
        await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);
    }

    // ✅ REVOKE ALL TOKENS (Logout All Devices)
    public async Task RevokeAllUserTokensAsync(int userId)
    {
        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
    }
        public async Task<TokenResponse> RegisterAsync(RegisterRequest request)
        {
            // Call repository to create user
            var userId = await _userRepository.RegisterAsync(request);

            if (userId <= 0)
                throw new Exception("User registration failed");

            // Get created user
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
                throw new Exception("User retrieval failed after registration");

            // Generate tokens
            var accessToken = _tokenGenerationService.GenerateAccessToken(user);
            var refreshToken = _tokenGenerationService.GenerateRefreshToken();

            await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            });

            return new TokenResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public async Task<TokenResponse> SocialLoginAsync(
    string email,
    string name,
    string provider,
    string socialId)
        {
            // Call repository (ADO.NET SP)
            var user = await _userRepository.SocialLogin(email, provider, socialId);

            if (user == null)
                throw new UnauthorizedAccessException("Social login failed");

            // Convert UserModel to User (for token generation)
            var tokenUser = new User
            {
                Id = user.Id,
                Email = user.Email
            };

            // Generate tokens
            var accessToken = _tokenGenerationService.GenerateAccessToken(tokenUser);
            var refreshToken = _tokenGenerationService.GenerateRefreshToken();

            await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            });

            return new TokenResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(15)
            };
        }
    }
}