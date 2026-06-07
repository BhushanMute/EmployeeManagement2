// File: EmployeeManagement.UI/Controllers/UserManagementController.cs
using EmployeeManagement.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EmployeeManagement.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<UserManagementController> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public UserManagementController(IHttpClientFactory factory, ILogger<UserManagementController> logger)
        {
            _client = factory.CreateClient("API");
            _logger = logger;
        }

        // ============================================================
        // 🔧 HELPERS
        // ============================================================

        private void SetAuth()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                _logger.LogDebug("Auth token set");
            }
            else
            {
                _logger.LogWarning("⚠️ No AccessToken in session!");
                _client.DefaultRequestHeaders.Authorization = null;
            }
        }

        private async Task<string> CallApiGetAsync(string url)
        {
            SetAuth();
            _logger.LogInformation("📤 GET: {Url}", url);
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("📥 GET {Url} - Status: {Status}", url, response.StatusCode);
            return content;
        }

        private async Task<(bool success, string content, System.Net.HttpStatusCode status)>
            CallApiPostAsync(string url, object data)
        {
            SetAuth();
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("📤 POST: {Url}, Body: {Body}", url, json);
            var response = await _client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("📥 POST {Url} - Status: {Status}, Response: {Resp}",
                url, response.StatusCode, result);

            return (response.IsSuccessStatusCode, result, response.StatusCode);
        }

        private async Task<(bool success, string content)> CallApiPutAsync(string url, object data)
        {
            SetAuth();
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("📤 PUT: {Url}, Body: {Body}", url, json);
            var response = await _client.PutAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("📥 PUT {Url} - Status: {Status}", url, response.StatusCode);

            return (response.IsSuccessStatusCode, result);
        }

        // ============================================================
        // 📋 USER LIST
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Index(string? search = null, string? role = null,
            string? status = null, int pageNumber = 1)
        {
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
                if (!string.IsNullOrEmpty(role)) queryParams.Add($"role={Uri.EscapeDataString(role)}");
                if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={Uri.EscapeDataString(status)}");
                queryParams.Add($"pageNumber={pageNumber}");
                queryParams.Add("pageSize=20");

                var url = "api/UserManagement/users?" + string.Join("&", queryParams);
                var content = await CallApiGetAsync(url);

                ViewBag.UsersJson = content;
                ViewBag.SearchTerm = search;
                ViewBag.SelectedRole = role;
                ViewBag.SelectedStatus = status;
                ViewBag.PageNumber = pageNumber;

                // Get roles for filter
                var rolesContent = await CallApiGetAsync("api/UserManagement/roles");
                ViewBag.RolesJson = rolesContent;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                TempData["Error"] = $"Failed to load users: {ex.Message}";
                ViewBag.UsersJson = "{}";
                ViewBag.RolesJson = "{}";
                return View();
            }
        }

        // ============================================================
        // ➕ CREATE USER
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var content = await CallApiGetAsync("api/UserManagement/dropdowns");
                ViewBag.DropdownsJson = content;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form");
                TempData["Error"] = $"Error: {ex.Message}";
                ViewBag.DropdownsJson = "{}";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string username, string fullName, string email, string? phoneNumber,
            string password, string roleIds, int? departmentId, int? designationId, string? employeeCode)
        {
            try
            {
                // ✅ Basic validation
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fullName) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(roleIds))
                {
                    TempData["Error"] = "All required fields must be filled";
                    return RedirectToAction(nameof(Create));
                }

                var data = new
                {
                    username,
                    fullName,
                    email,
                    phoneNumber,
                    password,
                    roleIds,
                    departmentId,
                    designationId,
                    employeeCode
                };

                var (success, content, statusCode) = await CallApiPostAsync("api/UserManagement/users", data);

                if (success)
                {
                    TempData["Success"] = "✅ User created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                // ✅ Parse error from API
                string errorMessage = "Failed to create user";
                try
                {
                    var errorResp = JsonSerializer.Deserialize<JsonElement>(content, _jsonOptions);
                    if (errorResp.TryGetProperty("message", out var msg))
                        errorMessage = msg.GetString() ?? errorMessage;
                }
                catch { }

                TempData["Error"] = $"❌ {errorMessage}";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Create));
            }
        }

        // ============================================================
        // ✏️ EDIT USER
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userContent = await CallApiGetAsync($"api/UserManagement/users/{id}");
                var dropContent = await CallApiGetAsync("api/UserManagement/dropdowns");

                ViewBag.UserJson = userContent;
                ViewBag.DropdownsJson = dropContent;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for user {Id}", id);
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int userId, string fullName, string email,
            string? phoneNumber, bool isActive, string roleIds,
            int? departmentId, int? designationId, string? employeeCode)
        {
            try
            {
                var data = new
                {
                    fullName,
                    email,
                    phoneNumber,
                    isActive,
                    roleIds,
                    departmentId,
                    designationId,
                    employeeCode
                };

                var (success, content) = await CallApiPutAsync($"api/UserManagement/users/{userId}", data);

                if (success)
                {
                    TempData["Success"] = "✅ User updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                string errorMessage = "Failed to update user";
                try
                {
                    var errorResp = JsonSerializer.Deserialize<JsonElement>(content, _jsonOptions);
                    if (errorResp.TryGetProperty("message", out var msg))
                        errorMessage = msg.GetString() ?? errorMessage;
                }
                catch { }

                TempData["Error"] = $"❌ {errorMessage}";
                return RedirectToAction(nameof(Edit), new { id = userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", userId);
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Edit), new { id = userId });
            }
        }

        // ============================================================
        // 🔄 TOGGLE STATUS (AJAX)
        // ============================================================

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            try
            {
                var data = new { isActive };
                var (success, content) = await CallApiPutAsync($"api/UserManagement/users/{id}/toggle-status", data);

                return Json(new
                {
                    success,
                    message = success ? "Status updated" : "Failed to update status"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for user {Id}", id);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // 🔑 RESET PASSWORD (AJAX)
        // ============================================================

        [HttpPost]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                    return Json(new { success = false, message = "Password is required" });

                var data = new { newPassword };
                var (success, content) = await CallApiPutAsync(
                    $"api/UserManagement/users/{id}/reset-password", data);

                return Json(new
                {
                    success,
                    message = success ? "Password reset successfully" : "Failed to reset password"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user {Id}", id);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // 🗑️ DELETE USER
        // ============================================================

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                SetAuth();
                var response = await _client.DeleteAsync($"api/UserManagement/users/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "🗑️ User deleted successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Failed to delete user";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {Id}", id);
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ============================================================
        // 🛡️ ROLE MANAGEMENT
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            try
            {
                var content = await CallApiGetAsync("api/UserManagement/roles");
                ViewBag.RolesJson = content;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading roles");
                TempData["Error"] = ex.Message;
                ViewBag.RolesJson = "{}";
                return View();
            }
        }

        /// <summary>
        /// ✅ FIXED: Matches API endpoint /roles (lowercase)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName, string? roleDescription)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    TempData["Error"] = "Role name is required";
                    return RedirectToAction(nameof(Roles));
                }

                var data = new { roleName, roleDescription };

                // ✅ FIXED: Use lowercase "roles" endpoint
                var (success, content, statusCode) = await CallApiPostAsync("api/UserManagement/roles", data);

                if (success)
                {
                    TempData["Success"] = "✅ Role created successfully!";
                }
                else
                {
                    string errorMessage = "Failed to create role";
                    try
                    {
                        var errorResp = JsonSerializer.Deserialize<JsonElement>(content, _jsonOptions);
                        if (errorResp.TryGetProperty("message", out var msg))
                            errorMessage = msg.GetString() ?? errorMessage;
                    }
                    catch { }

                    TempData["Error"] = $"❌ {errorMessage} (Status: {statusCode})";
                }

                return RedirectToAction(nameof(Roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Roles));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(int roleId, string roleName,
            string? roleDescription, bool isActive)
        {
            try
            {
                var data = new { roleName, roleDescription, isActive };
                var (success, content) = await CallApiPutAsync(
                    $"api/UserManagement/roles/{roleId}", data);

                TempData[success ? "Success" : "Error"] =
                    success ? "✅ Role updated!" : "Failed to update role";

                return RedirectToAction(nameof(Roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {Id}", roleId);
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Roles));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                SetAuth();
                var response = await _client.DeleteAsync($"api/UserManagement/roles/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    string errorMessage = "Failed to delete role";
                    try
                    {
                        var errorResp = JsonSerializer.Deserialize<JsonElement>(content, _jsonOptions);
                        if (errorResp.TryGetProperty("message", out var msg))
                            errorMessage = msg.GetString() ?? errorMessage;
                    }
                    catch { }

                    return Json(new { success = false, message = errorMessage });
                }

                return Json(new { success = true, message = "Role deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {Id}", id);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // 🐛 DEBUG ENDPOINT (Temporary - Remove in production)
        // ============================================================

        [HttpGet]
        public IActionResult Debug()
        {
            var token = HttpContext.Session.GetString("AccessToken") ?? "MISSING";
            var roles = HttpContext.Session.GetString("Roles") ?? "MISSING";
            var userId = HttpContext.Session.GetString("UserId") ?? "MISSING";

            return Json(new
            {
                hasToken = !string.IsNullOrEmpty(token) && token != "MISSING",
                tokenPreview = token.Length > 20 ? token.Substring(0, 20) + "..." : token,
                roles,
                userId,
                isAdmin = User.IsInRole("Admin"),
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }
}