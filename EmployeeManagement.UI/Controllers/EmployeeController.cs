using EmployeeManagement.UI.Models;
using EmployeeManagement.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace EmployeeManagement.UI.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly HttpClient _client;

        public EmployeeController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Account");

                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var employees = await _client.GetFromJsonAsync<List<EmployeeViewModel>>("api/Employee");
                return View(employees);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                TempData["Error"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var departments = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department")
                         ?? new List<DepartmentViewModel>();

                var model = new EmployeeViewModel
                {
                    Departments = departments
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                TempData["Error"] = "Failed to load departments: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            try
            {
                var departments = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department");
                model.Departments = departments;

                if (model.DepartmentId.HasValue)
                {
                    var selectedDept = departments.FirstOrDefault(d => d.Id == model.DepartmentId.Value);
                    model.DepartmentName = selectedDept?.Name;
                }

                var token = HttpContext.Session.GetString("token");
                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Account");

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var postModel = new
                {
                    model.Name,
                    model.Email,
                    model.DepartmentId,
                    model.DepartmentName,
                    model.Salary
                };

                var response = await _client.PostAsJsonAsync("api/Employee", postModel);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Create failed: {error}");
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
                return View(model);
            }
        }

         

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var departments = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department")
                                 ?? new List<DepartmentViewModel>();

                var token = HttpContext.Session.GetString("token");
                if (!string.IsNullOrEmpty(token))
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var emp = await _client.GetFromJsonAsync<EmployeeViewModel>($"api/Employee/{id}");
                if (emp == null) return NotFound();

                emp.Departments = departments;
                return View(emp);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                TempData["Error"] = "Failed to load employee: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            try
            {
                var departments = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department")
                                  ?? new List<DepartmentViewModel>();
                model.Departments = departments;

                if (model.DepartmentId.HasValue)
                {
                    var selectedDept = departments.FirstOrDefault(d => d.Id == model.DepartmentId.Value);
                    model.DepartmentName = selectedDept?.Name;
                }

                var token = HttpContext.Session.GetString("token");
                if (!string.IsNullOrEmpty(token))
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _client.PutAsJsonAsync($"api/Employee/{model.Id}", model);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Update failed: {error}");
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                if (!string.IsNullOrEmpty(token))
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var emp = await _client.GetFromJsonAsync<EmployeeViewModel>($"api/Employee/{id}");
                if (emp == null) return NotFound();

                return View(emp);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                TempData["Error"] = "Failed to load employee: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                if (!string.IsNullOrEmpty(token))
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _client.DeleteAsync($"api/Employee/{id}");
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                TempData["ErrorMessage"] = $"Delete failed: {await response.Content.ReadAsStringAsync()}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}