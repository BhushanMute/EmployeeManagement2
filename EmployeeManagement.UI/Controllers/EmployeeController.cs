using EmployeeManagement.UI.Models;
using EmployeeManagement.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace EmployeeManagement.UI.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IHttpClientFactory factory, ILogger<EmployeeController> logger)
        {
            _client = factory.CreateClient("API");
            _logger = logger;
        }

        /// <summary>
        /// Helper method to set authorization header
        /// </summary>
        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("token");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogWarning("No token found in session");
            }
        }

        /// <summary>
        /// Helper method to handle HTTP errors
        /// </summary>
        private IActionResult HandleHttpError(Exception ex, string action)
        {
            _logger.LogError($"Error in {action}: {ex.Message}");

            if (ex is HttpRequestException httpEx)
            {
                if (httpEx.InnerException is TimeoutException)
                {
                    TempData["Error"] = "Request timeout. API server may not be responding. Please check if API is running.";
                }
                else if (httpEx.Message.Contains("Connection refused"))
                {
                    TempData["Error"] = "Cannot connect to API server. Ensure API is running on https://localhost:7192/";
                }
                else if (httpEx.Message.Contains("SSL"))
                {
                    TempData["Error"] = "SSL certificate error. Check HTTPS configuration.";
                }
                else
                {
                    TempData["Error"] = $"Connection error: {httpEx.Message}";
                }
            }
            else
            {
                TempData["Error"] = $"Unexpected error: {ex.Message}";
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
              

                SetAuthorizationHeader();

                _logger.LogInformation("Fetching employees from API");

                // Add timeout to prevent hanging
                var employees = await _client.GetFromJsonAsync<List<EmployeeViewModel>>("api/Employee");

                employees = employees?.OrderBy(e => e.Id).ToList() ?? new List<EmployeeViewModel>();

                _logger.LogInformation($"Successfully fetched {employees.Count} employees");

                return View(employees);
            }
            catch (HttpRequestException ex)
            {
                return HandleHttpError(ex, nameof(Index));
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError($"Request timeout in {nameof(Index)}: {ex.Message}");
                TempData["Error"] = "Request timed out. Please try again.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                SetAuthorizationHeader();

                _logger.LogInformation("Loading departments for Create view");

                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10));
                var departments = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department", cancellationToken: cts.Token)
                    ?? new List<DepartmentViewModel>();

                var model = new EmployeeViewModel
                {
                    Departments = departments
                };

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                return HandleHttpError(ex, nameof(Create));
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Create));
            }
        }

        [HttpPost]
         public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in Create");
                    var departments = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department")
                        ?? new List<DepartmentViewModel>();
                    model.Departments = departments;
                    return View(model);
                }

                SetAuthorizationHeader();

                var departments_list = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department");
                model.Departments = departments_list;

                if (model.DepartmentId.HasValue)
                {
                    var selectedDept = departments_list?.FirstOrDefault(d => d.Id == model.DepartmentId.Value);
                    model.DepartmentName = selectedDept?.Name;
                }

                var postModel = new
                {
                    model.Name,
                    model.Email,
                    model.DepartmentId,
                    model.DepartmentName,
                    model.Salary
                };

                _logger.LogInformation($"Creating new employee: {model.Name}");

                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _client.PostAsJsonAsync("api/Employee", postModel, cancellationToken: cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Employee created successfully: {model.Name}");
                    return RedirectToAction("Index");
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Create failed: {error}");
                ModelState.AddModelError("", $"Create failed: {error}");
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                return HandleHttpError(ex, nameof(Create));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Create: {ex.Message}");
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                SetAuthorizationHeader();

                _logger.LogInformation($"Loading employee {id} for editing");

                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10));

                var departments = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department", cancellationToken: cts.Token)
                    ?? new List<DepartmentViewModel>();

                var emp = await _client.GetFromJsonAsync<EmployeeViewModel>($"api/Employee/{id}", cancellationToken: cts.Token);

                if (emp == null)
                {
                    _logger.LogWarning($"Employee {id} not found");
                    return NotFound();
                }

                emp.Departments = departments;
                return View(emp);
            }
            catch (HttpRequestException ex)
            {
                return HandleHttpError(ex, nameof(Edit));
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Edit));
            }
        }

        [HttpPost]
         public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    _logger.LogWarning($"Invalid model state in Edit for employee {model.Id}");
                //    var departments = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department")
                //        ?? new List<DepartmentViewModel>();
                //    model.Departments = departments;
                //    return View(model);
                //}

                if (model.DepartmentId.HasValue)
                {
                    _logger.LogWarning($"Invalid model state in Edit for employee {model.Id}");

                    var departments = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department")
                        ?? new List<DepartmentViewModel>();
                    var selectedDept = departments
                        .FirstOrDefault(d => d.Id == model.DepartmentId.Value);

                    model.DepartmentName = selectedDept?.Name;
                }

                SetAuthorizationHeader();

                var departments_list = await _client.GetFromJsonAsync<List<DepartmentViewModel>>("api/Department")
                    ?? new List<DepartmentViewModel>();
                model.Departments = departments_list;

                if (model.DepartmentId.HasValue)
                {
                    var selectedDept = departments_list?.FirstOrDefault(d => d.Id == model.DepartmentId.Value);
                    model.DepartmentName = selectedDept?.Name;
                }

                _logger.LogInformation($"Updating employee {model.Id}");

                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _client.PutAsJsonAsync($"api/Employee/{model.Id}", model, cancellationToken: cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Employee {model.Id} updated successfully");
                    return RedirectToAction(nameof(Index));
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Update failed: {error}");
                ModelState.AddModelError("", $"Update failed: {error}");
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                return HandleHttpError(ex, nameof(Edit));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Edit: {ex.Message}");
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                SetAuthorizationHeader();

                _logger.LogInformation($"Loading employee {id} for deletion");

                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10));
                var emp = await _client.GetFromJsonAsync<EmployeeViewModel>($"api/Employee/{id}", cancellationToken: cts.Token);

                if (emp == null)
                {
                    _logger.LogWarning($"Employee {id} not found");
                    return NotFound();
                }

                return View(emp);
            }
            catch (HttpRequestException ex)
            {
                return HandleHttpError(ex, nameof(Delete));
            }
            catch (Exception ex)
            {
                return HandleHttpError(ex, nameof(Delete));
            }
        }

        [HttpPost]
         public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                SetAuthorizationHeader();

                _logger.LogInformation($"Deleting employee {id}");

                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _client.DeleteAsync($"api/Employee/{id}", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Employee {id} deleted successfully");
                    return RedirectToAction(nameof(Index));
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Delete failed: {error}");
                TempData["ErrorMessage"] = $"Delete failed: {error}";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error deleting employee {id}: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to connect to server";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting employee {id}: {ex.Message}");
                TempData["ErrorMessage"] = $"Unexpected error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}