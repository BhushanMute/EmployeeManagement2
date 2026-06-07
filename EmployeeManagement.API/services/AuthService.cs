using DocumentFormat.OpenXml.Spreadsheet;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.services;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using ITokenService = EmployeeManagement.API.Repositories.ITokenService;

namespace EmployeeManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IAuditService _auditService;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;           // ✅ Fixed: Added underscore
        private readonly IConfiguration _configuration;         // ✅ Fixed: Added missing field

        public AuthService(
            IAuthRepository authRepository,
            ITokenService tokenService,
            IPasswordService passwordService,
            IAuditService auditService,
            ILogger<AuthService> logger,
            IEmailService emailService,
            IConfiguration configuration)                       // ✅ Fixed: Added to constructor
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _auditService = auditService;
            _logger = logger;
            _emailService = emailService;                       // ✅ Fixed: Proper assignment
            _configuration = configuration;                     // ✅ Fixed: Proper assignment
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

            var profilePictureUrl = "";
            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                profilePictureUrl = user.ProfilePicture.StartsWith("/")
                    ? user.ProfilePicture
                    : $"/uploads/profiles/{user.ProfilePicture}";
            }

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
                Permissions = permissions.Select(p => p.PermissionName).ToList(),
                ProfilePicture = profilePictureUrl
            };

            return ApiResponse<AuthResponse>.Success(response, "Login successful");
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, int? createdBy = null)
        {
            var isAdminCreated = createdBy.HasValue;
            if (isAdminCreated && (!request.RoleId.HasValue || request.RoleId.Value <= 0))
            {
                return ApiResponse<AuthResponse>.Fail("Role is required when an admin creates a user");
            }

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
            var (userId, message) = await _authRepository.RegisterUserAsync(
                user,
                isAdminCreated ? request.RoleId : null,
                createdBy);

            if (userId == 0)
            {
                return ApiResponse<AuthResponse>.Fail(message);
            }

            user.Id = userId;

            if (!isAdminCreated)
            {
                var pendingResponse = new AuthResponse
                {
                    UserId = userId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}"
                };

                return ApiResponse<AuthResponse>.Success(
                    pendingResponse,
                    "Registration submitted successfully. Admin approval and role assignment are required before login.");
            }

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
                // Get user with PasswordHash AND PasswordSalt
                var user = await _authRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return ApiResponse<bool>.Fail("User not found");
                }

                // Verify current password with both hash and salt
                if (!VerifyPassword(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    _logger.LogWarning("Password change failed: incorrect current password for user {UserId}", userId);
                    return ApiResponse<bool>.Fail("Current password is incorrect");
                }

                // Check if new password is same as current
                if (VerifyPassword(request.NewPassword, user.PasswordHash, user.PasswordSalt))
                {
                    return ApiResponse<bool>.Fail("New password cannot be the same as current password");
                }

                // Hash new password (returns both hash and salt)
                var (newHash, newSalt) = _passwordService.HashPassword(request.NewPassword);

                // Update password with both hash and salt
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

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request,
            string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                _logger.LogInformation("Forgot password request for email: {Email}", request.Email);

                // Get user by email
                var user = await _authRepository.GetUserByEmailAsync(request.Email);

                // Security: Always return success to prevent email enumeration
                if (user == null || !user.IsActive || user.IsDeleted)
                {
                    _logger.LogWarning("Forgot password: User not found or inactive for email: {Email}", request.Email);

                    // Return success to prevent email enumeration attack
                    return ApiResponse<bool>.Success(true,
                        "If the email exists in our system, you will receive a password reset link shortly.");
                }

                // ✅ Fixed: Use the correct method name
                var token = GenerateSecureToken();

                // Token expires in 30 minutes
                var expiryDate = DateTime.UtcNow.AddMinutes(30);

                // Save token to database
                await _authRepository.SavePasswordResetTokenAsync(
                    user.Id,
                    token,
                    expiryDate,
                    ipAddress,
                    userAgent);

                // Build reset link
                var frontendUrl = _configuration["AppSettings:FrontendUrl"]?.TrimEnd('/')
                    ?? "https://localhost:44354";
                var resetLink = $"{frontendUrl}/Account/ResetPassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

                // Send email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(
                    user.Email,
                    $"{user.FirstName} {user.LastName}",
                    resetLink);

                if (!emailSent)
                {
                    _logger.LogWarning("Failed to send password reset email to {Email}", user.Email);
                }
                else
                {
                    _logger.LogInformation("Password reset email sent successfully to {Email}", user.Email);
                }

                // Log audit
                await _auditService.LogAsync(
                    user.Id,
                    "Password Reset Requested",
                    "Users",
                    user.Id,
                    null,
                    $"Password reset requested from IP: {ipAddress}",
                    ipAddress,
                    userAgent);

                return ApiResponse<bool>.Success(true,
                    "If the email exists in our system, you will receive a password reset link shortly.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password for email: {Email}", request.Email);

                // Return success to prevent information disclosure
                return ApiResponse<bool>.Success(true,
                    "If the email exists in our system, you will receive a password reset link shortly.");
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordWithTokenAsync(ResetPasswordWithTokenRequest request)
        {
            try
            {
                _logger.LogInformation("Password reset attempt with token for email: {Email}", request.Email);

                // Validate token
                var tokenData = await _authRepository.ValidatePasswordResetTokenAsync(request.Token);

                if (tokenData == null)
                {
                    _logger.LogWarning("Invalid or expired reset token");
                    return ApiResponse<bool>.Fail("Invalid or expired password reset link. Please request a new one.");
                }

                // Verify email matches the token
                if (!string.Equals(tokenData.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Email mismatch for password reset token");
                    return ApiResponse<bool>.Fail("Invalid password reset request.");
                }

                // Check token expiry
                if (tokenData.ExpiryDate < DateTime.UtcNow)
                {
                    _logger.LogWarning("Password reset token expired for user {UserId}", tokenData.UserId);
                    return ApiResponse<bool>.Fail("Password reset link has expired. Please request a new one.");
                }

                // Check if token is already used
                if (tokenData.IsUsed)
                {
                    _logger.LogWarning("Password reset token already used for user {UserId}", tokenData.UserId);
                    return ApiResponse<bool>.Fail("This password reset link has already been used.");
                }

                // Validate password confirmation
                if (request.NewPassword != request.ConfirmPassword)
                {
                    return ApiResponse<bool>.Fail("Passwords do not match.");
                }

                // Hash new password (returns both hash and salt)
                var (newHash, newSalt) = _passwordService.HashPassword(request.NewPassword);

                // Update password with both hash and salt
                var success = await _authRepository.ChangePasswordAsync(tokenData.UserId, newHash, newSalt, null);

                if (!success)
                {
                    _logger.LogError("Failed to update password for user {UserId}", tokenData.UserId);
                    return ApiResponse<bool>.Fail("Failed to reset password. Please try again.");
                }

                // Mark token as used
                await _authRepository.MarkPasswordResetTokenUsedAsync(request.Token);

                // Revoke all existing refresh tokens for security
                await _authRepository.RevokeAllUserTokensAsync(tokenData.UserId);

                // Send notification email
                if (!string.IsNullOrEmpty(tokenData.Email))
                {
                    await _emailService.SendPasswordChangedNotificationAsync(
                        tokenData.Email,
                        $"{tokenData.FirstName} {tokenData.LastName}");
                }

                // Log audit
                await _auditService.LogAsync(
                    tokenData.UserId,
                    "Password Reset Completed",
                    "Users",
                    tokenData.UserId,
                    null,
                    "Password reset via email link");

                _logger.LogInformation("Password reset successful for user {UserId}", tokenData.UserId);

                return ApiResponse<bool>.Success(true,
                    "Password has been reset successfully. You can now log in with your new password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password with token");
                return ApiResponse<bool>.Fail("An error occurred while resetting your password. Please try again.");
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

                // Hash new password (returns both hash and salt)
                var (newHash, newSalt) = _passwordService.HashPassword(request.NewPassword);

                // Update password with correct parameter order
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
                if (string.IsNullOrWhiteSpace(token))
                {
                    return ApiResponse<bool>.Fail("Token is required.");
                }

                var tokenData = await _authRepository.ValidatePasswordResetTokenAsync(token);

                if (tokenData == null)
                {
                    return ApiResponse<bool>.Fail("Invalid or expired password reset link.");
                }

                if (tokenData.IsUsed)
                {
                    return ApiResponse<bool>.Fail("This password reset link has already been used.");
                }

                if (tokenData.ExpiryDate < DateTime.UtcNow)
                {
                    return ApiResponse<bool>.Fail("Password reset link has expired.");
                }

                return ApiResponse<bool>.Success(true, "Token is valid.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating password reset token");
                return ApiResponse<bool>.Fail("An error occurred while validating the token.");
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

        /// <summary>
        /// ✅ Fixed: Added this method (was missing but being called)
        /// Generates a cryptographically secure token for password reset
        /// </summary>
        private string GenerateSecureToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            // Convert to URL-safe base64
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }

        /// <summary>
        /// Alternative method name (kept for backward compatibility)
        /// </summary>
        private string GeneratePasswordResetToken()
        {
            return GenerateSecureToken();
        }

        private async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            return await _authRepository.GetUserPermissionsAsync(userId);
        }

        #endregion
    }
}
