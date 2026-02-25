using DocumentFormat.OpenXml.Spreadsheet;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.services;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using ITokenService = EmployeeManagement.API.Repositories.ITokenService;

namespace EmployeeManagement.API.Services
{
    public class AuthService : IAuthService
{
        //private readonly IUserRepository _userRepository;
        //private readonly IRefreshTokenRepository _refreshTokenRepository;
        //private readonly ITokenGenerationService _tokenGenerationService;
        //private readonly ILogger<AuthService> _logger;

        //public AuthService(
        //    IUserRepository userRepository,
        //    IRefreshTokenRepository refreshTokenRepository,
        //    ITokenGenerationService tokenGenerationService,
        //    ILogger<AuthService> logger)
        //{
        //    _userRepository = userRepository;
        //    _refreshTokenRepository = refreshTokenRepository;
        //    _tokenGenerationService = tokenGenerationService;
        //    _logger = logger;
        //}

        //    public async Task<TokenResponse> LoginAsync(LoginRequest request)
        //    {
        //        var user = await _userRepository.GetUserByEmailAsync(request.Email);

        //        if (user == null)
        //            throw new UnauthorizedAccessException("Invalid credentials");

        //        bool isValid = BCrypt.Net.BCrypt.EnhancedVerify(
        //            request.Password,
        //            user.PasswordHash
        //        );

        //        if (!isValid)
        //            throw new UnauthorizedAccessException("Invalid credentials");

        //        var accessToken = _tokenGenerationService.GenerateAccessToken(user);
        //        var refreshToken = _tokenGenerationService.GenerateRefreshToken();

        //        await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
        //        {
        //            UserId = user.Id,
        //            Token = refreshToken,
        //            ExpiryDate = DateTime.UtcNow.AddDays(7)
        //        });

        //        return new TokenResponse
        //        {
        //            Token = accessToken,
        //            RefreshToken = refreshToken,
        //            Expiration = DateTime.UtcNow.AddMinutes(15)
        //        };
        //    }
        //    // ✅ REFRESH TOKEN
        //    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        //{
        //    var storedToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);

        //    if (storedToken == null ||
        //        storedToken.IsRevoked ||
        //        storedToken.ExpiryDate <= DateTime.UtcNow)
        //    {
        //        throw new UnauthorizedAccessException("Invalid refresh token");
        //    }

        //    var user = await _userRepository.GetUserByEmailAsync(
        //        (await _userRepository.GetUserByEmailAsync(storedToken.UserId.ToString()))?.Email
        //    );

        //    var newAccessToken = _tokenGenerationService.GenerateAccessToken(user);
        //    var newRefreshToken = _tokenGenerationService.GenerateRefreshToken();

        //    // Rotate token
        //    await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken, newRefreshToken);

        //    await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
        //    {
        //        UserId = storedToken.UserId,
        //        Token = newRefreshToken,
        //        ExpiryDate = DateTime.UtcNow.AddDays(7)
        //    });

        //    return new TokenResponse
        //    {
        //        Token = newAccessToken,
        //        RefreshToken = newRefreshToken,
        //        Expiration = DateTime.UtcNow.AddMinutes(15)
        //    };
        //}

        //// ✅ REVOKE ONE TOKEN
        //public async Task RevokeTokenAsync(string refreshToken)
        //{
        //    await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);
        //}

        //// ✅ REVOKE ALL TOKENS (Logout All Devices)
        //public async Task RevokeAllUserTokensAsync(int userId)
        //{
        //    await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
        //}
        //    public async Task<TokenResponse> RegisterAsync(RegisterRequest request)
        //    {
        //        // Call repository to create user
        //        var userId = await _userRepository.RegisterAsync(request);

        //        if (userId <= 0)
        //            throw new Exception("User registration failed");

        //        // Get created user
        //        var user = await _userRepository.GetUserByEmailAsync(request.Email);

        //        if (user == null)
        //            throw new Exception("User retrieval failed after registration");

        //        // Generate tokens
        //        var accessToken = _tokenGenerationService.GenerateAccessToken(user);
        //        var refreshToken = _tokenGenerationService.GenerateRefreshToken();

        //        await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
        //        {
        //            UserId = user.Id,
        //            Token = refreshToken,
        //            ExpiryDate = DateTime.UtcNow.AddDays(7)
        //        });

        //        return new TokenResponse
        //        {
        //            Token = accessToken,
        //            RefreshToken = refreshToken,
        //            Expiration = DateTime.UtcNow.AddMinutes(60)
        //        };
        //    }

        //    public async Task<TokenResponse> SocialLoginAsync(
        //string email,
        //string name,
        //string provider,
        //string socialId)
        //    {
        //        // Call repository (ADO.NET SP)
        //        var user = await _userRepository.SocialLogin(email, provider, socialId);

        //        if (user == null)
        //            throw new UnauthorizedAccessException("Social login failed");

        //        // Convert UserModel to User (for token generation)
        //        var tokenUser = new User
        //        {
        //            Id = user.Id,
        //            Email = user.Email
        //        };

        //        // Generate tokens
        //        var accessToken = _tokenGenerationService.GenerateAccessToken(tokenUser);
        //        var refreshToken = _tokenGenerationService.GenerateRefreshToken();

        //        await _refreshTokenRepository.SaveRefreshTokenAsync(new RefreshToken
        //        {
        //            UserId = user.Id,
        //            Token = refreshToken,
        //            ExpiryDate = DateTime.UtcNow.AddDays(7)
        //        });

        //        return new TokenResponse
        //        {
        //            Token = accessToken,
        //            RefreshToken = refreshToken,
        //            Expiration = DateTime.UtcNow.AddMinutes(15)
        //        };
        //    }
        //}
        private readonly IAuthRepository _authRepository;
        private readonly  ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IAuditService _auditService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IAuthRepository authRepository,
             ITokenService tokenService,
            IPasswordService passwordService,
            IAuditService auditService,
            ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress)
        {
            // Get user
            var user = await _authRepository.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail);

            if (user == null)
            {
                return ApiResponse<AuthResponse>.Fail("Invalid username or password");
            }

            // Check if account is locked
            if (user.LockoutEndDate.HasValue && user.LockoutEndDate > DateTime.UtcNow)
            {
                var remainingTime = (user.LockoutEndDate.Value - DateTime.UtcNow).Minutes;
                return ApiResponse<AuthResponse>.Fail($"Account is locked. Try again in {remainingTime} minutes");
            }

            // Check if account is active
            if (!user.IsActive)
            {
                return ApiResponse<AuthResponse>.Fail("Account is deactivated. Contact administrator");
            }

            // Verify password
            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                await _authRepository.UpdateLoginStatusAsync(user.Id, false, ipAddress);
                return ApiResponse<AuthResponse>.Fail("Invalid username or password");
            }

            // Get roles and permissions
            var roles = await _authRepository.GetRolesAsync(user.Id);
            var permissions = await _authRepository.GetUserPermissionsAsync(user.Id);

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user, roles, permissions);
            var refreshToken = _tokenService.GenerateRefreshToken();


            // Save refresh token
            await _authRepository.SaveRefreshTokenAsync(
                user.Id,
                refreshToken,
                _tokenService.GetRefreshTokenExpiry(),
                ipAddress);

            // Update login status
            await _authRepository.UpdateLoginStatusAsync(user.Id, true, ipAddress);

            // Log audit
            await _authRepository.LogAuditAsync(user.Id, "Login", "Users", user.Id, null, null, ipAddress, null);

            var response = new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenExpiry = _tokenService.GetAccessTokenExpiry(),
                Roles = roles.Select(r => r.RoleName).ToList(),
                Permissions = permissions.Select(p => p.PermissionName).ToList()
            };

            return ApiResponse<AuthResponse>.Success(response, "Login successful");
        }


        public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest  request, int? createdBy = null)
        {
            // Hash password
            var (hash, salt) = _passwordService.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber 
            };

            // Register user
            var (userId, message) = await _authRepository.RegisterUserAsync(user, request.RoleId, createdBy);

            if (userId == 0)
            {
                return ApiResponse<AuthResponse>.Fail(message);
            }

            user.Id = userId;

            // Get roles and permissions
            var roles = await _authRepository.GetRolesAsync(userId);
            var permissions = await _authRepository.GetUserPermissionsAsync(userId);
             
            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user, roles, permissions);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Save refresh token
            await _authRepository.SaveRefreshTokenAsync(userId, refreshToken, _tokenService.GetRefreshTokenExpiry(), null);

            var response = new AuthResponse
            {
                UserId = userId,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenExpiry = _tokenService.GetAccessTokenExpiry(),
                Roles = roles.Select(r => r.RoleName).ToList(),
                Permissions = permissions.Select(p => p.PermissionName).ToList()
            };

            return ApiResponse<AuthResponse>.Success(response, "Registration successful");
        }

        public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress)
        {
            // Validate existing tokens
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return ApiResponse<AuthResponse>.Fail("Invalid access token");
            }

            // Validate refresh token
            var (refreshToken, user) = await _authRepository.ValidateRefreshTokenAsync(request.RefreshToken);

            if (refreshToken == null || user == null)
            {
                return ApiResponse<AuthResponse>.Fail("Invalid or expired refresh token");
            }

            // Get roles and permissions
            var roles = await _authRepository.GetRolesAsync(user.Id);
            var permissions = await _authRepository.GetUserPermissionsAsync(user.Id);

            // Generate new tokens
            var newAccessToken = _tokenService.GenerateAccessToken(user, roles, permissions);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Revoke old refresh token
            await _authRepository.RevokeRefreshTokenAsync(
                request.RefreshToken,
                ipAddress,
                "Replaced by new token",
                newRefreshToken);

            // Save new refresh token
            await _authRepository.SaveRefreshTokenAsync(
                user.Id,
                newRefreshToken,
                _tokenService.GetRefreshTokenExpiry(),
                ipAddress);

            var response = new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenExpiry = _tokenService.GetAccessTokenExpiry(),
                Roles = roles.Select(r => r.RoleName).ToList(),
                Permissions = permissions.Select(p => p.PermissionName).ToList()
            };

            return ApiResponse<AuthResponse>.Success(response, "Token refreshed successfully");
        }

        public async Task<ApiResponse<bool>> RevokeTokenAsync(string token, string? ipAddress)
        {
            var (refreshToken, _) = await _authRepository.ValidateRefreshTokenAsync(token);

            if (refreshToken == null)
            {
                return ApiResponse<bool>.Fail("Token not found");
            }

            await _authRepository.RevokeRefreshTokenAsync(token, ipAddress, "Revoked by user", null);

            return ApiResponse<bool>.Success(true, "Token revoked successfully");
        }

        public async Task<bool> CheckPermissionAsync(int userId, string permissionName)
        {
            return await _authRepository.CheckUserPermissionAsync(userId, permissionName);
        }


        #region Password Management Methods

        public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            try
            {
                // ✅ Get user with PasswordHash AND PasswordSalt
                var user = await _authRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return ApiResponse<bool>.Fail("User not found");
                }

                // ✅ Verify current password with both hash and salt
                if (!VerifyPassword(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    _logger.LogWarning("Password change failed: incorrect current password for user {UserId}", userId);
                    return ApiResponse<bool>.Fail("Current password is incorrect");
                }

                // ✅ Check if new password is same as current
                if (VerifyPassword(request.NewPassword, user.PasswordHash, user.PasswordSalt))
                {
                    return ApiResponse<bool>.Fail("New password cannot be the same as current password");
                }

                // ✅ Hash new password (returns both hash and salt)
                var (newHash, newSalt) = _passwordService.HashPassword(request.NewPassword);

                // ✅ Update password with both hash and salt
                var success = await _authRepository.ChangePasswordAsync(userId, newHash, newSalt, userId);

                if (success)
                {
                    // Revoke all refresh tokens for security
                    await _authRepository.RevokeAllUserTokensAsync(userId);

                    // Log audit
                    await _auditService.LogAsync(userId, "Password Changed", "Users", userId);

                    _logger.LogInformation("Password changed successfully for user {UserId}", userId);
                    return ApiResponse<bool>.Success(true, "Password changed successfully. Please login again.");
                }

                return ApiResponse<bool>.Fail("Failed to change password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return ApiResponse<bool>.Fail("An error occurred while changing password");
            }
        }

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _authRepository.GetUserByEmailAsync(request.Email);

                // Always return success to prevent email enumeration
                if (user == null)
                {
                    //_logger.LogWarning("Forgot password requested for non-existent email: {Email}", request.Email);
                    return ApiResponse<bool>.Success(true, "If the email exists, a password reset link will be sent.");
                }

                // Generate reset token
                var token = GeneratePasswordResetToken();
                var expiryDate = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

                // Save token
                await _authRepository.SavePasswordResetTokenAsync(user.Id, token, expiryDate);

                // TODO: Send email with reset link
                // var resetLink = $"{_configuration["AppSettings:FrontendUrl"]}/Account/ResetPassword?token={token}&email={user.Email}";
                // await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

               // _logger.LogInformation("Password reset token generated for user {UserId}. Token: {Token}", user.Id, token);

                // For development - return token in message (remove in production!)
               #if DEBUG
                return ApiResponse<bool>.Success(true, $"Password reset token: {token}");
             #else
                return ApiResponse<bool>.Success(true, "Password reset link sent to your email.");
               #endif
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error processing forgot password for email {Email}", request.Email);
                return ApiResponse<bool>.Fail("An error occurred while processing your request");
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordWithTokenAsync(ResetPasswordWithTokenRequest request)
        {
            try
            {
                // Validate token
                var tokenData = await _authRepository.ValidatePasswordResetTokenAsync(request.Token);

                if (tokenData == null)
                {
                    return ApiResponse<bool>.Fail("Invalid or expired reset token");
                }

                // Verify email matches
                if (!string.Equals(tokenData.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    return ApiResponse<bool>.Fail("Invalid reset request");
                }

                // ✅ Hash new password (returns both hash and salt)
                var (newHash, newSalt) = _passwordService.HashPassword(request.NewPassword);

                // ✅ Update password with both hash and salt
                var success = await _authRepository.ChangePasswordAsync(tokenData.UserId, newHash, newSalt, null);

                if (success)
                {
                    // Mark token as used
                    await _authRepository.MarkPasswordResetTokenUsedAsync(request.Token);

                    // Revoke all refresh tokens
                    await _authRepository.RevokeAllUserTokensAsync(tokenData.UserId);

                    // Log audit
                    await _auditService.LogAsync(tokenData.UserId, "Password Reset via Token", "Users", tokenData.UserId);

                    return ApiResponse<bool>.Success(true, "Password reset successfully. Please login with your new password.");
                }

                return ApiResponse<bool>.Fail("Failed to reset password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password with token");
                return ApiResponse<bool>.Fail("An error occurred while resetting password");
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordByAdminAsync(ResetPasswordRequest request, int adminUserId)
        {
            try
            {
                var user = await _authRepository.GetUserByIdAsync(request.UserId);

                if (user == null)
                {
                    return ApiResponse<bool>.Fail("User not found");
                }

                // ✅ Hash new password (returns both hash and salt)
                var (newHash, newSalt) = _passwordService.HashPassword(request.NewPassword);

                // ✅ Update password with correct parameter order
                var success = await _authRepository.ChangePasswordAsync(request.UserId, newHash, newSalt, adminUserId);

                if (success)
                {
                    // Revoke all refresh tokens
                    await _authRepository.RevokeAllUserTokensAsync(request.UserId);

                    // Log audit
                    await _auditService.LogAsync(
                        adminUserId,
                        "Password Reset by Admin",
                        "Users",
                        request.UserId,
                        null,
                        $"Admin {adminUserId} reset password for user {request.UserId}",
                        null,
                        null);

                    _logger.LogInformation("Password reset by admin {AdminId} for user {UserId}", adminUserId, request.UserId);
                    return ApiResponse<bool>.Success(true, "Password reset successfully");
                }

                return ApiResponse<bool>.Fail("Failed to reset password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user {UserId}", request.UserId);
                return ApiResponse<bool>.Fail("An error occurred while resetting password");
            }
        }

        public async Task<ApiResponse<bool>> ValidatePasswordResetTokenAsync(string token)
        {
            try
            {
                var tokenData = await _authRepository.ValidatePasswordResetTokenAsync(token);

                if (tokenData == null)
                {
                    return ApiResponse<bool>.Fail("Invalid or expired reset token");
                }

                return ApiResponse<bool>.Success(true, "Token is valid");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error validating password reset token");
                return ApiResponse<bool>.Fail("An error occurred while validating token");
            }
        }

        #endregion
        #region Private Helper Methods

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        private bool VerifyPassword(string password, string passwordHash, string passwordSalt)
        {
            try
            {
                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(passwordSalt))
                    return false;

                byte[] saltBytes = Convert.FromBase64String(passwordSalt);

                string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                return hash == passwordHash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password");
                return false;
            }
        }

        private string GeneratePasswordResetToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }

        private async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            // TODO: Implement get user permissions from repository
            // This should get permissions based on user's roles
            return new List<Permission>();
        }

        #endregion


    }
}