using EmployeeManagement.UI.Models;
using EmployeeManagement.UI.Services;
using EmployeeManagement.UI.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ChangePasswordViewModel = EmployeeManagement.UI.ViewModels.ChangePasswordViewModel;

namespace EmployeeManagement.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<AccountController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController( IApiService apiService, ILogger<AccountController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _apiService = apiService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Employee");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _logger.LogInformation("Login attempt for user: {Username}", model.UsernameOrEmail);

                var result = await _apiService.PostAsync<AuthResponse>("api/auth/login", model);

                if (result == null)
                {
                    _logger.LogWarning("Login failed: API returned null response");
                    ModelState.AddModelError("", "Unable to connect to server. Please try again.");
                    return View(model);
                }

                if (!result.Status || result.Data == null)
                {
                    _logger.LogWarning("Login failed for user {Username}: {Message}", model.UsernameOrEmail, result.Message);
                    ModelState.AddModelError("", result.Message ?? "Invalid username or password");
                    return View(model);
                }

                var authData = result.Data;

                _logger.LogInformation("Login successful for user: {Username}", authData.Username);
                _logger.LogInformation("ProfilePicture from API: {ProfilePicture}", authData.ProfilePicture ?? "NULL");

                // ✅ Clear old session first
                HttpContext.Session.Clear();

                // ✅ Store in session
                HttpContext.Session.SetString("AccessToken", authData.AccessToken ?? "");
                HttpContext.Session.SetString("RefreshToken", authData.RefreshToken ?? "");
                HttpContext.Session.SetString("UserId", authData.UserId.ToString());
                HttpContext.Session.SetString("Username", authData.Username ?? "");
                HttpContext.Session.SetString("Email", authData.Email ?? "");
                HttpContext.Session.SetString("FullName", authData.FullName ?? authData.Username ?? "");
                HttpContext.Session.SetString("Roles", string.Join(",", authData.Roles ?? new List<string>()));
                HttpContext.Session.SetString("Permissions", string.Join(",", authData.Permissions ?? new List<string>()));

                // ✅ Store ProfilePicture with full URL
                var profilePicture = "";
                if (!string.IsNullOrEmpty(authData.ProfilePicture))
                {
                    if (authData.ProfilePicture.StartsWith("http"))
                    {
                        profilePicture = authData.ProfilePicture;
                    }
                    else
                    {
                        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') ?? "https://localhost:7192";
                        profilePicture = $"{apiBaseUrl}{authData.ProfilePicture}";
                    }
                }

                HttpContext.Session.SetString("ProfilePicture", profilePicture);
                _logger.LogInformation("ProfilePicture stored in session: {Url}", profilePicture);

                // ✅ Create claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, authData.UserId.ToString()),
            new Claim(ClaimTypes.Name, authData.Username ?? ""),
            new Claim(ClaimTypes.Email, authData.Email ?? ""),
            new Claim("FullName", authData.FullName ?? authData.Username ?? ""),
            new Claim("ProfilePicture", profilePicture)
        };

                // Add role claims
                if (authData.Roles != null && authData.Roles.Any())
                {
                    foreach (var role in authData.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                // Add permission claims
                if (authData.Permissions != null && authData.Permissions.Any())
                {
                    foreach (var permission in authData.Permissions)
                    {
                        claims.Add(new Claim("Permission", permission));
                    }
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

                _logger.LogInformation("User {Username} signed in successfully", authData.Username);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectBasedOnRole(authData.Roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user {Username}", model.UsernameOrEmail);
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var apiModel = new
            {
                firstName = model.FirstName,
                lastName = model.LastName,
                username = model.Username,
                email = model.Email,
                phoneNumber = model.PhoneNumber,
                password = model.Password,
                confirmPassword = model.ConfirmPassword
            };

            var result = await _apiService.PostAsync<AuthResponse>("api/auth/register", apiModel);

            if (result == null || !result.Status)
            {
                if (result?.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                else
                {
                    ModelState.AddModelError("", result?.Message ?? "Registration failed");
                }
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var refreshToken = HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Json(new { success = false });
            }

            var result = await _apiService.PostAsync<AuthResponse>("api/auth/refresh-token",
                new { AccessToken = accessToken, RefreshToken = refreshToken });

            if (result == null || !result.Status || result.Data == null)
            {
                HttpContext.Session.Clear();
                return Json(new { success = false });
            }

            HttpContext.Session.SetString("AccessToken", result.Data.AccessToken);
            HttpContext.Session.SetString("RefreshToken", result.Data.RefreshToken ?? "");

            return Json(new { success = true, token = result.Data.AccessToken });
        }

        private IActionResult RedirectBasedOnRole(List<string>? roles)
        {
            if (roles == null || !roles.Any())
            {
                _logger.LogWarning("No roles found, redirecting to Home");
                return RedirectToAction("Index", "Home");
            }

            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "Employee");
            else if (roles.Contains("HR") || roles.Contains("Employee"))
                return RedirectToAction("Index", "Employee");
            else
                return RedirectToAction("Index", "Home");
        }

        #region Password Management

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var request = new
                {
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword
                };

                var result = await _apiService.PostAsync<object>("api/auth/change-password", request, token);

                if (result == null || !result.Status)
                {
                    TempData["ErrorMessage"] = result?.Message ?? "Current password is invalid. Please enter valid password";
                    return View(model);
                }

                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                TempData["ErrorMessage"] = "An error occurred while changing password";
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            // Redirect if already logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _logger.LogInformation("Forgot password request for: {Email}", model.Email);

                var request = new { Email = model.Email };

                var result = await _apiService.PostAsync<bool>("api/auth/forgot-password", request);

                // Always show success to prevent email enumeration
                TempData["SuccessMessage"] = "If the email exists in our system, you will receive a password reset link shortly.";

                return RedirectToAction("ForgotPasswordConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password for: {Email}", model.Email);

                // Still show success to prevent information disclosure
                TempData["SuccessMessage"] = "If the email exists in our system, you will receive a password reset link shortly.";

                return RedirectToAction("ForgotPasswordConfirmation");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPassword(string? token, string? email)
        //{
        //    // Validate parameters
        //    if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
        //    {
        //        TempData["ErrorMessage"] = "Invalid password reset link.";
        //        return RedirectToAction("Login");
        //    }

        //    try
        //    {
        //        // Validate token with API
        //        var result = await _apiService.GetAsync<bool>($"api/auth/validate-reset-token?token={Uri.EscapeDataString(token)}");

        //        if (result == null || !result.Status)
        //        {
        //            TempData["ErrorMessage"] = result?.Message ?? "Invalid or expired password reset link. Please request a new one.";
        //            return RedirectToAction("ForgotPassword");
        //        }

        //        var model = new ResetPasswordViewModel
        //        {
        //            Token = token,
        //            Email = email
        //        };

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error validating reset token");
        //        TempData["ErrorMessage"] = "An error occurred. Please try again or request a new reset link.";
        //        return RedirectToAction("ForgotPassword");
        //    }
        //}

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string? token, string? email)
        {
            _logger.LogInformation("=== ResetPassword GET ===");
            _logger.LogInformation("Token: {Token}", token);
            _logger.LogInformation("Email: {Email}", email);

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Token or email is empty");
                TempData["ErrorMessage"] = "Invalid password reset link.";
                return RedirectToAction("Login");
            }

            try
            {
                var apiUrl = $"api/auth/validate-reset-token?token={Uri.EscapeDataString(token)}";
                _logger.LogInformation("Calling API: {Url}", apiUrl);

                var result = await _apiService.GetAsync<bool>(apiUrl);

                _logger.LogInformation("API Result - IsNull: {IsNull}", result == null);
                _logger.LogInformation("API Result - Status: {Status}", result?.Status);
                _logger.LogInformation("API Result - Message: {Message}", result?.Message);

                if (result == null || !result.Status)
                {
                    _logger.LogWarning("Token validation failed: {Message}", result?.Message);
                    TempData["ErrorMessage"] = result?.Message ?? "Invalid or expired password reset link.";
                    return RedirectToAction("ForgotPassword");
                }

                var model = new ResetPasswordViewModel
                {
                    Token = token,
                    Email = email
                };

                _logger.LogInformation("Token valid, showing ResetPassword view");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ResetPassword GET: {Message}", ex.Message);
                TempData["ErrorMessage"] = "An error occurred.";
                return RedirectToAction("ForgotPassword");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _logger.LogInformation("Password reset attempt for: {Email}", model.Email);

                var request = new
                {
                    Token = model.Token,
                    Email = model.Email,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword
                };

                var result = await _apiService.PostAsync<bool>("api/auth/reset-password", request);

                if (result == null || !result.Status)
                {
                    ModelState.AddModelError("", result?.Message ?? "Failed to reset password. Please try again.");
                    return View(model);
                }

                _logger.LogInformation("Password reset successful for: {Email}", model.Email);

                TempData["SuccessMessage"] = "Your password has been reset successfully. You can now log in with your new password.";

                return RedirectToAction("ResetPasswordConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for: {Email}", model.Email);
                ModelState.AddModelError("", "An error occurred while resetting your password. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminResetPassword(int userId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                var user = await _apiService.GetAsync<UserModel>($"api/users/{userId}", token);

                if (user == null || !user.Status || user.Data == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("Index", "User");
                }

                var model = new AdminResetPasswordViewModel
                {
                    UserId = userId,
                    Username = user.Data.Username,
                    Email = user.Data.Email
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin reset password page");
                TempData["Error"] = "An error occurred";
                return RedirectToAction("Index", "User");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminResetPassword(AdminResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var request = new
                {
                    UserId = model.UserId,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword,
                    SendEmailNotification = model.SendEmailNotification
                };

                var result = await _apiService.PostAsync<bool>("api/auth/reset-password-admin", request, token);

                if (result == null || !result.Status)
                {
                    ModelState.AddModelError("", result?.Message ?? "Failed to reset password");
                    return View(model);
                }

                TempData["SuccessMessage"] = $"Password has been reset for user '{model.Username}'";
                return RedirectToAction("Index", "User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in admin reset password for user {UserId}", model.UserId);
                ModelState.AddModelError("", "An error occurred while resetting password");
                return View(model);
            }
        }

        #endregion



        #region Profile Management
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                _logger.LogInformation("=== Profile GET Started ===");
                _logger.LogInformation("Token exists: {HasToken}", !string.IsNullOrEmpty(token));

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No token found, redirecting to Login");
                    return RedirectToAction("Login");
                }

                // ✅ Call API
                _logger.LogInformation("Calling API: api/Profile");
                var result = await _apiService.GetAsync<ProfileViewModel>("api/Profile", token);

                // ✅ Debug: Log what we received
                _logger.LogInformation("API Result - IsNull: {IsNull}", result == null);

                if (result != null)
                {
                    _logger.LogInformation("API Result - Status: {Status}", result.Status);
                    _logger.LogInformation("API Result - Message: {Message}", result.Message ?? "null");
                    _logger.LogInformation("API Result - Data IsNull: {DataIsNull}", result.Data == null);
                    _logger.LogInformation("API Result - StatusCode: {StatusCode}", result.StatusCode);

                    if (result.Data != null)
                    {
                        _logger.LogInformation("Profile Data - Username: {Username}", result.Data.Username);
                        _logger.LogInformation("Profile Data - Email: {Email}", result.Data.Email);
                        _logger.LogInformation("Profile Data - ProfilePicture: {ProfilePic}", result.Data.ProfilePicture ?? "null");
                    }
                }

                // ✅ Check result
                if (result != null && result.Status && result.Data != null)
                {
                    var profile = result.Data;

                    // Build full URL for profile picture
                    if (!string.IsNullOrEmpty(profile.ProfilePicture) && !profile.ProfilePicture.StartsWith("http"))
                    {
                        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') ?? "https://localhost:7192";
                        profile.ProfilePicture = $"{apiBaseUrl}{profile.ProfilePicture}";
                        _logger.LogInformation("Built ProfilePicture URL: {Url}", profile.ProfilePicture);
                    }

                    // Update session
                    HttpContext.Session.SetString("ProfilePicture", profile.ProfilePicture ?? "");

                    _logger.LogInformation("Returning Profile view with data");
                    return View(profile);
                }

                // ✅ If we reach here, something failed
                _logger.LogWarning("Profile load failed - Creating fallback from session");

                var profileFromSession = new ProfileViewModel
                {
                    Username = HttpContext.Session.GetString("Username") ?? "Unknown",
                    Email = HttpContext.Session.GetString("Email") ?? "Unknown",
                    FullName = HttpContext.Session.GetString("FullName") ?? "Unknown",
                    FirstName = HttpContext.Session.GetString("FullName")?.Split(' ').FirstOrDefault() ?? "",
                    LastName = HttpContext.Session.GetString("FullName")?.Split(' ').LastOrDefault() ?? "",
                    ProfilePicture = HttpContext.Session.GetString("ProfilePicture") ?? ""
                };

                _logger.LogInformation("Returning Profile view with session data");
                return View(profileFromSession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile");
                TempData["Error"] = "Failed to load profile";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Edit Profile - GET (Load profile data for editing)
        /// </summary>
        // Update EditProfile method in MVC ProfileController.cs

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile(int id)
        {
            try
            {
                _logger.LogInformation("=== EditProfile Called with ID: {Id} ===", id);

                // ✅ Check if ID is valid
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid ID: {Id}", id);
                    TempData["Error"] = "Invalid user ID";
                    return RedirectToAction("Profile");
                }

                var token = HttpContext.Session.GetString("AccessToken");
                _logger.LogInformation("Token exists: {HasToken}", !string.IsNullOrEmpty(token));

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No access token found");
                    return RedirectToAction("Login", "Account");
                }

                // ✅ Log the API URL being called
                var apiUrl = $"api/Profile/{id}";
                _logger.LogInformation("Calling API: {ApiUrl}", apiUrl);

                var result = await _apiService.GetAsync<ProfileViewModel>(apiUrl, token);

                // ✅ Log the result
                _logger.LogInformation("API Result - IsNull: {IsNull}, Status: {Status}, HasData: {HasData}",
                    result == null,
                    result?.Status,
                    result?.Data != null);

                if (result != null && result.Status && result.Data != null)
                {
                    var profile = result.Data;
                    _logger.LogInformation("Profile loaded: {FullName}", profile.FullName);

                    // Build full URL for profile picture
                    if (!string.IsNullOrEmpty(profile.ProfilePicture) && !profile.ProfilePicture.StartsWith("http"))
                    {
                        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/') ?? "https://localhost:7192";
                        profile.ProfilePicture = $"{apiBaseUrl}{profile.ProfilePicture}";
                    }

                    HttpContext.Session.SetString("ProfilePicture", profile.ProfilePicture ?? "");

                    _logger.LogInformation("Returning EditProfile View");
                    return View(profile);  // ✅ Should return view here
                }

                // ✅ Log why it failed
                _logger.LogWarning("Failed to load profile. Message: {Message}", result?.Message);
                TempData["Error"] = result?.Message ?? "Failed to load profile";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in EditProfile. ID: {Id}, Error: {Message}", id, ex.Message);
                TempData["Error"] = "Failed to load profile";
                return RedirectToAction("Profile");
            }
        }
        /// <summary>
        /// Edit Profile - POST (Save updated profile data)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            _logger.LogInformation("=== EditProfile POST Called ===");
            _logger.LogInformation("Model ID: {Id}, Name: {FirstName} {LastName}", model.Id, model.FirstName, model.LastName);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState Invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation Error: {Error}", error.ErrorMessage);
                }
                return View(model);
            }

            var token = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No token found");
                return RedirectToAction("Login");
            }

            try
            {
                var updateRequest = new UpdateProfileRequest
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth
                };

                _logger.LogInformation("Calling PUT api/Profile/UpdateProfile");

                var result = await _apiService.PutAsync<ProfileViewModel>(
                    "api/Profile/UpdateProfile",
                    updateRequest,
                    token
                );

                _logger.LogInformation("API Result - Status: {Status}, Message: {Message}", result?.Status, result?.Message);

                if (result != null && result.Status)
                {
                    // ✅ Update session with new values
                    HttpContext.Session.SetString("FullName", $"{model.FirstName} {model.LastName}");
                    HttpContext.Session.SetString("Email", model.Email ?? "");

                    TempData["Success"] = "Profile updated successfully!";
                    return RedirectToAction("Profile");
                }

                TempData["Error"] = result?.Message ?? "Failed to update profile";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EditProfile POST Exception: {Message}", ex.Message);
                TempData["Error"] = "An error occurred while updating profile";
                return View(model);
            }
        }
        /// <summary>
        /// Upload Profile Picture - AJAX
        /// </summary>
        /// <summary>
        /// Upload Profile Picture - AJAX
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                _logger.LogInformation("=== Upload Profile Picture Started ===");
                _logger.LogInformation("Token exists: {HasToken}", !string.IsNullOrEmpty(token));
                _logger.LogInformation("File: {FileName}, Size: {Size}", file?.FileName, file?.Length);

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("No file received");
                    return Json(new { success = false, message = "No file selected" });
                }

                // Validate file size (5MB max)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "File size must be less than 5MB" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return Json(new { success = false, message = "Only JPG, PNG, and GIF files are allowed" });
                }

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No token found");
                    return Json(new { success = false, message = "Session expired. Please login again." });
                }

                // Create multipart form content
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "file", file.FileName);

                // Create HttpClient
                var client = _httpClientFactory.CreateClient("API");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var apiUrl = "api/Profile/UploadProfilePicture";
                _logger.LogInformation("Calling API: {Url}", apiUrl);

                // Call API
                var response = await client.PostAsync(apiUrl, content);

                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("API Response Content: {Content}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<string>>(responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (result != null && result.Status && !string.IsNullOrEmpty(result.Data))
                    {
                        // ✅ Get API base URL and create full URL
                        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"]?.TrimEnd('/')
                            ?? "https://localhost:26024";

                        var fullImageUrl = result.Data.StartsWith("http")
                            ? result.Data
                            : $"{apiBaseUrl}{result.Data}";

                        // ✅ Update session
                        HttpContext.Session.SetString("ProfilePicture", fullImageUrl);

                        _logger.LogInformation("Profile picture uploaded successfully: {Url}", fullImageUrl);

                        return Json(new
                        {
                            success = true,
                            message = "Profile picture uploaded successfully",
                            imageUrl = fullImageUrl
                        });
                    }

                    return Json(new { success = false, message = result?.Message ?? "Upload failed" });
                }

                _logger.LogError("API failed with status: {StatusCode}, Content: {Content}",
                    response.StatusCode, responseContent);

                return Json(new { success = false, message = $"Upload failed: {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload failed with exception");
                return Json(new { success = false, message = "An error occurred while uploading" });
            }
        }

        #endregion
    }
}
