 
using EmployeeManagement.UI.Models;
using EmployeeManagement.UI.Services;
using EmployeeManagement.UI.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
 
 
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using AuthResponse = EmployeeManagement.UI.Models.AuthResponse;
using LoginModel = EmployeeManagement.UI.Models.LoginModel;
using LoginRequest = EmployeeManagement.UI.Models.LoginRequest;

namespace EmployeeManagement.UI.Controllers
{
    #region Privous logc
    //public class AccountController : Controller
    //{
    //    private readonly HttpClient _client;

    //    public AccountController(IHttpClientFactory factory)
    //    {
    //        _client = factory.CreateClient();
    //        _client.BaseAddress = new Uri("http://localhost:26024/");                                                                                                                                                                                                                                                   
    //    }

    //    [HttpGet("Account/Login")]
    //    public IActionResult Login()
    //    {

    //        //return Challenge(new AuthenticationProperties
    //        //{
    //        //    RedirectUri = "/Account/GoogleResponse",

    //        //}, "Google");
    //        return View(new LoginModel());
    //    }

    //    [HttpPost("Account/Login")]
    //    public async Task<IActionResult> Login(LoginModel model)
    //    {
    //        if (!ModelState.IsValid)
    //            return View(model);

    //        try
    //        {
    //            var request = new LoginRequest
    //            {
    //                Email = model.Email,   // map correctly
    //                Password = model.Password
    //            };

    //            var response = await _client.PostAsJsonAsync("api/auth/login", request);

    //            var content = await response.Content.ReadAsStringAsync();

    //            if (!response.IsSuccessStatusCode)
    //            {
    //                ViewBag.Error = content;
    //                return View(model);
    //            }

    //            var result = JsonSerializer.Deserialize<AuthResponse>(content,
    //                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    //            // Store JWT in Session
    //            HttpContext.Session.SetString("token", result.Token);

    //            // ✅ ADD THIS BLOCK (IMPORTANT)
    //            var claims = new List<Claim>
    //    {
    //        new Claim(ClaimTypes.Name, model.Email),
    //        new Claim("JWT", result.Token)
    //    };

    //            var claimsIdentity = new ClaimsIdentity(
    //                claims, CookieAuthenticationDefaults.AuthenticationScheme);

    //            var authProperties = new AuthenticationProperties
    //            {
    //                IsPersistent = true,
    //                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
    //            };

    //            await HttpContext.SignInAsync(
    //                CookieAuthenticationDefaults.AuthenticationScheme,
    //                new ClaimsPrincipal(claimsIdentity),
    //                authProperties);

    //            // Redirect to Employee Index
    //            return RedirectToAction("Index", "Employee");
    //        }
    //        catch (Exception ex)
    //        {
    //            ViewBag.Error = ex.Message;
    //            return View(model);
    //        }
    //    }

    //    [HttpGet]
    //    public async Task<IActionResult> Logout()
    //    {
    //        try
    //        {
    //            HttpContext.Session.Clear();

    //            await HttpContext.SignOutAsync("Cookies");

    //            return RedirectToAction("Login");
    //        }
    //        catch (Exception ex)
    //        {
    //            ErrorLogger.Log(ex);
    //            return RedirectToAction("Login");
    //        }
    //    }
    //    [HttpGet]
    //    public IActionResult Register()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //     public async Task<IActionResult> Register(RegisterModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return View(model);
    //        }

    //        try
    //        {
    //            var response = await _client.PostAsJsonAsync("api/Auth/Register", model);

    //            if (response.IsSuccessStatusCode)
    //            {
    //                TempData["SuccessMessage"] = "Account created successfully. Please login.";
    //                return RedirectToAction("Login");
    //            }
    //            else
    //            {

    //                var error = await response.Content.ReadAsStringAsync();
    //                ViewBag.Error = error;
    //                ErrorLogger.Log(error);
    //                return View(model);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            ErrorLogger.Log(ex);
    //            ViewBag.Error = "Something went wrong: " + ex.Message;
    //            return View(model);
    //        }
    //    }
    //    public IActionResult GoogleLogin()
    //    {
    //        return Challenge(
    //            new AuthenticationProperties
    //            {
    //                RedirectUri = Url.Action("ExternalLoginCallback")
    //            },
    //            GoogleDefaults.AuthenticationScheme);
    //    }
    //    public async Task<IActionResult> GoogleResponse()
    //    {
    //        var result = await HttpContext.AuthenticateAsync("Cookies");

    //        if (!result.Succeeded)
    //            return RedirectToAction("Login");

    //        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
    //        var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
    //        //var Token = result.Principal.FindFirst(ClaimTypes.)?.Value;

    //        if (string.IsNullOrEmpty(email))
    //            return RedirectToAction("Login");


    //        var response = await _client.PostAsJsonAsync("api/auth/social-login", new
    //        {
    //            Email = email,
    //            Name = name,
    //            Provider = "Google",
    //            SocialId = email
    //        });

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            TempData["Error"] = "Google login failed";
    //            return RedirectToAction("Login");
    //        }

    //        var resultData = await response.Content.ReadFromJsonAsync<AuthResponse>();

    //        if (string.IsNullOrEmpty(resultData?.Token))
    //        {
    //            TempData["Error"] = "Token generation failed";
    //            return RedirectToAction("Login");
    //        }

    //        // ✅ Store JWT in Session
    //        HttpContext.Session.SetString("token", resultData.Token);

    //        return RedirectToAction("Index", "Employee");
    //    }

    //    public IActionResult FacebookLogin()
    //    {
    //        return Challenge(
    //            new AuthenticationProperties
    //            {
    //                RedirectUri = Url.Action("ExternalLoginCallback")
    //            },
    //            FacebookDefaults.AuthenticationScheme);
    //    }

    //    public async Task<IActionResult> ExternalLoginCallback()
    //    {
    //        var authenticateResult = await HttpContext.AuthenticateAsync(
    //            CookieAuthenticationDefaults.AuthenticationScheme);

    //        if (!authenticateResult.Succeeded)
    //            return RedirectToAction("Login");

    //        var principal = authenticateResult.Principal;

    //        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
    //        var name = principal.FindFirst(ClaimTypes.Name)?.Value;
    //        var socialId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    //        // detect provider automatically
    //        var provider = authenticateResult.Properties.Items[".AuthScheme"];

    //        if (string.IsNullOrEmpty(email))
    //            return RedirectToAction("Login");

    //        // Call your API (COMMON FOR ALL PROVIDERS)
    //        var response = await _client.PostAsJsonAsync("api/auth/social-login", new
    //        {
    //            Email = email,
    //            Name = name,
    //            Provider = provider,
    //            SocialId = socialId
    //        });

    //        if (!response.IsSuccessStatusCode)
    //            return RedirectToAction("Login");

    //        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

    //        HttpContext.Session.SetString("token", result.Token);

    //        return RedirectToAction("Index", "Employee");
    //    }

    //    public async Task<IActionResult> FacebookResponse()
    //    {
    //        var result = await HttpContext.AuthenticateAsync("Cookies");

    //        if (!result.Succeeded)
    //            return RedirectToAction("Login");

    //        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
    //        var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
    //        var facebookId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    //        if (string.IsNullOrEmpty(email))
    //            return RedirectToAction("Login");

    //        // Call API to get or insert user + generate JWT
    //        var response = await _client.PostAsJsonAsync("api/auth/social-login", new
    //        {
    //            Email = email,
    //            Name = name,
    //            Provider = "Facebook",
    //            SocialId = facebookId
    //        });

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            TempData["Error"] = "Facebook login failed";
    //            return RedirectToAction("Login");
    //        }

    //        var resultData = await response.Content.ReadFromJsonAsync<AuthResponse>();
    //        if (string.IsNullOrEmpty(resultData?.Token))
    //        {
    //            TempData["Error"] = "Token generation failed";
    //            return RedirectToAction("Login");
    //        }

    //        // Store JWT in Session
    //        HttpContext.Session.SetString("token", resultData.Token);

    //        return RedirectToAction("Index", "Employee");
    //    }
    //}
    #endregion
    public class AccountController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IApiService apiService, ILogger<AccountController> logger)
        {
            _apiService = apiService;
            _logger = logger;
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
                _logger.LogInformation($"Login attempt for user: {model.UsernameOrEmail}");

                var result = await _apiService.PostAsync<AuthResponse>("api/auth/login", model);

                if (result == null)
                {
                    _logger.LogWarning("Login failed: API returned null response");
                    ModelState.AddModelError("", "Unable to connect to server. Please try again.");
                    return View(model);
                }

                if (!result.Status || result.Data == null)
                {
                    _logger.LogWarning($"Login failed for user {model.UsernameOrEmail}: {result.Message}");
                    ModelState.AddModelError("", result.Message ?? "Invalid username or password");
                    return View(model);
                }

                var authData = result.Data;

                // ✅ Log received data for debugging
                _logger.LogInformation($"Login successful for user: {authData.Username}");
                _logger.LogInformation($"Roles received: {string.Join(", ", authData.Roles ?? new List<string>())}");
                _logger.LogInformation($"Permissions received: {string.Join(", ", authData.Permissions ?? new List<string>())}");

                // ✅ Store tokens and data in session
                HttpContext.Session.SetString("AccessToken", authData.AccessToken);
                HttpContext.Session.SetString("RefreshToken", authData.RefreshToken ?? "");
                HttpContext.Session.SetString("UserId", authData.UserId.ToString());
                HttpContext.Session.SetString("Username", authData.Username);
                HttpContext.Session.SetString("Email", authData.Email ?? "");           // ✅ Add this
                HttpContext.Session.SetString("FullName", authData.FullName ?? authData.Username);  // ✅ Add this

                HttpContext.Session.SetString("Roles", JsonSerializer.Serialize(authData.Roles ?? new List<string>()));
                HttpContext.Session.SetString("Permissions", JsonSerializer.Serialize(authData.Permissions ?? new List<string>()));

                // ✅ Create claims list
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, authData.UserId.ToString()),
                    new Claim(ClaimTypes.Name, authData.Username),
                    new Claim(ClaimTypes.Email, authData.Email ?? ""),
                    new Claim("FullName", authData.FullName ?? authData.Username)
                };

                // ✅ Add role claims - IMPORTANT!
                if (authData.Roles != null && authData.Roles.Any())
                {
                    foreach (var role in authData.Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                        _logger.LogInformation($"Added role claim: {role}");
                    }
                }
                else
                {
                    _logger.LogWarning("No roles found in auth response!");
                }

                // ✅ Add permission claims
                if (authData.Permissions != null && authData.Permissions.Any())
                {
                    foreach (var permission in authData.Permissions)
                    {
                        claims.Add(new Claim("Permission", permission));
                    }
                }

                // ✅ Create identity and principal
                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                // ✅ Sign in with authentication properties
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

                _logger.LogInformation($"User {authData.Username} signed in successfully with roles: {string.Join(", ", authData.Roles ?? new List<string>())}");

                // ✅ Redirect based on role or return URL
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Redirect based on role
                return RedirectBasedOnRole(authData.Roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Login error for user {model.UsernameOrEmail}");
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
                confirmPassword = model.ConfirmPassword,
                roleId = model.RoleId
            };

            var result = await _apiService.PostAsync<AuthResponse >("api/auth/register", apiModel);

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
         public async Task<IActionResult> Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Sign out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("User logged out");

            return RedirectToAction("Login");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

        // AJAX endpoint for token refresh
        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var refreshToken = HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Json(new { success = false });
            }

            // ✅ FIX: Use AuthResponseDto instead of AuthResponse
            var result = await _apiService.PostAsync<AuthResponse>("api/auth/refresh-token",
                new { AccessToken = accessToken, RefreshToken = refreshToken });

            if (result == null || !result.Status)
            {
                HttpContext.Session.Clear();
                return Json(new { success = false });
            }

            HttpContext.Session.SetString("AccessToken", result.Data!.AccessToken);
            HttpContext.Session.SetString("RefreshToken", result.Data.RefreshToken);

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
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else if (roles.Contains("HR"))
            {
                return RedirectToAction("Index", "Employee");
            }
            else if (roles.Contains("Employee"))
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        #region Password Management

        /// <summary>
        /// Change Password - GET
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        /// <summary>
        /// Change Password - POST
        /// </summary>
        [HttpPost]
        [Authorize]
        
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var token = HttpContext.Session.GetString("AccessToken"); // ✅ Get token

                var request = new
                {
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword,
                    ConfirmNewPassword = model.ConfirmNewPassword
                };

                var result = await _apiService.PostAsync<object>("api/auth/change-password", request, token); // ✅ Pass token


                if (result == null || !result.Status)
                {
                    ModelState.AddModelError("", result?.Message ?? "Failed to change password");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Index", "Employee");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                ModelState.AddModelError("", "An error occurred while changing password");
                return View(model);
            }
        }

        /// <summary>
        /// Forgot Password - GET
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        /// <summary>
        /// Forgot Password - POST
        /// </summary>
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
                var result = await _apiService.PostAsync<bool>("api/auth/forgot-password", new { Email = model.Email });

                // Always show success to prevent email enumeration
                TempData["SuccessMessage"] = "If the email exists in our system, you will receive a password reset link shortly.";
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password");
                TempData["SuccessMessage"] = "If the email exists in our system, you will receive a password reset link shortly.";
                return RedirectToAction("ForgotPasswordConfirmation");
            }
        }

        /// <summary>
        /// Forgot Password Confirmation
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Reset Password - GET
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Invalid password reset link";
                return RedirectToAction("Login");
            }

            try
            {
                // Validate token
                var result = await _apiService.GetAsync<bool>($"api/auth/validate-reset-token?token={token}");

                if (result == null || !result.Status)
                {
                    TempData["Error"] = "Invalid or expired password reset link";
                    return RedirectToAction("Login");
                }

                var model = new ResetPasswordViewModel
                {
                    Token = token,
                    Email = email
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating reset token");
                TempData["Error"] = "Invalid or expired password reset link";
                return RedirectToAction("Login");
            }
        }

        /// <summary>
        /// Reset Password - POST
        /// </summary>
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
                    ModelState.AddModelError("", result?.Message ?? "Failed to reset password");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Password has been reset successfully. Please login with your new password.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                ModelState.AddModelError("", "An error occurred while resetting password");
                return View(model);
            }
        }

        /// <summary>
        /// Admin Reset Password - GET
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminResetPassword(int userId)
        {
            try
            {
                // Get user details
                var user = await _apiService.GetAsync<UserModel>($"api/users/{userId}");

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

        /// <summary>
        /// Admin Reset Password - POST
        /// </summary>
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
                var request = new
                {
                    UserId = model.UserId,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword,
                    SendEmailNotification = model.SendEmailNotification
                };

                var result = await _apiService.PostAsync<bool>("api/auth/reset-password-admin", request);

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
    }
}