using EmployeeManagement.API.Controllers;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
 
using EmployeeManagement.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.UI.Controllers
{
    /// <summary>
    /// Salary Structure MVC Controller
    /// वेतन रचना MVC नियंत्रक
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    public class SalaryStructureController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<SalaryStructureController> _logger;

        public SalaryStructureController(IApiService apiService, ILogger<SalaryStructureController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        #region Salary Components

        /// <summary>
        /// List salary components
        /// वेतन घटक यादी
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Components(bool activeOnly = true)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<List<SalaryComponent>>(
                    $"api/SalaryStructure/components?activeOnly={activeOnly}",
                    token);

                return View(result?.Data ?? new List<SalaryComponent>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading salary components");
                TempData["Error"] = "वेतन घटक लोड करताना त्रुटी आली";
                return View(new List<SalaryComponent>());
            }
        }

        /// <summary>
        /// Create new salary component
        /// नवीन वेतन घटक तयार करा
        /// </summary>
        [HttpGet]
        public IActionResult CreateComponent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateComponent(SalaryComponent component)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(component);
                }

                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.PostAsync<int>(
                    "api/SalaryStructure/components",
                    component,
                    token);

                if (result?.Status == true)
                {
                    TempData["SuccessMessage"] = $"'{component.ComponentName}' यशस्वीरित्या तयार झाला";
                    return RedirectToAction(nameof(Components));
                }

                TempData["Error"] = result?.Message ?? "तयार करणे अयशस्वी झाले";
                return View(component);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating component");
                TempData["Error"] = "त्रुटी आली";
                return View(component);
            }
        }

        /// <summary>
        /// Edit salary component
        /// वेतन घटक संपादित करा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditComponent(int componentId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<SalaryComponent>(
                    $"api/SalaryStructure/components/{componentId}",
                    token);

                if (result?.Data != null)
                {
                    return View(result.Data);
                }

                TempData["Error"] = "घटक न मिळाला";
                return RedirectToAction(nameof(Components));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading component");
                TempData["Error"] = "घटक लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(Components));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditComponent(int componentId, SalaryComponent component)
        {
            try
            {
                if (componentId != component.Id)
                {
                    TempData["Error"] = "ID मिलत नाहीत";
                    return View(component);
                }

                if (!ModelState.IsValid)
                {
                    return View(component);
                }

                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.PutAsync<bool>(
                    $"api/SalaryStructure/components/{componentId}",
                    component,
                    token);

                if (result?.Status == true)
                {
                    TempData["SuccessMessage"] = "घटक यशस्वीरित्या अपडेट झाला";
                    return RedirectToAction(nameof(Components));
                }

                TempData["Error"] = result?.Message ?? "अपडेट अयशस्वी झाली";
                return View(component);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating component");
                TempData["Error"] = "त्रुटी आली";
                return View(component);
            }
        }

        /// <summary>
        /// Delete salary component
        /// वेतन घटक हटवा
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteComponent(int componentId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.DeleteAsync<bool>(
                    $"api/SalaryStructure/components/{componentId}",
                    token);

                if (result?.Status == true)
                {
                    return Json(new { success = true, message = "घटक यशस्वीरित्या हटवला गेला" });
                }

                return Json(new { success = false, message = result?.Message ?? "हटवणे अयशस्वी झाली" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting component");
                return Json(new { success = false, message = "त्रुटी आली" });
            }
        }

        #endregion

        #region Salary Templates

        /// <summary>
        /// List salary templates
        /// वेतन टेम्पलेट यादी
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Templates(bool activeOnly = true)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<List<SalaryTemplate>>(
                    $"api/SalaryStructure/templates?activeOnly={activeOnly}",
                    token);

                return View(result?.Data ?? new List<SalaryTemplate>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading templates");
                TempData["Error"] = "टेम्पलेट लोड करताना त्रुटी आली";
                return View(new List<SalaryTemplate>());
            }
        }

        /// <summary>
        /// View template details
        /// टेम्पलेट तपशील पहा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TemplateDetails(int templateId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<SalaryTemplate>(
                    $"api/SalaryStructure/templates/{templateId}",
                    token);

                if (result?.Data != null)
                {
                    var components = await _apiService.GetAsync<List<SalaryTemplateComponent>>(
                        $"api/SalaryStructure/templates/{templateId}/components",
                        token);

                    ViewBag.Components = components?.Data ?? new List<SalaryTemplateComponent>();
                    return View(result.Data);
                }

                TempData["Error"] = "टेम्पलेट न मिळाला";
                return RedirectToAction(nameof(Templates));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading template details");
                TempData["Error"] = "तपशील लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(Templates));
            }
        }

        #endregion

        #region Employee Salary Assignment

        /// <summary>
        /// Assign salary to employee
        /// कर्मचाऱ्याला वेतन नियुक्त करा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AssignSalary(int employeeId = 0)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                // Get all templates
                var templatesResult = await _apiService.GetAsync<List<SalaryTemplate>>(
                    "api/SalaryStructure/templates?activeOnly=true",
                    token);

                ViewBag.Templates = templatesResult?.Data ?? new List<SalaryTemplate>();
                ViewBag.EmployeeId = employeeId;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assign salary page");
                TempData["Error"] = "पृष्ठ लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(Templates));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignSalary(AssignSalaryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(request);
                }

                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.PostAsync<int>(
                    "api/SalaryStructure/employee/assign",
                    request,
                    token);

                if (result?.Status == true)
                {
                    TempData["SuccessMessage"] = "वेतन रचना यशस्वीरित्या नियुक्त झाली";
                    return RedirectToAction("Index", "Employee");
                }

                TempData["Error"] = result?.Message ?? "नियुक्त करणे अयशस्वी झाली";
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning salary");
                TempData["Error"] = "त्रुटी आली";
                return View(request);
            }
        }

        /// <summary>
        /// View employee salary structure
        /// कर्मचारी वेतन रचना पहा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ViewEmployeeSalary(int employeeId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<SalaryStructureResponse>(
                    $"api/SalaryStructure/employee/{employeeId}/current",
                    token);

                if (result?.Data != null)
                {
                    return View(result.Data);
                }

                TempData["Error"] = "वेतन रचना न मिळाली";
                return RedirectToAction("Index", "Employee");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee salary");
                TempData["Error"] = "वेतन रचना लोड करताना त्रुटी आली";
                return RedirectToAction("Index", "Employee");
            }
        }

        /// <summary>
        /// View salary history
        /// वेतन इतिहास पहा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SalaryHistory(int employeeId)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.GetAsync<List<SalaryStructureResponse>>(
                    $"api/SalaryStructure/employee/{employeeId}/history",
                    token);

                return View(result?.Data ?? new List<SalaryStructureResponse>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading salary history");
                TempData["Error"] = "इतिहास लोड करताना त्रुटी आली";
                return RedirectToAction("Index", "Employee");
            }
        }

        /// <summary>
        /// Bulk assign salary
        /// अनेक कर्मचाऱ्यांना एकत्रित वेतन नियुक्त करा
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BulkAssignSalary()
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");

                var templatesResult = await _apiService.GetAsync<List<SalaryTemplate>>(
                    "api/SalaryStructure/templates?activeOnly=true",
                    token);

                ViewBag.Templates = templatesResult?.Data ?? new List<SalaryTemplate>();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bulk assign page");
                TempData["Error"] = "पृष्ठ लोड करताना त्रुटी आली";
                return RedirectToAction(nameof(Templates));
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkAssignSalary(BulkSalaryAssignRequest request)
        {
            try
            {
                if (!ModelState.IsValid || request.EmployeeIds == null || !request.EmployeeIds.Any())
                {
                    TempData["Error"] = "कर्मचारी निवडा";
                    return View(request);
                }

                var token = HttpContext.Session.GetString("AccessToken");

                var result = await _apiService.PostAsync<bool>(
                    "api/SalaryStructure/employee/bulk-assign",
                    request,
                    token);

                if (result?.Status == true)
                {
                    TempData["SuccessMessage"] = $"{request.EmployeeIds.Count} कर्मचाऱ्यांना वेतन यशस्वीरित्या नियुक्त झाला";
                    return RedirectToAction(nameof(Templates));
                }

                TempData["Error"] = result?.Message ?? "नियुक्त करणे अयशस्वी झाली";
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk salary assignment");
                TempData["Error"] = "त्रुटी आली";
                return View(request);
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