using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.UI.Controllers
{
    /// <summary>
    /// Payroll MVC Controller
    /// वेतन प्रक्रिया MVC नियंत्रक
    /// </summary>
    [Authorize(Roles = "Admin,HR,Manager")]
    public class PayrollController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<PayrollController> _logger;

        public PayrollController(IApiService apiService, ILogger<PayrollController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        #region Dashboard

        /// <summary>
        /// Payroll Dashboard
        /// वेतन डॅशबोर्ड
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<PayrollDashboardResponse>(
                    "api/payroll/dashboard",
                    token);

                if (result?.Data != null)
                {
                    return View(result.Data);
                }

                TempData["Error"] = "Failed to load payroll dashboard";
                return View(new PayrollDashboardResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payroll dashboard");
                TempData["Error"] = "An error occurred while loading dashboard";
                return View(new PayrollDashboardResponse());
            }
        }

        #endregion

        #region Payroll Cycle Management

        /// <summary>
        /// List all payroll cycles
        /// सर्व वेतन सायकल यादी
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Cycles(int year = 0)
        {
            try
            {
                if (year == 0)
                {
                    year = DateTime.Now.Year;
                }

                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<List<PayrollSummaryResponse>>(
                    $"api/payroll/cycles?year={year}",
                    token);

                ViewBag.SelectedYear = year;
                return View(result?.Data ?? new List<PayrollSummaryResponse>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payroll cycles");
                TempData["Error"] = "Failed to load payroll cycles";
                return View(new List<PayrollSummaryResponse>());
            }
        }

        /// <summary>
        /// Create new payroll cycle
        /// Create new payroll cycle
        /// नवीन वेतन सायकल तयार करा
        /// </summary>
        [HttpGet]
        public IActionResult CreateCycle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCycle(int month, int year)
        {
            try
            {
                if (month < 1 || month > 12)
                {
                    TempData["Error"] = "महिना 1 ते 12 मधील असणे आवश्यक आहे";
                    return RedirectToAction(nameof(CreateCycle));
                }

                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.PostAsync<int>(
                    $"api/payroll/cycle/create?month={month}&year={year}",
                    null,
                    token);

                if (result?.Status == true)
                {
                    TempData["SuccessMessage"] = $"वेतन सायकल {month}/{year} यशस्वीरित्या तयार झाला";
                    return RedirectToAction(nameof(Cycles), new { year });
                }

                TempData["Error"] = result?.Message ?? "वेतन सायकल तयार करणे अयशस्वी झाले";
                return RedirectToAction(nameof(CreateCycle));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payroll cycle");
                TempData["Error"] = "वेतन सायकल तयार करताना त्रुटी आली";
                return RedirectToAction(nameof(CreateCycle));
            }
        }

        /// <summary>
        /// View payroll cycle summary
        /// वेतन सायकल सारांश पहा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CycleSummary(int cycleId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<PayrollSummaryResponse>(
                    $"api/payroll/cycle/{cycleId}/summary",
                    token);

                if (result?.Data != null)
                {
                    return View(result.Data);
                }

                TempData["Error"] = "वेतन सायकल न मिळाला";
                return RedirectToAction(nameof(Cycles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cycle summary");
                TempData["Error"] = "सायकल सारांश लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(Cycles));
            }
        }

        #endregion

        #region Payroll Processing

        /// <summary>
        /// Process payroll for cycle
        /// सायकलसाठी वेतन प्रक्रिया करा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ProcessPayroll(int cycleId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var cycle = await _apiService.GetAsync<PayrollSummaryResponse>(
                    $"api/payroll/cycle/{cycleId}/summary",
                    token);

                if (cycle?.Data != null)
                {
                    return View(cycle.Data);
                }

                TempData["Error"] = "वेतन सायकल न मिळाला";
                return RedirectToAction(nameof(Cycles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading process payroll page");
                TempData["Error"] = "प्रक्रिया पृष्ठ लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(Cycles));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayroll(int cycleId, ProcessPayrollRequest request)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                request.PayrollCycleId = cycleId;

                var result = await _apiService.PostAsync<bool>(
                    "api/payroll/process/bulk",
                    request,
                    token);

                if (result?.Status == true)
                {
                    TempData["SuccessMessage"] = "वेतन प्रक्रिया यशस्वीरित्या पूर्ण झाली";
                    return RedirectToAction(nameof(CycleSummary), new { cycleId });
                }

                TempData["Error"] = result?.Message ?? "वेतन प्रक्रिया अयशस्वी झाली";
                return RedirectToAction(nameof(ProcessPayroll), new { cycleId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payroll");
                TempData["Error"] = "वेतन प्रक्रिया करताना त्रुटी आली";
                return RedirectToAction(nameof(ProcessPayroll), new { cycleId });
            }
        }

        /// <summary>
        /// View payroll register
        /// वेतन नोंदणी पहा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> PayrollRegister(int cycleId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<List<EmployeePayrollDetailResponse>>(
                    $"api/payroll/cycle/{cycleId}/register",
                    token);

                return View(result?.Data ?? new List<EmployeePayrollDetailResponse>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payroll register");
                TempData["Error"] = "वेतन नोंदणी लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(Cycles));
            }
        }

        /// <summary>
        /// View employee payroll details
        /// कर्मचारी वेतन तपशील पहा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EmployeePayrollDetails(int cycleId, int employeeId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<EmployeePayrollDetailResponse>(
                    $"api/payroll/employee/{employeeId}/cycle/{cycleId}",
                    token);

                if (result?.Data != null)
                {
                    return View(result.Data);
                }

                TempData["Error"] = "कर्मचारी वेतन तपशील न मिळाला";
                return RedirectToAction(nameof(PayrollRegister), new { cycleId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee payroll details");
                TempData["Error"] = "तपशील लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(PayrollRegister), new { cycleId });
            }
        }

        #endregion

        #region Payroll Actions

        /// <summary>
        /// Lock payroll cycle
        /// वेतन सायकल लॉक करा
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> LockPayroll(int cycleId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.PostAsync<bool>(
                    $"api/payroll/cycle/{cycleId}/lock",
                    null,
                    token);

                if (result?.Status == true)
                {
                    return Json(new { success = true, message = "वेतन सायकल यशस्वीरित्या लॉक झाला" });
                }

                return Json(new { success = false, message = result?.Message ?? "लॉक करणे अयशस्वी झाले" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking payroll");
                return Json(new { success = false, message = "त्रुटी आली" });
            }
        }

        /// <summary>
        /// Approve payroll cycle
        /// वेतन सायकल मंजूर करा
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ApprovePayroll(int cycleId, string remarks = "")
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.PostAsync<bool>(
                    $"api/payroll/cycle/{cycleId}/approve",
                    remarks,
                    token);

                if (result?.Status == true)
                {
                    return Json(new { success = true, message = "वेतन सायकल यशस्वीरित्या मंजूर झाला" });
                }

                return Json(new { success = false, message = result?.Message ?? "मंजूरी अयशस्वी झाली" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving payroll");
                return Json(new { success = false, message = "त्रुटी आली" });
            }
        }

        /// <summary>
        /// Hold payroll
        /// वेतन होल्ड करा
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> HoldPayroll(int processingId, string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    return Json(new { success = false, message = "कारण आवश्यक आहे" });
                }

                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.PostAsync<bool>(
                    $"api/payroll/{processingId}/hold",
                    reason,
                    token);

                if (result?.Status == true)
                {
                    return Json(new { success = true, message = "वेतन होल्ड यशस्वीरित्या झाला" });
                }

                return Json(new { success = false, message = result?.Message ?? "होल्ड अयशस्वी झाली" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error holding payroll");
                return Json(new { success = false, message = "त्रुटी आली" });
            }
        }

        #endregion

        #region Reports

        /// <summary>
        /// View payroll reports
        /// वेतन अहवाल पहा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Reports(int cycleId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var summaryResult = await _apiService.GetAsync<PayrollSummaryResponse>(
                    $"api/payroll/cycle/{cycleId}/summary",
                    token);

                ViewBag.CycleId = cycleId;
                return View(summaryResult?.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reports page");
                TempData["Error"] = "अहवाल पृष्ठ लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(Cycles));
            }
        }

        /// <summary>
        /// Download payroll register Excel
        /// वेतन नोंदणी Excel डाउनलोड करा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadPayrollRegisterExcel(int cycleId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var fileBytes = await _apiService.GetAsync<byte[]>(
                    $"api/payroll-reports/payroll-register/{cycleId}/export/excel",
                    token);

                if (fileBytes?.Data != null)
                {
                    var fileName = $"PayrollRegister_{cycleId}_{DateTime.Now:yyyyMMdd}.xlsx";
                    return File(fileBytes.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }

                TempData["Error"] = "Excel डाउनलोड अयशस्वी झाला";
                return RedirectToAction(nameof(Reports), new { cycleId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading Excel");
                TempData["Error"] = "डाउनलोड करताना त्रुटी आली";
                return RedirectToAction(nameof(Reports), new { cycleId });
            }
        }

        /// <summary>
        /// Download bank transfer file
        /// बँक हस्तांतरण फाइल डाउनलोड करा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadBankTransferFile(int cycleId, string format = "Excel")
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var fileBytes = await _apiService.GetAsync<byte[]>(
                    $"api/payroll-reports/bank-transfer/{cycleId}/download?format={format}",
                    token);

                if (fileBytes?.Data != null)
                {
                    var ext = format.ToLower() == "excel" ? "xlsx" : "txt";
                    var contentType = format.ToLower() == "excel"
                        ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        : "text/plain";
                    var fileName = $"BankTransfer_{cycleId}_{DateTime.Now:yyyyMMdd}.{ext}";

                    return File(fileBytes.Data, contentType, fileName);
                }

                TempData["Error"] = "बँक फाइल डाउनलोड अयशस्वी झाला";
                return RedirectToAction(nameof(Reports), new { cycleId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading bank file");
                TempData["Error"] = "डाउनलोड करताना त्रुटी आली";
                return RedirectToAction(nameof(Reports), new { cycleId });
            }
        }

        #endregion

        #region Helper Methods

        private int GetCurrentUserId()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;
        }

        #endregion
    }
}