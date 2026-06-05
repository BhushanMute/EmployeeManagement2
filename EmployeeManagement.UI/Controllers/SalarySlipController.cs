using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace EmployeeManagement.UI.Controllers
{
    /// <summary>
    /// Salary Slip MVC Controller - Complete Implementation
    /// वेतन पावती MVC नियंत्रक - संपूर्ण अंमलबजावणी
    /// </summary>
    [Authorize]
    public class SalarySlipController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<SalarySlipController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public SalarySlipController(
            IApiService apiService,
            ILogger<SalarySlipController> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _apiService = apiService;
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        #region Employee Self Service (ESS)

        /// <summary>
        /// My Salary Slips - Employee Dashboard
        /// माझ्या वेतन पावत्या - कर्मचारी डॅशबोर्ड
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MySalarySlips(int? year = null, int? month = null)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                var currentYear = year ?? DateTime.Now.Year;

                var queryString = $"api/SalarySlip/my-slips?year={currentYear}";
                if (month.HasValue)
                    queryString += $"&month={month}";

                var result = await _apiService.GetAsync<List<SalarySlipResponse>>(
                    queryString,
                    token);

                ViewBag.SelectedYear = currentYear;
                ViewBag.SelectedMonth = month;
                ViewBag.AvailableYears = Enumerable.Range(DateTime.Now.Year - 3, 4).Reverse().ToList();

                return View(result?.Data ?? new List<SalarySlipResponse>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading my salary slips");
                TempData["Error"] = "वेतन पावत्या लोड करताना त्रुटी आली";
                return View(new List<SalarySlipResponse>());
            }
        }

        /// <summary>
        /// View Salary Slip Detail
        /// वेतन पावती तपशील पहा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ViewSalarySlip(int slipId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<SalarySlipResponse>(
                    $"api/SalarySlip/{slipId}",
                    token);

                if (result?.Data != null)
                {
                    // Track view
                    await TrackSlipViewAsync(slipId, token);

                    return View(result.Data);
                }

                TempData["Error"] = "वेतन पावती न मिळाली";
                return RedirectToAction(nameof(MySalarySlips));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading salary slip: {SlipId}", slipId);
                TempData["Error"] = "पावती लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(MySalarySlips));
            }
        }

        /// <summary>
        /// Download Salary Slip PDF
        /// वेतन पावती PDF डाउनलोड करा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadSalarySlip(int slipId, string? password = null)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                // Get SSRS Report
                var pdfBytes = await GenerateSSRSSalarySlipPdfAsync(slipId, token);

                if (pdfBytes != null && pdfBytes.Length > 0)
                {
                    // Track download
                    await TrackSlipDownloadAsync(slipId, token);

                    var fileName = $"SalarySlip_{slipId}_{DateTime.Now:yyyyMMdd}.pdf";
                    return File(pdfBytes, "application/pdf", fileName);
                }

                TempData["Error"] = "PDF डाउनलोड अयशस्वी झाली";
                return RedirectToAction(nameof(ViewSalarySlip), new { slipId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading salary slip: {SlipId}", slipId);
                TempData["Error"] = "डाउनलोड करताना त्रुटी आली";
                return RedirectToAction(nameof(MySalarySlips));
            }
        }

        /// <summary>
        /// Download Salary Slip as Excel
        /// वेतन पावती Excel मध्ये डाउनलोड करा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadSalarySlipExcel(int slipId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                // Get SSRS Report as Excel
                var excelBytes = await GenerateSSRSSalarySlipExcelAsync(slipId, token);

                if (excelBytes != null && excelBytes.Length > 0)
                {
                    var fileName = $"SalarySlip_{slipId}_{DateTime.Now:yyyyMMdd}.xlsx";
                    return File(excelBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }

                TempData["Error"] = "Excel डाउनलोड अयशस्वी झाली";
                return RedirectToAction(nameof(ViewSalarySlip), new { slipId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading salary slip Excel: {SlipId}", slipId);
                TempData["Error"] = "डाउनलोड करताना त्रुटी आली";
                return RedirectToAction(nameof(MySalarySlips));
            }
        }

        #endregion

        #region HR Management - Salary Slip

        /// <summary>
        /// Generate Salary Slips for Cycle (HR/Admin)
        /// सायकलसाठी वेतन पावत्या तयार करा (HR/Admin)
        /// </summary>
        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        public async Task<IActionResult> GenerateSalarySlips(int cycleId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                // Get cycle summary
                var cycleResult = await _apiService.GetAsync<PayrollSummaryResponse>(
                    $"api/payroll/cycle/{cycleId}/summary",
                    token);

                if (cycleResult?.Data == null)
                {
                    TempData["Error"] = "वेतन सायकल न मिळाला";
                    return RedirectToAction("Cycles", "Payroll");
                }

                // Check if cycle is in correct status
                if (cycleResult.Data.Status != "Processed" &&
                    cycleResult.Data.Status != "Approved")
                {
                    TempData["Error"] = "फक्त प्रक्रिया केलेल्या किंवा मंजूर वेतन सायकलसाठी पावत्या तयार करता येतात";
                    return RedirectToAction("CycleSummary", "Payroll", new { cycleId });
                }

                ViewBag.CycleId = cycleId;
                return View(cycleResult.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading generate salary slips page");
                TempData["Error"] = "पृष्ठ लोड करताना त्रुटी आली";
                return RedirectToAction("Cycles", "Payroll");
            }
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        public async Task<IActionResult> GenerateSalarySlips(GenerateSalarySlipRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "अवैध विनंती";
                    return RedirectToAction(nameof(GenerateSalarySlips), new { cycleId = request.PayrollCycleId });
                }

                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.PostAsync<bool>(
                    "api/SalarySlip/generate",
                    request,
                    token);

                if (result?.Status == true)
                {
                    TempData["SuccessMessage"] = "वेतन पावत्या यशस्वीरित्या तयार झाल्या";
                    return RedirectToAction(nameof(SalarySlipList), new { cycleId = request.PayrollCycleId });
                }

                TempData["Error"] = result?.Message ?? "पावत्या तयार करणे अयशस्वी झाले";
                return RedirectToAction(nameof(GenerateSalarySlips), new { cycleId = request.PayrollCycleId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating salary slips");
                TempData["Error"] = "त्रुटी आली";
                return RedirectToAction("Cycles", "Payroll");
            }
        }

        /// <summary>
        /// Salary Slip List for Cycle (HR/Admin)
        /// सायकलसाठी वेतन पावत्यांची यादी
        /// </summary>
        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        public async Task<IActionResult> SalarySlipList(int cycleId, int? departmentId = null)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var cycleResult = await _apiService.GetAsync<PayrollSummaryResponse>(
                    $"api/payroll/cycle/{cycleId}/summary",
                    token);

                var registerResult = await _apiService.GetAsync<List<EmployeePayrollDetailResponse>>(
                    $"api/payroll/cycle/{cycleId}/register",
                    token);

                ViewBag.CycleId = cycleId;
                ViewBag.CycleName = cycleResult?.Data?.CycleName ?? "";
                ViewBag.CycleSummary = cycleResult?.Data;

                return View(registerResult?.Data ?? new List<EmployeePayrollDetailResponse>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading salary slip list");
                TempData["Error"] = "यादी लोड करताना त्रुटी आली";
                return RedirectToAction("Cycles", "Payroll");
            }
        }

        /// <summary>
        /// Send Salary Slips via Email (HR/Admin)
        /// ईमेलद्वारे वेतन पावत्या पाठवा
        /// </summary>
        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        public async Task<IActionResult> SendSalarySlipsEmail(int cycleId, List<int>? slipIds = null)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var request = new SendBulkSalarySlipEmailRequest
                {
                    CycleId = cycleId,
                    SlipIds = slipIds,
                    SendToAll = slipIds == null || !slipIds.Any()
                };

                var result = await _apiService.PostAsync<bool>(
                    "api/SalarySlip/send-bulk-email",
                    request,
                    token);

                if (result?.Status == true)
                {
                    return Json(new { success = true, message = "ईमेल यशस्वीरित्या पाठवला" });
                }

                return Json(new { success = false, message = result?.Message ?? "ईमेल पाठवणे अयशस्वी झाले" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending salary slips email");
                return Json(new { success = false, message = "त्रुटी आली" });
            }
        }

        /// <summary>
        /// View Employee Salary Slip (HR can view any employee's slip)
        /// कर्मचाऱ्याची वेतन पावती पहा (HR कोणत्याही कर्मचाऱ्याची पाहू शकतो)
        /// </summary>
        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        public async Task<IActionResult> EmployeeSalarySlip(int employeeId, int? year = null, int? month = null)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var currentYear = year ?? DateTime.Now.Year;
                var queryString = $"api/SalarySlip/employee/{employeeId}?year={currentYear}";

                if (month.HasValue)
                    queryString += $"&month={month}";

                var result = await _apiService.GetAsync<List<SalarySlipResponse>>(queryString, token);

                ViewBag.EmployeeId = employeeId;
                ViewBag.SelectedYear = currentYear;
                ViewBag.SelectedMonth = month;
                ViewBag.AvailableYears = Enumerable.Range(DateTime.Now.Year - 3, 4).Reverse().ToList();

                return View(result?.Data ?? new List<SalarySlipResponse>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee salary slips");
                TempData["Error"] = "पावत्या लोड करताना त्रुटी आली";
                return RedirectToAction("Index", "Employee");
            }
        }

        #endregion

        #region SSRS Report Integration

        /// <summary>
        /// View Salary Slip via SSRS (Inline Report)
        /// SSRS द्वारे वेतन पावती पहा (इनलाइन अहवाल)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ViewSSRSSalarySlip(int slipId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                // Get report URL from API
                var urlResult = await _apiService.GetAsync<string>(
                    $"api/SalarySlip/{slipId}/report-url",
                    token);

                var ssrsSettings = _configuration.GetSection("SSRSSettings");
                var reportServerUrl = ssrsSettings["ReportServerUrl"];
                var reportPath = ssrsSettings["ReportPath"];

                var reportUrl = $"{reportServerUrl}?{reportPath}/SalarySlip" +
                               $"&SlipId={slipId}" +
                               $"&rs:Format=HTML4.0" +
                               $"&rc:Toolbar=false";

                ViewBag.SSRSReportUrl = reportUrl;
                ViewBag.SlipId = slipId;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading SSRS salary slip: {SlipId}", slipId);
                TempData["Error"] = "SSRS अहवाल लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(ViewSalarySlip), new { slipId });
            }
        }

        #endregion

        #region AJAX Methods

        /// <summary>
        /// Get Salary Slip Summary - AJAX
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSlipSummary(int slipId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<SalarySlipResponse>(
                    $"api/SalarySlip/{slipId}",
                    token);

                if (result?.Status == true)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            slipNumber = result.Data?.SlipNumber,
                            monthName = result.Data?.MonthName,
                            year = result.Data?.Year,
                            grossSalary = result.Data?.GrossSalary,
                            totalDeductions = result.Data?.TotalDeductions,
                            netSalary = result.Data?.NetSalary,
                            netSalaryInWords = result.Data?.NetSalaryInWords,
                            status = result.Data?.Status
                        }
                    });
                }

                return Json(new { success = false, message = "Slip not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting slip summary");
                return Json(new { success = false, message = "Error occurred" });
            }
        }

        /// <summary>
        /// Check Email Status - AJAX
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckEmailStatus(int slipId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<SalarySlipResponse>(
                    $"api/SalarySlip/{slipId}",
                    token);

                if (result?.Status == true)
                {
                    return Json(new
                    {
                        success = true,
                        emailSent = result.Data?.EmailSent,
                        emailSentDate = result.Data?.EmailSentDate?.ToString("dd/MM/yyyy HH:mm")
                    });
                }

                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email status");
                return Json(new { success = false });
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Generate SSRS Salary Slip PDF
        /// SSRS द्वारे PDF तयार करा
        /// </summary>
        private async Task<byte[]?> GenerateSSRSSalarySlipPdfAsync(int slipId, string? token)
        {
            try
            {
                var ssrsSettings = _configuration.GetSection("SSRSSettings");
                var reportServerUrl = ssrsSettings["ReportServerUrl"];
                var reportPath = ssrsSettings["ReportPath"];
                var username = ssrsSettings["Username"];
                var password = ssrsSettings["Password"];

                // Build SSRS URL
                var ssrsUrl = $"{reportServerUrl}?{reportPath}/SalarySlip" +
                             $"&SlipId={slipId}" +
                             $"&rs:Format=PDF";

                // Create HTTP Client with Windows Authentication
                var handler = new HttpClientHandler
                {
                    Credentials = new System.Net.NetworkCredential(username, password)
                };

                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = await client.GetAsync(ssrsUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }

                _logger.LogWarning("SSRS PDF generation failed for slip: {SlipId}, Status: {Status}",
                    slipId, response.StatusCode);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating SSRS PDF for slip: {SlipId}", slipId);
                return null;
            }
        }

        /// <summary>
        /// Generate SSRS Salary Slip Excel
        /// SSRS द्वारे Excel तयार करा
        /// </summary>
        private async Task<byte[]?> GenerateSSRSSalarySlipExcelAsync(int slipId, string? token)
        {
            try
            {
                var ssrsSettings = _configuration.GetSection("SSRSSettings");
                var reportServerUrl = ssrsSettings["ReportServerUrl"];
                var reportPath = ssrsSettings["ReportPath"];
                var username = ssrsSettings["Username"];
                var password = ssrsSettings["Password"];

                // Build SSRS URL for Excel
                var ssrsUrl = $"{reportServerUrl}?{reportPath}/SalarySlip" +
                             $"&SlipId={slipId}" +
                             $"&rs:Format=Excel";

                var handler = new HttpClientHandler
                {
                    Credentials = new System.Net.NetworkCredential(username, password)
                };

                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = await client.GetAsync(ssrsUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating SSRS Excel for slip: {SlipId}", slipId);
                return null;
            }
        }

        /// <summary>
        /// Track salary slip view
        /// </summary>
        private async Task TrackSlipViewAsync(int slipId, string? token)
        {
            try
            {
                await _apiService.PostAsync<bool>(
                    $"api/SalarySlip/{slipId}/track-view",
                    null,
                    token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error tracking slip view: {SlipId}", slipId);
            }
        }

        /// <summary>
        /// Track salary slip download
        /// </summary>
        private async Task TrackSlipDownloadAsync(int slipId, string? token)
        {
            try
            {
                await _apiService.PostAsync<bool>(
                    $"api/SalarySlip/{slipId}/track-download",
                    null,
                    token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error tracking slip download: {SlipId}", slipId);
            }
        }

        /// <summary>
        /// Get current user ID
        /// </summary>
        private int GetCurrentUserId()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var id) ? id : 0;
        }

        #endregion
    }

    #region Request Models

    public class SendBulkSalarySlipEmailRequest
    {
        public int CycleId { get; set; }
        public List<int>? SlipIds { get; set; }
        public bool SendToAll { get; set; }
    }

    #endregion
}