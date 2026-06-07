using EmployeeManagement.UI.Models;
using EmployeeManagement.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EmployeeManagement.UI.Controllers
{
    public class LeaveController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<LeaveController> _logger;
        private readonly string _apiBaseUrl;
        private readonly IMemoryCache _cache;

        // ✅ FIX: Shared JsonSerializerOptions — reuse instead of creating new every time
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public LeaveController(
            IHttpClientFactory factory,
            ILogger<LeaveController> logger,
            IConfiguration config,
            IMemoryCache cache)
        {
            _client = factory.CreateClient("API");
            _logger = logger;
            _apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "http://localhost:26024";
            _cache = cache;
        }

        #region Helper Methods

        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                // ✅ FIX: Always set fresh — prevents stale/duplicate headers
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogWarning("⚠️ No AccessToken found in session!");
                // ✅ Clear any existing auth header
                _client.DefaultRequestHeaders.Authorization = null;
            }
        }

        private int GetCurrentEmployeeId()
        {
            var empId = HttpContext.Session.GetString("UserId");
            return int.TryParse(empId, out var id) ? id : 0;
        }

        private int GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetString("UserId");
            return int.TryParse(userId, out var id) ? id : 0;
        }

        private IActionResult HandleHttpError(Exception ex, string action)
        {
            _logger.LogError(ex, "Error in {Action}: {Message}", action, ex.Message);

            if (ex.Message.Contains("circuit", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("not allowing calls", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "⚠️ API server is temporarily unavailable. Please wait 30 seconds and try again.";
                return RedirectToAction("Index", "Home");
            }

            if (ex is HttpRequestException httpEx)
            {
                if (httpEx.Message.Contains("Connection refused"))
                    TempData["Error"] = "❌ Cannot connect to API server. Please ensure the API is running.";
                else if (httpEx.InnerException is TimeoutException)
                    TempData["Error"] = "⏰ Request timed out. The API server is not responding.";
                else
                    TempData["Error"] = $"Connection error: {httpEx.Message}";
            }
            else if (ex is TaskCanceledException || ex is OperationCanceledException)
            {
                TempData["Error"] = "⏰ Request timed out. Please try again.";
            }
            else
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// ✅ FIX: Cache helper with auth header support
        /// </summary>
        /// <summary>
        /// ✅ FIXED: Cache helper — clears and re-sets auth header properly
        /// </summary>
        private async Task<T?> GetWithCacheAsync<T>(string url, string cacheKey, TimeSpan cacheDuration) where T : class
        {
            // Check cache first
            if (_cache.TryGetValue(cacheKey, out T? cached))
            {
                _logger.LogDebug("Cache HIT: {Key}", cacheKey);
                return cached;
            }

            _logger.LogDebug("Cache MISS: {Key} — calling API: {Url}", cacheKey, url);

            try
            {
                // ✅ FIX: Always clear and re-set auth header before each API call
                var token = HttpContext.Session.GetString("AccessToken");
                if (!string.IsNullOrEmpty(token))
                {
                    // ✅ Remove old auth header and set fresh one
                    _client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    _logger.LogWarning("No AccessToken in session for cache key: {Key}", cacheKey);
                    return null;
                }

                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("API call failed for {Key}: Status={Status}, Error={Error}",
                        cacheKey, response.StatusCode,
                        errorContent.Length > 200 ? errorContent.Substring(0, 200) : errorContent);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogWarning("Empty response for {Key}", cacheKey);
                    return null;
                }

                var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);

                if (result?.Data != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = cacheDuration,
                        SlidingExpiration = cacheDuration.TotalMinutes > 2
                            ? TimeSpan.FromMinutes(1)
                            : null
                    };

                    _cache.Set(cacheKey, result.Data, cacheOptions);
                    _logger.LogDebug("Cache SET: {Key} for {Duration}min", cacheKey, cacheDuration.TotalMinutes);
                }
                else
                {
                    _logger.LogWarning("API response Data is null for {Key}. Status={Status}, Message={Message}",
                        cacheKey, result?.Status, result?.Message);
                }

                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetWithCacheAsync for key: {Key}, URL: {Url}", cacheKey, url);
                return null;
            }
        }
        /// <summary>
        /// ✅ FIX: Clear specific cache entries when data changes
        /// </summary>
        private void ClearLeaveCache(int? employeeId = null)
        {
            _logger.LogInformation("Clearing leave cache for Employee: {EmployeeId}", employeeId ?? 0);

            // Clear employee-specific cache
            if (employeeId.HasValue)
            {
                _cache.Remove($"leave_balance_{employeeId}_{DateTime.Now.Year}");
                _cache.Remove($"leave_history_{employeeId}_{DateTime.Now.Year}");
            }

            // Clear shared caches
            _cache.Remove("mvc_leave_types");
            _cache.Remove($"mvc_holidays_{DateTime.Now.Year}");
            _cache.Remove("pending_leaves");
        }

        /// <summary>
        /// ✅ FIX: Single LoadLeaveTypes method using cache
        /// </summary>
        /// <summary>
        /// ✅ FIXED: Load leave types with fallback
        /// </summary>
        private async Task LoadLeaveTypesAsync(ApplyLeaveViewModel model)
        {
            try
            {
                // Try cache first
                model.LeaveTypes = await GetWithCacheAsync<List<LeaveTypeViewModel>>(
                    "api/Leave/types",
                    "mvc_leave_types",
                    TimeSpan.FromMinutes(30)
                ) ?? new List<LeaveTypeViewModel>();

                // ✅ FIX: If cache returned empty, try direct call
                if (!model.LeaveTypes.Any())
                {
                    _logger.LogWarning("Cache returned empty leave types, trying direct API call");

                    SetAuthorizationHeader();
                    var response = await _client.GetAsync("api/Leave/types");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<ApiResponse<List<LeaveTypeViewModel>>>(
                            content, _jsonOptions);
                        model.LeaveTypes = result?.Data ?? new List<LeaveTypeViewModel>();

                        // Store in cache for next time
                        if (model.LeaveTypes.Any())
                        {
                            _cache.Set("mvc_leave_types", model.LeaveTypes,
                                TimeSpan.FromMinutes(30));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading leave types");
                model.LeaveTypes = new List<LeaveTypeViewModel>();
            }
        }

        #endregion

        #region Leave Dashboard

        /// <summary>
        /// Leave Dashboard - Shows balance, recent requests, upcoming holidays
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                SetAuthorizationHeader();
                var employeeId = GetCurrentEmployeeId();

                if (employeeId == 0)
                {
                    TempData["Error"] = "Employee ID not found. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                var dashboard = new LeaveDashboardViewModel
                {
                    EmployeeId = employeeId,
                    EmployeeName = HttpContext.Session.GetString("FullName")
                };

                // ✅ PARALLEL API CALLS with cache
                var balanceTask = GetWithCacheAsync<List<LeaveBalanceViewModel>>(
                    $"api/Leave/balance/{employeeId}",
                    $"leave_balance_{employeeId}_{DateTime.Now.Year}",
                    TimeSpan.FromMinutes(2));

                var historyTask = GetWithCacheAsync<List<LeaveRequestViewModel>>(
                    $"api/Leave/history/{employeeId}",
                    $"leave_history_{employeeId}_{DateTime.Now.Year}",
                    TimeSpan.FromMinutes(1));

                var holidayTask = GetWithCacheAsync<List<HolidayViewModel>>(
                    "api/Leave/holidays",
                    $"mvc_holidays_{DateTime.Now.Year}",
                    TimeSpan.FromHours(1));

                // Wait for all 3 to complete
                await Task.WhenAll(balanceTask, historyTask, holidayTask);

                // Process results
                dashboard.Balances = await balanceTask ?? new();
                dashboard.RecentRequests = (await historyTask)?.Take(5).ToList() ?? new();
                dashboard.UpcomingHolidays = (await holidayTask)?
                    .Where(h => h.Date >= DateTime.Today)
                    .OrderBy(h => h.Date)
                    .Take(5).ToList() ?? new();

                // Calculate totals
                dashboard.TotalLeaveAvailable = dashboard.Balances.Sum(b => b.TotalAvailable);
                dashboard.TotalLeaveTaken = dashboard.Balances.Sum(b => b.TotalUsed);
                dashboard.TotalPendingRequests = dashboard.RecentRequests.Count(r => r.Status == "Pending");

                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Leave Dashboard");
                TempData["Error"] = $"Error: {ex.Message}";
                return View(new LeaveDashboardViewModel
                {
                    EmployeeId = GetCurrentEmployeeId(),
                    EmployeeName = HttpContext.Session.GetString("FullName")
                });
            }
        }

        #endregion

        #region Apply Leave

        //[HttpGet]
        //public async Task<IActionResult> Apply(int? leaveTypeId = null)
        //{
        //    try
        //    {
        //        SetAuthorizationHeader();

        //        var employeeId = GetCurrentEmployeeId();
        //        if (employeeId == 0)
        //        {
        //            TempData["Error"] = "Please login first.";
        //            return RedirectToAction("Login", "Account");
        //        }

        //        var model = new ApplyLeaveViewModel
        //        {
        //            EmployeeId = employeeId,
        //            EmployeeName = HttpContext.Session.GetString("FullName"),
        //            StartDate = DateTime.Today,
        //            EndDate = DateTime.Today,
        //            LeaveTypeId = leaveTypeId ?? 0  // ✅ Pre-select leave type
        //        };

        //        await LoadLeaveTypesAsync(model);

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleHttpError(ex, nameof(Apply));
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Apply(ApplyLeaveViewModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            await LoadLeaveTypesAsync(model);
        //            return View(model);
        //        }

        //        SetAuthorizationHeader();

        //        if (model.Attachment != null && model.Attachment.Length > 0)
        //        {
        //            using var formContent = new MultipartFormDataContent();
        //            formContent.Add(new StringContent(model.EmployeeId.ToString()), "EmployeeId");
        //            formContent.Add(new StringContent(model.LeaveTypeId.ToString()), "LeaveTypeId");
        //            formContent.Add(new StringContent(model.StartDate.ToString("yyyy-MM-dd")), "StartDate");
        //            formContent.Add(new StringContent(model.EndDate.ToString("yyyy-MM-dd")), "EndDate");
        //            formContent.Add(new StringContent(model.Reason), "Reason");
        //            formContent.Add(new StringContent(model.IsHalfDay.ToString()), "IsHalfDay");

        //            if (!string.IsNullOrEmpty(model.HalfDayType))
        //                formContent.Add(new StringContent(model.HalfDayType), "HalfDayType");

        //            if (!string.IsNullOrEmpty(model.EmergencyContact))
        //                formContent.Add(new StringContent(model.EmergencyContact), "EmergencyContact");

        //            var fileContent = new StreamContent(model.Attachment.OpenReadStream());
        //            fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.Attachment.ContentType);
        //            formContent.Add(fileContent, "attachment", model.Attachment.FileName);

        //            var response = await _client.PostAsync("api/Leave/apply-with-attachment", formContent);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                // ✅ FIX: Clear cache after applying leave
        //                ClearLeaveCache(model.EmployeeId);
        //                TempData["SuccessMessage"] = "Leave applied successfully!";
        //                return RedirectToAction(nameof(History));
        //            }

        //            var error = await response.Content.ReadAsStringAsync();
        //            var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(error, _jsonOptions);
        //            TempData["Error"] = errorResult?.Message ?? "Failed to apply leave.";
        //        }
        //        else
        //        {
        //            var requestBody = new
        //            {
        //                model.EmployeeId,
        //                model.LeaveTypeId,
        //                StartDate = model.StartDate.ToString("yyyy-MM-dd"),
        //                EndDate = model.EndDate.ToString("yyyy-MM-dd"),
        //                model.Reason,
        //                model.IsHalfDay,
        //                model.HalfDayType,
        //                model.EmergencyContact
        //            };

        //            var response = await _client.PostAsJsonAsync("api/Leave/apply", requestBody);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                // ✅ FIX: Clear cache after applying leave
        //                ClearLeaveCache(model.EmployeeId);
        //                TempData["SuccessMessage"] = "Leave applied successfully!";
        //                return RedirectToAction(nameof(History));
        //            }

        //            var error = await response.Content.ReadAsStringAsync();
        //            var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(error, _jsonOptions);
        //            TempData["Error"] = errorResult?.Message ?? "Failed to apply leave.";
        //        }

        //        await LoadLeaveTypesAsync(model);
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error applying leave");
        //        TempData["Error"] = $"Error: {ex.Message}";
        //        await LoadLeaveTypesAsync(model);
        //        return View(model);
        //    }
        //}

        #endregion

        #region Leave History

        [HttpGet]
        public async Task<IActionResult> History(int? year = null)
        {
            try
            {
                SetAuthorizationHeader();
                var employeeId = GetCurrentEmployeeId();
                var selectedYear = year ?? DateTime.Now.Year;

                // ✅ FIX: Use cache for history
                var history = await GetWithCacheAsync<List<LeaveRequestViewModel>>(
                    $"api/Leave/history/{employeeId}?year={selectedYear}",
                    $"leave_history_{employeeId}_{selectedYear}",
                    TimeSpan.FromMinutes(1)
                ) ?? new List<LeaveRequestViewModel>();

                ViewBag.SelectedYear = selectedYear;
                ViewBag.ApiBaseUrl = _apiBaseUrl.TrimEnd('/');

                return View(history);
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(History));
            }
        }

        #endregion

        #region Leave Balance

        [HttpGet]
        public async Task<IActionResult> Balance()
        {
            try
            {
                var employeeId = GetCurrentEmployeeId();

                // ✅ FIX: Use cache for balance
                var balances = await GetWithCacheAsync<List<LeaveBalanceViewModel>>(
                    $"api/Leave/balance/{employeeId}",
                    $"leave_balance_{employeeId}_{DateTime.Now.Year}",
                    TimeSpan.FromMinutes(2)
                ) ?? new List<LeaveBalanceViewModel>();

                return View(balances);
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Balance));
            }
        }

        #endregion

        #region Cancel Leave

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int leaveRequestId, string? cancelReason)
        {
            try
            {
                SetAuthorizationHeader();

                var requestBody = new
                {
                    LeaveRequestId = leaveRequestId,
                    CancelReason = cancelReason
                };

                var response = await _client.PostAsJsonAsync("api/Leave/cancel", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    // ✅ FIX: Clear cache after cancelling
                    ClearLeaveCache(GetCurrentEmployeeId());
                    TempData["SuccessMessage"] = "Leave request cancelled successfully.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(error, _jsonOptions);
                    TempData["Error"] = errorResult?.Message ?? "Failed to cancel leave request.";
                }

                return RedirectToAction(nameof(History));
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Cancel));
            }
        }

        #endregion

        #region Leave Approval

        [HttpGet]
        public async Task<IActionResult> PendingApprovals()
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _client.GetAsync("api/Leave/pending");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to load pending requests.";
                    return View(new LeaveApprovalDashboardViewModel());
                }

                var result = JsonSerializer.Deserialize<ApiResponse<List<LeaveRequestViewModel>>>(content, _jsonOptions);

                var dashboard = new LeaveApprovalDashboardViewModel
                {
                    PendingRequests = result?.Data ?? new List<LeaveRequestViewModel>(),
                    TotalPending = result?.Data?.Count ?? 0
                };

                return View(dashboard);
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(PendingApprovals));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int leaveRequestId, string? remarks)
        {
            try
            {
                SetAuthorizationHeader();

                var requestBody = new
                {
                    LeaveRequestId = leaveRequestId,
                    Remarks = remarks
                };

                var response = await _client.PostAsJsonAsync("api/Leave/approve", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    // ✅ FIX: Clear ALL leave caches after approval
                    ClearLeaveCache();
                    TempData["SuccessMessage"] = "Leave approved successfully.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(error, _jsonOptions);
                    TempData["Error"] = errorResult?.Message ?? "Failed to approve leave.";
                }

                return RedirectToAction(nameof(PendingApprovals));
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Approve));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int leaveRequestId, string? remarks)
        {
            try
            {
                SetAuthorizationHeader();

                var requestBody = new
                {
                    LeaveRequestId = leaveRequestId,
                    Remarks = remarks
                };

                var response = await _client.PostAsJsonAsync("api/Leave/reject", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    // ✅ FIX: Clear ALL leave caches after rejection
                    ClearLeaveCache();
                    TempData["SuccessMessage"] = "Leave rejected.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(error, _jsonOptions);
                    TempData["Error"] = errorResult?.Message ?? "Failed to reject leave.";
                }

                return RedirectToAction(nameof(PendingApprovals));
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Reject));
            }
        }

        #endregion

        #region Holidays

        /// <summary>
        /// ✅ FIX: Holidays — removed double API call, uses cache properly
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Holidays(int? year = null)
        {
            try
            {
                var selectedYear = year ?? DateTime.Now.Year;

                // ✅ FIX: Single call with cache — was calling API twice before!
                var holidays = await GetWithCacheAsync<List<HolidayViewModel>>(
                    $"api/Leave/holidays?year={selectedYear}",
                    $"mvc_holidays_{selectedYear}",
                    TimeSpan.FromHours(1)
                ) ?? new List<HolidayViewModel>();

                // Get stats
                HolidayStatsViewModel? stats = null;
                try
                {
                    stats = await GetWithCacheAsync<HolidayStatsViewModel>(
                        $"api/Leave/holidays/stats?year={selectedYear}",
                        $"mvc_holiday_stats_{selectedYear}",
                        TimeSpan.FromHours(1));
                }
                catch { }

                var model = new HolidayPageViewModel
                {
                    Holidays = holidays,
                    Stats = stats ?? new HolidayStatsViewModel(),
                    SelectedYear = selectedYear
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Holidays));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHoliday(CreateHolidayViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Please fill all required fields.";
                    return RedirectToAction(nameof(Holidays), new { year = model.Date.Year });
                }

                SetAuthorizationHeader();

                var requestBody = new
                {
                    model.Name,
                    Date = model.Date.ToString("yyyy-MM-dd"),
                    model.Type,
                    model.Description
                };

                var response = await _client.PostAsJsonAsync("api/Leave/holidays", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    // ✅ FIX: Clear holiday cache after adding
                    _cache.Remove($"mvc_holidays_{model.Date.Year}");
                    _cache.Remove($"mvc_holiday_stats_{model.Date.Year}");
                    TempData["SuccessMessage"] = $"Holiday '{model.Name}' added successfully!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(error, _jsonOptions);
                    TempData["Error"] = errorResult?.Message ?? "Failed to add holiday.";
                }

                return RedirectToAction(nameof(Holidays), new { year = model.Date.Year });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Holidays));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHoliday(EditHolidayViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Please fill all required fields.";
                    return RedirectToAction(nameof(Holidays), new { year = model.Date.Year });
                }

                SetAuthorizationHeader();

                var requestBody = new
                {
                    model.Id,
                    model.Name,
                    Date = model.Date.ToString("yyyy-MM-dd"),
                    model.Type,
                    model.Description,
                    model.IsActive
                };

                var response = await _client.PutAsJsonAsync($"api/Leave/holidays/{model.Id}", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    // ✅ FIX: Clear holiday cache after editing
                    _cache.Remove($"mvc_holidays_{model.Date.Year}");
                    _cache.Remove($"mvc_holiday_stats_{model.Date.Year}");
                    TempData["SuccessMessage"] = $"Holiday '{model.Name}' updated successfully!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(error, _jsonOptions);
                    TempData["Error"] = errorResult?.Message ?? "Failed to update holiday.";
                }

                return RedirectToAction(nameof(Holidays), new { year = model.Date.Year });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Holidays));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHoliday(int id, int year)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _client.DeleteAsync($"api/Leave/holidays/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // ✅ FIX: Clear holiday cache after deleting
                    _cache.Remove($"mvc_holidays_{year}");
                    _cache.Remove($"mvc_holiday_stats_{year}");
                    TempData["SuccessMessage"] = "Holiday deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete holiday.";
                }

                return RedirectToAction(nameof(Holidays), new { year });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Holidays));
            }
        }

        #endregion

        #region Reports

        [HttpGet]
        public async Task<IActionResult> Reports(int? year = null, int? departmentId = null, int? month = null)
        {
            try
            {
                SetAuthorizationHeader();
                var selectedYear = year ?? DateTime.Now.Year;
                var selectedMonth = month ?? DateTime.Now.Month;

                var viewModel = new LeaveReportPageViewModel
                {
                    SelectedYear = selectedYear,
                    SelectedDepartmentId = departmentId,
                    SelectedMonth = selectedMonth
                };

                // ✅ Parallel API calls
                var statsUrl = $"api/Leave/reports/dashboard-stats?year={selectedYear}";
                var empUrl = $"api/Leave/reports/employee?year={selectedYear}" +
                    (departmentId.HasValue ? $"&departmentId={departmentId}" : "");
                var deptReportUrl = $"api/Leave/reports/department?year={selectedYear}";

                var statsTask = GetWithCacheAsync<DashboardStatsViewModel>(
                    statsUrl, $"report_stats_{selectedYear}", TimeSpan.FromMinutes(5));
                var empTask = GetWithCacheAsync<List<EmployeeLeaveReportViewModel>>(
                    empUrl, $"report_emp_{selectedYear}_{departmentId}", TimeSpan.FromMinutes(5));
                var deptTask = GetWithCacheAsync<List<DepartmentLeaveReportViewModel>>(
                    deptReportUrl, $"report_dept_{selectedYear}", TimeSpan.FromMinutes(5));

                await Task.WhenAll(statsTask, empTask, deptTask);

                viewModel.DashboardStats = await statsTask;
                viewModel.EmployeeReport = await empTask ?? new();
                viewModel.DepartmentReport = await deptTask ?? new();

                // ✅ DEBUG: Log what we got
                _logger.LogInformation("Reports loaded — Stats: {HasStats}, Employees: {EmpCount}, Departments: {DeptCount}",
                    viewModel.DashboardStats != null,
                    viewModel.EmployeeReport.Count,
                    viewModel.DepartmentReport.Count);

                // ✅ DEBUG: If department report is empty, try direct call
                if (!viewModel.DepartmentReport.Any())
                {
                    _logger.LogWarning("⚠️ Department report is EMPTY! Trying direct API call...");

                    try
                    {
                        var directResponse = await _client.GetAsync(deptReportUrl);
                        var directContent = await directResponse.Content.ReadAsStringAsync();

                        _logger.LogInformation("Direct dept report call — Status: {Status}, Content: {Content}",
                            directResponse.StatusCode,
                            directContent.Length > 300 ? directContent.Substring(0, 300) : directContent);

                        if (directResponse.IsSuccessStatusCode)
                        {
                            var directResult = JsonSerializer.Deserialize<ApiResponse<List<DepartmentLeaveReportViewModel>>>(
                                directContent, _jsonOptions);
                            viewModel.DepartmentReport = directResult?.Data ?? new();

                            _logger.LogInformation("Direct call got {Count} department records", viewModel.DepartmentReport.Count);
                        }
                    }
                    catch (Exception apiEx)
                    {
                        _logger.LogError(apiEx, "Direct API call failed for department report");
                    }
                }

                // Get departments for filter dropdown
                try
                {
                    var deptList = await GetWithCacheAsync<List<DepartmentViewModel>>(
                        "api/Department", "mvc_departments", TimeSpan.FromMinutes(30));
                    viewModel.Departments = deptList;
                }
                catch { }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Reports));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(int? year = null, int? departmentId = null)
        {
            try
            {
                SetAuthorizationHeader();

                var url = $"api/Leave/reports/export/excel?year={year ?? DateTime.Now.Year}";
                if (departmentId.HasValue) url += $"&departmentId={departmentId}";

                var response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"LeaveReport_{year ?? DateTime.Now.Year}_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }

                TempData["Error"] = "Failed to export report.";
                return RedirectToAction(nameof(Reports), new { year });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Export error: {ex.Message}";
                return RedirectToAction(nameof(Reports));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv(int? year = null, int? departmentId = null)
        {
            try
            {
                SetAuthorizationHeader();

                var url = $"api/Leave/reports/export/csv?year={year ?? DateTime.Now.Year}";
                if (departmentId.HasValue) url += $"&departmentId={departmentId}";

                var response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    return File(fileBytes, "text/csv",
                        $"LeaveReport_{year ?? DateTime.Now.Year}_{DateTime.Now:yyyyMMddHHmmss}.csv");
                }

                TempData["Error"] = "Failed to export report.";
                return RedirectToAction(nameof(Reports), new { year });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Export error: {ex.Message}";
                return RedirectToAction(nameof(Reports));
            }
        }

        #endregion

        #region Team Calendar

        [HttpGet]
        public async Task<IActionResult> Calendar(int? month = null, int? year = null, int? departmentId = null)
        {
            try
            {
                SetAuthorizationHeader();
                var selectedMonth = month ?? DateTime.Now.Month;
                var selectedYear = year ?? DateTime.Now.Year;

                var viewModel = new LeaveCalendarViewModel
                {
                    SelectedMonth = selectedMonth,
                    SelectedYear = selectedYear,
                    SelectedDepartmentId = departmentId
                };

                // ✅ FIX: Parallel calls with cache
                var calendarUrl = $"api/Leave/calendar?month={selectedMonth}&year={selectedYear}" +
                    (departmentId.HasValue ? $"&departmentId={departmentId}" : "");

                var calendarTask = GetWithCacheAsync<List<LeaveCalendarItemViewModel>>(
                    calendarUrl,
                    $"calendar_{selectedMonth}_{selectedYear}_{departmentId}",
                    TimeSpan.FromMinutes(2));

                var holidayTask = GetWithCacheAsync<List<HolidayViewModel>>(
                    $"api/Leave/holidays?year={selectedYear}",
                    $"mvc_holidays_{selectedYear}",
                    TimeSpan.FromHours(1));

                var deptTask = GetWithCacheAsync<List<DepartmentViewModel>>(
                    "api/Department",
                    "mvc_departments",
                    TimeSpan.FromMinutes(30));

                await Task.WhenAll(calendarTask, holidayTask, deptTask);

                viewModel.LeaveItems = await calendarTask ?? new();
                viewModel.Holidays = (await holidayTask)?
                    .Where(h => h.Date.Month == selectedMonth)
                    .ToList() ?? new();
                viewModel.Departments = await deptTask;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Calendar));
            }
        }

        #endregion

        #region Admin - Leave Allocation

        [HttpGet]
        public async Task<IActionResult> ManageAllocation(int? year = null)
        {
            try
            {
                SetAuthorizationHeader();
                var selectedYear = year ?? DateTime.Now.Year;

                // ✅ FIX: Parallel calls with cache
                var balanceTask = GetWithCacheAsync<List<LeaveBalanceViewModel>>(
                    $"api/Leave/balance/all?year={selectedYear}",
                    $"all_balances_{selectedYear}",
                    TimeSpan.FromMinutes(2));

                var leaveTypesTask = GetWithCacheAsync<List<LeaveTypeViewModel>>(
                    "api/Leave/types",
                    "mvc_leave_types",
                    TimeSpan.FromMinutes(30));

                await Task.WhenAll(balanceTask, leaveTypesTask);

                var balances = await balanceTask ?? new();
                var leaveTypes = await leaveTypesTask ?? new();

                ViewBag.SelectedYear = selectedYear;
                ViewBag.LeaveTypes = leaveTypes;

                var grouped = balances
                    .GroupBy(b => new { b.EmployeeId, b.EmployeeName })
                    .Select(g => new EmployeeLeaveAllocationViewModel
                    {
                        EmployeeId = g.Key.EmployeeId,
                        EmployeeName = g.Key.EmployeeName ?? "Unknown",
                        Balances = g.ToList(),
                        TotalAllocated = g.Sum(b => b.TotalAllocated),
                        TotalUsed = g.Sum(b => b.TotalUsed),
                        TotalAvailable = g.Sum(b => b.TotalAvailable)
                    })
                    .OrderBy(e => e.EmployeeName)
                    .ToList();

                return View(grouped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ManageAllocation");
                TempData["Error"] = $"Error: {ex.Message}";
                return View(new List<EmployeeLeaveAllocationViewModel>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateToAll(int year, decimal leavesPerType)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _client.PostAsJsonAsync("api/Leave/balance/allocate-fixed-all",
                    new { Year = year, LeavesPerType = leavesPerType });
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // ✅ FIX: Clear allocation cache
                    _cache.Remove($"all_balances_{year}");
                    var result = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                    TempData["SuccessMessage"] = result?.Message ?? "Leaves allocated successfully!";
                }
                else
                {
                    var error = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                    TempData["Error"] = error?.Message ?? "Failed to allocate leaves.";
                }

                return RedirectToAction(nameof(ManageAllocation), new { year });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(ManageAllocation));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateToEmployee(int employeeId, int year, decimal leavesPerType)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _client.PostAsJsonAsync("api/Leave/balance/allocate-single",
                    new { EmployeeId = employeeId, Year = year, LeavesPerType = leavesPerType });

                if (response.IsSuccessStatusCode)
                {
                    // ✅ FIX: Clear cache for this employee
                    _cache.Remove($"all_balances_{year}");
                    _cache.Remove($"leave_balance_{employeeId}_{year}");
                    TempData["SuccessMessage"] = $"Leaves allocated to employee {employeeId} successfully!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(error, _jsonOptions);
                    TempData["Error"] = errorResult?.Message ?? "Failed to allocate leaves.";
                }

                return RedirectToAction(nameof(ManageAllocation), new { year });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(ManageAllocation));
            }
        }

        #endregion

        #region Admin Dashboard

        [HttpGet]
        public async Task<IActionResult> AdminDashboard(int? year = null)
        {
            try
            {
                SetAuthorizationHeader();
                var selectedYear = year ?? DateTime.Now.Year;

                // ✅ FIX: Use cache for admin dashboard (short cache - 1 min)
                var dashboard = await GetWithCacheAsync<AdminLeaveDashboardViewModel>(
                    $"api/Leave/admin-dashboard?year={selectedYear}",
                    $"admin_dashboard_{selectedYear}",
                    TimeSpan.FromMinutes(1)
                );

                if (dashboard != null)
                {
                    dashboard.SelectedYear = selectedYear;
                }
                else
                {
                    dashboard = new AdminLeaveDashboardViewModel { SelectedYear = selectedYear };
                    TempData["Error"] = "Failed to load dashboard data.";
                }

                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["Error"] = $"Error: {ex.Message}";
                return View(new AdminLeaveDashboardViewModel { SelectedYear = year ?? DateTime.Now.Year });
            }
        }

        #endregion

        #region File Proxy

        [HttpGet]
        public async Task<IActionResult> ViewAttachment(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return NotFound("File path is empty");

                SetAuthorizationHeader();
                var cleanPath = filePath.TrimStart('/');

                var response = await _client.GetAsync(cleanPath);

                if (!response.IsSuccessStatusCode)
                    return NotFound("File not found on server");

                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                var fileName = Path.GetFileName(filePath);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching attachment: {FilePath}", filePath);
                return NotFound("Error loading file");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PreviewAttachment(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return NotFound("File path is empty");

                SetAuthorizationHeader();
                var cleanPath = filePath.TrimStart('/');

                var response = await _client.GetAsync(cleanPath);

                if (!response.IsSuccessStatusCode)
                    return NotFound("File not found");

                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = Path.GetFileName(filePath);
                var extension = Path.GetExtension(fileName).ToLower();

                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    _ => "application/octet-stream"
                };

                Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing attachment: {FilePath}", filePath);
                return NotFound("Error loading file");
            }
        }

        #endregion
        #region Apply Leave

        /// <summary>
        /// Apply Leave Page (GET)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Apply(int? leaveTypeId = null)
        {
            try
            {
                SetAuthorizationHeader();

                var employeeId = GetCurrentEmployeeId();
                if (employeeId == 0)
                {
                    TempData["Error"] = "Please login first.";
                    return RedirectToAction("Login", "Account");
                }

                var model = new ApplyLeaveViewModel
                {
                    EmployeeId = employeeId,
                    EmployeeName = HttpContext.Session.GetString("FullName"),
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    LeaveTypeId = leaveTypeId ?? 0
                };

                await LoadLeaveTypesAsync(model);

                return View(model);
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Apply));
            }
        }

        /// <summary>
        /// Submit Leave Application (POST) — Renamed to avoid ambiguity
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Apply")]  // ✅ Maps to same URL but different method name
        public async Task<IActionResult> ApplyPost(ApplyLeaveViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadLeaveTypesAsync(model);
                    return View("Apply", model);  // ✅ Explicitly specify view name
                }

                SetAuthorizationHeader();

                if (model.Attachment != null && model.Attachment.Length > 0)
                {
                    using var formContent = new MultipartFormDataContent();
                    formContent.Add(new StringContent(model.EmployeeId.ToString()), "EmployeeId");
                    formContent.Add(new StringContent(model.LeaveTypeId.ToString()), "LeaveTypeId");
                    formContent.Add(new StringContent(model.StartDate.ToString("yyyy-MM-dd")), "StartDate");
                    formContent.Add(new StringContent(model.EndDate.ToString("yyyy-MM-dd")), "EndDate");
                    formContent.Add(new StringContent(model.Reason), "Reason");
                    formContent.Add(new StringContent(model.IsHalfDay.ToString()), "IsHalfDay");

                    if (!string.IsNullOrEmpty(model.HalfDayType))
                        formContent.Add(new StringContent(model.HalfDayType), "HalfDayType");

                    if (!string.IsNullOrEmpty(model.EmergencyContact))
                        formContent.Add(new StringContent(model.EmergencyContact), "EmergencyContact");

                    var fileContent = new StreamContent(model.Attachment.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.Attachment.ContentType);
                    formContent.Add(fileContent, "attachment", model.Attachment.FileName);

                    var response = await _client.PostAsync("api/Leave/apply-with-attachment", formContent);

                    if (response.IsSuccessStatusCode)
                    {
                        ClearLeaveCache(model.EmployeeId);
                        TempData["SuccessMessage"] = "Leave applied successfully!";
                        return RedirectToAction(nameof(History));
                    }

                    var error = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(error, _jsonOptions);
                    TempData["Error"] = errorResult?.Message ?? "Failed to apply leave.";
                }
                else
                {
                    var requestBody = new
                    {
                        model.EmployeeId,
                        model.LeaveTypeId,
                        StartDate = model.StartDate.ToString("yyyy-MM-dd"),
                        EndDate = model.EndDate.ToString("yyyy-MM-dd"),
                        model.Reason,
                        model.IsHalfDay,
                        model.HalfDayType,
                        model.EmergencyContact
                    };

                    var response = await _client.PostAsJsonAsync("api/Leave/apply", requestBody);

                    if (response.IsSuccessStatusCode)
                    {
                        ClearLeaveCache(model.EmployeeId);
                        TempData["SuccessMessage"] = "Leave applied successfully!";
                        return RedirectToAction(nameof(History));
                    }

                    var error = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(error, _jsonOptions);
                    TempData["Error"] = errorResult?.Message ?? "Failed to apply leave.";
                }

                await LoadLeaveTypesAsync(model);
                return View("Apply", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying leave");
                TempData["Error"] = $"Error: {ex.Message}";
                await LoadLeaveTypesAsync(model);
                return View("Apply", model);
            }
        }
        /// <summary>
        /// AJAX: Get pending leave count for badge
        /// </summary>
        //[HttpGet]
        //public async Task<IActionResult> GetPendingCount()
        //{
        //    try
        //    {
        //        SetAuthorizationHeader();
        //        var response = await _client.GetAsync("api/Leave/pending");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            var result = JsonSerializer.Deserialize<ApiResponse<List<LeaveRequestViewModel>>>(
        //                content, _jsonOptions);

        //            return Json(new { count = result?.Data?.Count ?? 0 });
        //        }

        //        return Json(new { count = 0 });
        //    }
        //    catch
        //    {
        //        return Json(new { count = 0 });
        //    }
        //}

        #region Missing Actions

        /// <summary>
        /// AJAX: Get pending leave count for navbar badge
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPendingCount()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _client.GetAsync("api/Leave/pending");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LeaveRequestViewModel>>>(
                        content, _jsonOptions);

                    return Json(new { count = result?.Data?.Count ?? 0 });
                }
                return Json(new { count = 0 });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }

        /// <summary>
        /// View Leave Request Details
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _client.GetAsync($"api/Leave/request/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Leave request not found";
                    return RedirectToAction(nameof(History));
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<LeaveRequestViewModel>>(
                    content, _jsonOptions);

                if (result?.Data == null)
                {
                    TempData["Error"] = "Leave request not found";
                    return RedirectToAction(nameof(History));
                }

                ViewBag.ApiBaseUrl = _apiBaseUrl.TrimEnd('/');
                return View(result.Data);
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Details));
            }
        }

        /// <summary>
        /// Team History - Manager view
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TeamHistory(int? year = null,
            string? status = null, int? departmentId = null,
            int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                SetAuthorizationHeader();
                var selectedYear = year ?? DateTime.Now.Year;

                var url = $"api/Leave/all?pageNumber={pageNumber}&pageSize={pageSize}";
                if (!string.IsNullOrEmpty(status)) url += $"&status={status}";
                if (departmentId.HasValue) url += $"&departmentId={departmentId}";

                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to load team history";
                    return View(new PagedResultViewModel<LeaveRequestViewModel>());
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<PagedResultViewModel<LeaveRequestViewModel>>>(
                    content, _jsonOptions);

                ViewBag.SelectedYear = selectedYear;
                ViewBag.SelectedStatus = status;
                ViewBag.SelectedDepartmentId = departmentId;
                ViewBag.CurrentPage = pageNumber;

                return View(result?.Data ?? new PagedResultViewModel<LeaveRequestViewModel>());
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(TeamHistory));
            }
        }

        /// <summary>
        /// Bulk Approve Multiple Leave Requests
        /// </summary>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> BulkApprove(List<int> leaveIds, string? remarks = null)
        //{
        //    try
        //    {
        //        if (leaveIds == null || !leaveIds.Any())
        //        {
        //            return Json(new { success = false, message = "Please select at least one request" });
        //        }

        //        SetAuthorizationHeader();
        //        int successCount = 0;
        //        int failCount = 0;
        //        var errors = new List<string>();

        //        foreach (var leaveId in leaveIds)
        //        {
        //            try
        //            {
        //                var response = await _client.PostAsJsonAsync("api/Leave/approve",
        //                    new { LeaveRequestId = leaveId, Remarks = remarks });

        //                if (response.IsSuccessStatusCode) successCount++;
        //                else
        //                {
        //                    failCount++;
        //                    errors.Add($"Failed: ID {leaveId}");
        //                }
        //            }
        //            catch
        //            {
        //                failCount++;
        //            }
        //        }

        //        ClearLeaveCache();

        //        return Json(new
        //        {
        //            success = true,
        //            message = $"Approved: {successCount}, Failed: {failCount}",
        //            successCount,
        //            failCount
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        #endregion

        /// <summary>
        /// Bulk Approve Multiple Leave Requests (AJAX)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkApprove(List<int> leaveIds, string? remarks = null)
        {
            try
            {
                if (leaveIds == null || !leaveIds.Any())
                    return Json(new { success = false, message = "No leaves selected" });

                SetAuthorizationHeader();
                int successCount = 0;
                int failCount = 0;
                var errors = new List<string>();

                foreach (var leaveId in leaveIds)
                {
                    try
                    {
                        var response = await _client.PostAsJsonAsync("api/Leave/approve",
                            new { LeaveRequestId = leaveId, Remarks = remarks });

                        if (response.IsSuccessStatusCode) successCount++;
                        else
                        {
                            failCount++;
                            errors.Add($"ID {leaveId}");
                        }
                    }
                    catch
                    {
                        failCount++;
                    }
                }

                ClearLeaveCache();

                return Json(new
                {
                    success = true,
                    message = $"Approved: {successCount}, Failed: {failCount}",
                    successCount,
                    failCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Bulk Reject Multiple Leave Requests (AJAX)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkReject(List<int> leaveIds, string? remarks = null)
        {
            try
            {
                if (leaveIds == null || !leaveIds.Any())
                    return Json(new { success = false, message = "No leaves selected" });

                if (string.IsNullOrWhiteSpace(remarks))
                    return Json(new { success = false, message = "Remarks are required for rejection" });

                SetAuthorizationHeader();
                int successCount = 0;
                int failCount = 0;

                foreach (var leaveId in leaveIds)
                {
                    try
                    {
                        var response = await _client.PostAsJsonAsync("api/Leave/reject",
                            new { LeaveRequestId = leaveId, Remarks = remarks });

                        if (response.IsSuccessStatusCode) successCount++;
                        else failCount++;
                    }
                    catch
                    {
                        failCount++;
                    }
                }

                ClearLeaveCache();

                return Json(new
                {
                    success = true,
                    message = $"Rejected: {successCount}, Failed: {failCount}",
                    successCount,
                    failCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}