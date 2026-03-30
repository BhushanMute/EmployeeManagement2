using EmployeeManagement.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EmployeeManagement.UI.Controllers
{
    public class ReportController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<ReportController> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ReportController(IHttpClientFactory factory, ILogger<ReportController> logger)
        {
            _client = factory.CreateClient("API");
            _logger = logger;
        }

        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// Employee Report
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EmployeeReport( int? departmentId = null, bool? isActive = null, string? searchTerm = null)
        {
            try
            {
                SetAuthorizationHeader();

                // Build URL
                var queryParams = new List<string>();
                if (departmentId.HasValue) queryParams.Add($"departmentId={departmentId}");
                if (isActive.HasValue) queryParams.Add($"isActive={isActive}");
                if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"searchTerm={searchTerm}");

                var url = "api/Report/employee";
                if (queryParams.Any()) url += "?" + string.Join("&", queryParams);

                var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                ViewBag.ReportData = content;
                ViewBag.DepartmentId = departmentId;
                ViewBag.IsActive = isActive;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.ApiSuccess = response.IsSuccessStatusCode;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee report");
                TempData["Error"] = $"Error: {ex.Message}";
                ViewBag.ReportData = "{}";
                ViewBag.ApiSuccess = false;
                return View();
            }
        }

        /// <summary>
        /// Attendance Report
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AttendanceReport(
            int? month = null,
            int? year = null,
            int? departmentId = null)
        {
            try
            {
                SetAuthorizationHeader();

                var m = month ?? DateTime.Now.Month;
                var y = year ?? DateTime.Now.Year;

                var url = $"api/Report/attendance?month={m}&year={y}";
                if (departmentId.HasValue) url += $"&departmentId={departmentId}";

                var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                ViewBag.ReportData = content;
                ViewBag.Month = m;
                ViewBag.Year = y;
                ViewBag.DepartmentId = departmentId;
                ViewBag.ApiSuccess = response.IsSuccessStatusCode;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading attendance report");
                TempData["Error"] = $"Error: {ex.Message}";
                ViewBag.ReportData = "{}";
                ViewBag.Month = month ?? DateTime.Now.Month;
                ViewBag.Year = year ?? DateTime.Now.Year;
                ViewBag.ApiSuccess = false;
                return View();
            }
        }

        /// <summary>
        /// Salary Report
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SalaryReport(
            int? month = null,
            int? year = null,
            int? departmentId = null)
        {
            try
            {
                SetAuthorizationHeader();

                var m = month ?? DateTime.Now.Month;
                var y = year ?? DateTime.Now.Year;

                var url = $"api/Report/salary?month={m}&year={y}";
                if (departmentId.HasValue) url += $"&departmentId={departmentId}";

                var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                ViewBag.ReportData = content;
                ViewBag.Month = m;
                ViewBag.Year = y;
                ViewBag.DepartmentId = departmentId;
                ViewBag.ApiSuccess = response.IsSuccessStatusCode;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading salary report");
                TempData["Error"] = $"Error: {ex.Message}";
                ViewBag.ReportData = "{}";
                ViewBag.Month = month ?? DateTime.Now.Month;
                ViewBag.Year = year ?? DateTime.Now.Year;
                ViewBag.ApiSuccess = false;
                return View();
            }
        }
    }
}