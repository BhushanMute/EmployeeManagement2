using EmployeeManagement.UI.Models;
using EmployeeManagement.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
            return View(new LoginModel());
        }

        [HttpPost("Account/Login")]
         public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _client.PostAsJsonAsync("api/auth/login", model);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Login failed";
                    return View(model);
                }

                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

                if (string.IsNullOrEmpty(result?.Token))
                {
                    ViewBag.Error = result?.Message ?? "Invalid credentials";
                    return View(model);
                }

                HttpContext.Session.SetString("token", result.Token);
                return RedirectToAction("Index", "Employee");
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                ViewBag.Error = "Unexpected error occurred: " + ex.Message;
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                TempData["Error"] = "Error clearing session: " + ex.Message;
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
         public async Task<IActionResult> Register(LoginModel model)
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
    }
}