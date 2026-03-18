using EmployeeManagement.UI.Models;
using EmployeeManagement.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EmployeeManagement.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<AuditLogController> _logger;
        private readonly IMemoryCache _cache;
        private readonly string _apiBaseUrl;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public AuditLogController(
            IHttpClientFactory factory,
            ILogger<AuditLogController> logger,
            IConfiguration config,
            IMemoryCache cache)
        {
            _client = factory.CreateClient("API");
            _logger = logger;
            _cache = cache;
            _apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7192";
        }

        #region Helper Methods

        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogWarning("No AccessToken found in session");
                _client.DefaultRequestHeaders.Authorization = null;
            }
        }

        #endregion

        #region Index

        [HttpGet]
        public async Task<IActionResult> Index(AuditLogFilterViewModel filter)
        {
            try
            {
                SetAuthorizationHeader();

                var token = HttpContext.Session.GetString("AccessToken");
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "Session expired. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                if (filter.PageNumber <= 0) filter.PageNumber = 1;
                if (filter.PageSize <= 0) filter.PageSize = 50;

                var request = new
                {
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    UserId = filter.UserId,
                    Action = filter.Action,
                    EntityName = filter.EntityName,
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate
                };

                var response = await _client.PostAsJsonAsync("api/AuditLog/search", request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<AuditLogResponseViewModel>>(content, _jsonOptions);

                    if (result?.Status == true && result.Data != null)
                    {
                        result.Data.Filter = filter;
                        return View(result.Data);
                    }

                    TempData["Error"] = result?.Message ?? "Failed to load audit logs";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    TempData["Error"] = "Access denied. Admin role required.";
                    return RedirectToAction("Index", "Home");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["Error"] = "Session expired. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                return View(new AuditLogResponseViewModel { Filter = filter });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading audit logs");
                TempData["Error"] = "An error occurred while loading audit logs";
                return View(new AuditLogResponseViewModel { Filter = filter });
            }
        }

        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                SetAuthorizationHeader();

                var request = new
                {
                    PageNumber = 1,
                    PageSize = 1000
                };

                var response = await _client.PostAsJsonAsync("api/AuditLog/search", request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<AuditLogResponseViewModel>>(content, _jsonOptions);

                    if (result?.Data?.Logs != null)
                    {
                        var log = result.Data.Logs.FirstOrDefault(l => l.Id == id);
                        if (log != null)
                        {
                            return View(log);
                        }
                    }

                    TempData["Error"] = "Audit log not found";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading audit log details for ID: {Id}", id);
                TempData["Error"] = "An error occurred";
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Statistics

        [HttpGet]
        public async Task<IActionResult> Statistics(int? days = 30)
        {
            try
            {
                SetAuthorizationHeader();

                var request = new
                {
                    PageNumber = 1,
                    PageSize = 10000
                };

                var response = await _client.PostAsJsonAsync("api/AuditLog/search", request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to load statistics";
                    return View(new AuditLogStatisticsViewModel { SelectedDays = days.Value });
                }

                var result = JsonSerializer.Deserialize<ApiResponse<AuditLogResponseViewModel>>(content, _jsonOptions);
                var logs = result?.Data?.Logs ?? new List<AuditLogViewModel>();

                // Filter by days
                var cutoffDate = DateTime.Today.AddDays(-days.Value);
                logs = logs.Where(l => l.Timestamp >= cutoffDate).ToList();

                var stats = new AuditLogStatisticsViewModel
                {
                    TotalLogs = logs.Count,
                    SelectedDays = days.Value,
                    StartDate = cutoffDate,
                    EndDate = DateTime.Today,

                    ActionCounts = logs
                        .GroupBy(l => l.Action)
                        .Select(g => new ActionCountViewModel { Action = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .ToList(),

                    EntityCounts = logs
                        .Where(l => !string.IsNullOrEmpty(l.EntityName))
                        .GroupBy(l => l.EntityName!)
                        .Select(g => new EntityCountViewModel { EntityName = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .ToList(),

                    UserCounts = logs
                        .Where(l => !string.IsNullOrEmpty(l.Username))
                        .GroupBy(l => l.Username!)
                        .Select(g => new UserCountViewModel { Username = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .Take(10)
                        .ToList(),

                    DailyCounts = logs
                        .GroupBy(l => l.Timestamp.Date)
                        .Select(g => new DailyCountViewModel { Date = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Date)
                        .ToList(),

                    RecentLogs = logs
                        .OrderByDescending(l => l.Timestamp)
                        .Take(10)
                        .ToList()
                };

                return View(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading statistics");
                TempData["Error"] = "Failed to load statistics";
                return View(new AuditLogStatisticsViewModel { SelectedDays = days ?? 30 });
            }
        }

        #endregion

        #region Timeline

        [HttpGet]
        public async Task<IActionResult> Timeline(int? days = 7)
        {
            try
            {
                SetAuthorizationHeader();

                var request = new
                {
                    PageNumber = 1,
                    PageSize = days.Value * 50
                };

                var response = await _client.PostAsJsonAsync("api/AuditLog/search", request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<AuditLogResponseViewModel>>(content, _jsonOptions);
                    return View(result?.Data?.Logs ?? new List<AuditLogViewModel>());
                }

                TempData["Error"] = "Failed to load timeline";
                return View(new List<AuditLogViewModel>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading timeline");
                TempData["Error"] = "An error occurred";
                return View(new List<AuditLogViewModel>());
            }
        }

        #endregion

        #region Export

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportCsv(AuditLogFilterViewModel filter)
        {
            try
            {
                SetAuthorizationHeader();

                var request = new
                {
                    PageNumber = 1,
                    PageSize = 10000,
                    UserId = filter.UserId,
                    Action = filter.Action,
                    EntityName = filter.EntityName,
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate
                };

                var response = await _client.PostAsJsonAsync("api/AuditLog/search", request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to export audit logs";
                    return RedirectToAction("Index", filter);
                }

                var result = JsonSerializer.Deserialize<ApiResponse<AuditLogResponseViewModel>>(content, _jsonOptions);

                if (result?.Data?.Logs == null || !result.Data.Logs.Any())
                {
                    TempData["Error"] = "No data to export";
                    return RedirectToAction("Index", filter);
                }

                var csv = GenerateCsv(result.Data.Logs);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                var fileName = $"AuditLogs_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting audit logs");
                TempData["Error"] = "Export failed";
                return RedirectToAction("Index", filter);
            }
        }

        private string GenerateCsv(List<AuditLogViewModel> logs)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Id,Timestamp,Username,UserId,Action,EntityName,EntityId,IpAddress,OldValues,NewValues");

            foreach (var log in logs)
            {
                var oldValues = EscapeCsvField(log.OldValues);
                var newValues = EscapeCsvField(log.NewValues);

                sb.AppendLine($"{log.Id}," +
                    $"{log.Timestamp:yyyy-MM-dd HH:mm:ss}," +
                    $"\"{log.Username ?? "System"}\"," +
                    $"{log.UserId?.ToString() ?? ""}," +
                    $"\"{log.Action}\"," +
                    $"\"{log.EntityName ?? ""}\"," +
                    $"{log.EntityId?.ToString() ?? ""}," +
                    $"\"{log.IpAddress ?? ""}\"," +
                    $"\"{oldValues}\"," +
                    $"\"{newValues}\"");
            }

            return sb.ToString();
        }

        private string EscapeCsvField(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", "");
        }

        #endregion

        #region AJAX - Get User Details

        /// <summary>
        /// ✅ AJAX endpoint to get user details for modal popup
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            try
            {
                SetAuthorizationHeader();

                _logger.LogInformation("Getting user details for ID: {Id}", id);

                var response = await _client.GetAsync($"api/users/{id}");
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("User API response: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<UserViewModel>>(content, _jsonOptions);

                    if (result?.Status == true && result.Data != null)
                    {
                        var user = result.Data;

                        // Build profile picture URL
                        var profilePicture = user.ProfilePicture;
                        if (!string.IsNullOrEmpty(profilePicture) && !profilePicture.StartsWith("http"))
                        {
                            profilePicture = $"{_apiBaseUrl.TrimEnd('/')}{profilePicture}";
                        }

                        return Json(new
                        {
                            success = true,
                            userId = user.Id,
                            username = user.Username,
                            email = user.Email,
                            fullName = $"{user.FirstName} {user.LastName}".Trim(),
                            firstName = user.FirstName,
                            lastName = user.LastName,
                            role = user.RoleName,
                            isActive = user.IsActive,
                            profilePicture = profilePicture,
                            phoneNumber = user.PhoneNumber,
                            createdAt = user.CreatedAt?.ToString("MMM dd, yyyy")
                        });
                    }
                }

                _logger.LogWarning("User not found for ID: {Id}", id);
                return Json(new { success = false, message = "User not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user details for ID: {Id}", id);
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}