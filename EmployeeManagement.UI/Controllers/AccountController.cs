using EmployeeManagement.API.Models;
using EmployeeManagement.UI.Models;
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
    public class AccountController : Controller
    {
        private readonly HttpClient _client;

        public AccountController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:26024/");                                                                                                                                                                                                                                                   
        }

        [HttpGet("Account/Login")]
        public IActionResult Login()
        {

            //return Challenge(new AuthenticationProperties
            //{
            //    RedirectUri = "/Account/GoogleResponse",

            //}, "Google");
            return View(new LoginModel());
        }

        [HttpPost("Account/Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var request = new LoginRequest
                {
                    Email = model.Email,   // map correctly
                    Password = model.Password
                };

                var response = await _client.PostAsJsonAsync("api/auth/login", request);

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = content;
                    return View(model);
                }

                var result = JsonSerializer.Deserialize<AuthResponse>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Store JWT in Session
                HttpContext.Session.SetString("token", result.Token);

                // ✅ ADD THIS BLOCK (IMPORTANT)
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Email),
            new Claim("JWT", result.Token)
        };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirect to Employee Index
                return RedirectToAction("Index", "Employee");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                HttpContext.Session.Clear();

                await HttpContext.SignOutAsync("Cookies");

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                return RedirectToAction("Login");
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

            try
            {
                var response = await _client.PostAsJsonAsync("api/Auth/Register", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Account created successfully. Please login.";
                    return RedirectToAction("Login");
                }
                else
                {
                    
                    var error = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = error;
                    ErrorLogger.Log(error);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                ViewBag.Error = "Something went wrong: " + ex.Message;
                return View(model);
            }
        }
        public IActionResult GoogleLogin()
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("ExternalLoginCallback")
                },
                GoogleDefaults.AuthenticationScheme);
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Cookies");

            if (!result.Succeeded)
                return RedirectToAction("Login");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            //var Token = result.Principal.FindFirst(ClaimTypes.)?.Value;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");


            var response = await _client.PostAsJsonAsync("api/auth/social-login", new
            {
                Email = email,
                Name = name,
                Provider = "Google",
                SocialId = email
            });

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Google login failed";
                return RedirectToAction("Login");
            }

            var resultData = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (string.IsNullOrEmpty(resultData?.Token))
            {
                TempData["Error"] = "Token generation failed";
                return RedirectToAction("Login");
            }

            // ✅ Store JWT in Session
            HttpContext.Session.SetString("token", resultData.Token);

            return RedirectToAction("Index", "Employee");
        }

        public IActionResult FacebookLogin()
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("ExternalLoginCallback")
                },
                FacebookDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return RedirectToAction("Login");

            var principal = authenticateResult.Principal;

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value;
            var socialId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // detect provider automatically
            var provider = authenticateResult.Properties.Items[".AuthScheme"];

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            // Call your API (COMMON FOR ALL PROVIDERS)
            var response = await _client.PostAsJsonAsync("api/auth/social-login", new
            {
                Email = email,
                Name = name,
                Provider = provider,
                SocialId = socialId
            });

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            HttpContext.Session.SetString("token", result.Token);

            return RedirectToAction("Index", "Employee");
        }

        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Cookies");

            if (!result.Succeeded)
                return RedirectToAction("Login");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var facebookId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            // Call API to get or insert user + generate JWT
            var response = await _client.PostAsJsonAsync("api/auth/social-login", new
            {
                Email = email,
                Name = name,
                Provider = "Facebook",
                SocialId = facebookId
            });

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Facebook login failed";
                return RedirectToAction("Login");
            }

            var resultData = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (string.IsNullOrEmpty(resultData?.Token))
            {
                TempData["Error"] = "Token generation failed";
                return RedirectToAction("Login");
            }

            // Store JWT in Session
            HttpContext.Session.SetString("token", resultData.Token);

            return RedirectToAction("Index", "Employee");
        }
    }
}