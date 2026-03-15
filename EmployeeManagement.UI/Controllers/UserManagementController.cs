using EmployeeManagement.UI.Models;
using EmployeeManagement.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EmployeeManagement.UI.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<UserManagementController> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public UserManagementController(IHttpClientFactory factory,
                                        ILogger<UserManagementController> logger)
        {
            _client = factory.CreateClient("API");
            _logger = logger;
        }

        /// <summary>
        /// Set Authorization Header
        /// </summary>
        private void SetAuth()
        {
            var token = HttpContext.Session.GetString("AccessToken");

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// User Approval Dashboard
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new UserApprovalPageViewModel();

            try
            {
                SetAuth();

                // Run API calls in parallel
                var pendingTask = _client.GetAsync("api/UserManagement/pending");
                var allTask = _client.GetAsync("api/UserManagement/all");
                var rolesTask = _client.GetAsync("api/UserManagement/roles");
                var deptTask = _client.GetAsync("api/Department");

                await Task.WhenAll(pendingTask, allTask, rolesTask, deptTask);

                var pendingResponse = await pendingTask;
                var allResponse = await allTask;
                var rolesResponse = await rolesTask;
                var deptResponse = await deptTask;

                // Pending Users ✅
                if (pendingResponse.IsSuccessStatusCode)
                {
                    var content = await pendingResponse.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<PendingUserViewModel>>>(content, _jsonOptions);
                    viewModel.PendingUsers = result?.Data ?? new List<PendingUserViewModel>();
                }

                // All Users ✅
                if (allResponse.IsSuccessStatusCode)
                {
                    var content = await allResponse.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<PendingUserViewModel>>>(content, _jsonOptions);
                    viewModel.AllUsers = result?.Data ?? new List<PendingUserViewModel>();
                }

                // Roles ✅
                if (rolesResponse.IsSuccessStatusCode)
                {
                    var content = await rolesResponse.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<RoleInfoViewModel>>>(content, _jsonOptions);
                    viewModel.Roles = result?.Data ?? new List<RoleInfoViewModel>();
                }

                // Departments ✅ FIXED
                if (deptResponse.IsSuccessStatusCode)
                {
                    var content = await deptResponse.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<DepartmentViewModel>>>(content, _jsonOptions);
                    viewModel.Departments = result?.Data ?? new List<DepartmentViewModel>();
                }

                // Dashboard Stats
                viewModel.TotalPending = viewModel.PendingUsers.Count;
                viewModel.TotalApproved = viewModel.AllUsers.Count(x => x.RegistrationStatus == "Approved");
                viewModel.TotalRejected = viewModel.AllUsers.Count(x => x.RegistrationStatus == "Rejected");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user management dashboard");
                TempData["Error"] = "Something went wrong while loading the dashboard.";
                return View(viewModel);
            }
        }

        /// <summary>
        /// Approve User & Assign Role
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ApproveUser(int userId, int roleId, int? departmentId)
        {
            try
            {
                SetAuth();

                var request = new
                {
                    UserId = userId,
                    RoleId = roleId,
                    DepartmentId = departmentId
                };

                var response = await _client.PostAsJsonAsync("api/UserManagement/approve", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "User approved and role assigned successfully.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("ApproveUser failed: {Error}", error);

                    TempData["Error"] = "Failed to approve user.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving user");

                TempData["Error"] = "Error while approving the user.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Reject User
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RejectUser(int userId, string? rejectionReason)
        {
            try
            {
                SetAuth();

                var request = new
                {
                    UserId = userId,
                    RejectionReason = rejectionReason
                };

                var response = await _client.PostAsJsonAsync("api/UserManagement/reject", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "User registration rejected.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("RejectUser failed: {Error}", error);

                    TempData["Error"] = "Failed to reject user.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user");

                TempData["Error"] = "Error while rejecting the user.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}