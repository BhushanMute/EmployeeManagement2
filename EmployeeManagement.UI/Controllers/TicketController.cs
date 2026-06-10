// File: EmployeeManagement.UI/Controllers/TicketController.cs
using EmployeeManagement.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EmployeeManagement.UI.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<TicketController> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public TicketController(IHttpClientFactory factory, ILogger<TicketController> logger)
        {
            _client = factory.CreateClient("API");
            _logger = logger;
        }

        #region HELPERS

        private void SetAuth()
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        private int UserId
        {
            get
            {
                var id = HttpContext.Session.GetString("UserId");
                return int.TryParse(id, out var val) ? val : 0;
            }
        }

        private string UserRole
        {
            get
            {
                var roles = HttpContext.Session.GetString("Roles") ?? "";
                if (roles.Contains("Admin")) return "Admin";
                if (roles.Contains("Manager")) return "Manager";
                if (roles.Contains("QA")) return "QA";
                if (roles.Contains("Developer")) return "Developer";
                return "Employee";
            }
        }

        private string UserName =>
            HttpContext.Session.GetString("FullName") ?? "User";

        private async Task<string> CallApiGetAsync(string url)
        {
            SetAuth();
            var response = await _client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<(bool success, string content)> CallApiPostAsync(string url, object data)
        {
            SetAuth();
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, result);
        }

        private async Task<(bool success, string content)> CallApiPutAsync(string url, object data)
        {
            SetAuth();
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, result);
        }

        private void SetCommonViewBag()
        {
            ViewBag.UserRole = UserRole;
            ViewBag.UserId = UserId;
            ViewBag.UserName = UserName;
        }

        #endregion

        #region DASHBOARD

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var data = await CallApiGetAsync("api/Ticket/dashboard");
                SetCommonViewBag();
                ViewBag.DashboardJson = data;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard error");
                TempData["Error"] = "Failed to load dashboard";
                SetCommonViewBag();
                ViewBag.DashboardJson = "{}";
                return View();
            }
        }

        #endregion

        #region MY TICKETS

        [HttpGet]
        public async Task<IActionResult> MyTickets(string? status = null)
        {
            try
            {
                var url = "api/Ticket/my-tickets";
                if (!string.IsNullOrEmpty(status))
                    url += $"?status={status}";

                var data = await CallApiGetAsync(url);
                SetCommonViewBag();
                ViewBag.TicketsJson = data;
                ViewBag.SelectedStatus = status;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyTickets error");
                TempData["Error"] = "Failed to load tickets";
                SetCommonViewBag();
                ViewBag.TicketsJson = "{}";
                return View();
            }
        }

        #endregion

        #region ALL TICKETS LIST

        [HttpGet]
        public async Task<IActionResult> List(
            string? status = null, string? priority = null,
            string? ticketType = null, string? searchTerm = null,
            int? assignedTo = null, int? createdBy = null,
            int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
                if (!string.IsNullOrEmpty(priority)) queryParams.Add($"priority={priority}");
                if (!string.IsNullOrEmpty(ticketType)) queryParams.Add($"ticketType={ticketType}");
                if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"searchTerm={searchTerm}");
                if (assignedTo.HasValue) queryParams.Add($"assignedTo={assignedTo}");
                if (createdBy.HasValue) queryParams.Add($"createdBy={createdBy}");
                queryParams.Add($"pageNumber={pageNumber}");
                queryParams.Add($"pageSize={pageSize}");

                var url = "api/Ticket/list" +
                    (queryParams.Any() ? "?" + string.Join("&", queryParams) : "");

                var data = await CallApiGetAsync(url);

                SetCommonViewBag();
                ViewBag.TicketsJson = data;
                ViewBag.FilterStatus = status;
                ViewBag.FilterPriority = priority;
                ViewBag.FilterType = ticketType;
                ViewBag.FilterSearch = searchTerm;
                ViewBag.PageNumber = pageNumber;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "List error");
                TempData["Error"] = "Failed to load tickets";
                SetCommonViewBag();
                ViewBag.TicketsJson = "{}";
                return View();
            }
        }

        #endregion

        #region CREATE

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var dropdowns = await CallApiGetAsync("api/Ticket/dropdowns");
                SetCommonViewBag();
                ViewBag.DropdownsJson = dropdowns;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create form error");
                TempData["Error"] = "Failed to load form";
                SetCommonViewBag();
                ViewBag.DropdownsJson = "{}";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string title, string description, string ticketType,
            string priority, int? assignedTo, DateTime? dueDate,
            string? stepsToReproduce, string? expectedResult,
            string? actualResult, string? environment,
            string? category, string? module, string? severity)
        {
            try
            {
                var (success, content) = await CallApiPostAsync("api/Ticket/create", new
                {
                    title,
                    description,
                    ticketType,
                    priority,
                    assignedTo,
                    dueDate,
                    stepsToReproduce,
                    expectedResult,
                    actualResult,
                    environment,
                    category,
                    module,
                    severity
                });

                if (success)
                {
                    TempData["Success"] = "🎉 Ticket created successfully!";
                    return RedirectToAction(nameof(MyTickets));
                }

                TempData["Error"] = "Failed to create ticket";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Create));
            }
        }

        #endregion

        #region DETAILS

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var data = await CallApiGetAsync($"api/Ticket/{id}");

                // Try to parse for empty check
                var apiResult = JsonSerializer.Deserialize<JsonElement>(data, _jsonOptions);
                if (!apiResult.TryGetProperty("data", out _) ||
                    apiResult.GetProperty("status").GetBoolean() == false)
                {
                    TempData["Error"] = "Ticket not found";
                    return RedirectToAction(nameof(MyTickets));
                }

                SetCommonViewBag();
                ViewBag.TicketJson = data;

                // Also load dropdowns for assignment
                try
                {
                    var dropdowns = await CallApiGetAsync("api/Ticket/dropdowns");
                    ViewBag.DropdownsJson = dropdowns;
                }
                catch
                {
                    ViewBag.DropdownsJson = "{}";
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Details error for ticket {Id}", id);
                TempData["Error"] = "Failed to load ticket";
                return RedirectToAction(nameof(MyTickets));
            }
        }

        #endregion

        #region EDIT

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var ticketData = await CallApiGetAsync($"api/Ticket/{id}");
                var dropdownData = await CallApiGetAsync("api/Ticket/dropdowns");

                SetCommonViewBag();
                ViewBag.TicketJson = ticketData;
                ViewBag.DropdownsJson = dropdownData;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(MyTickets));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int ticketId, string title, string description,
            string ticketType, string priority, DateTime? dueDate,
            string? stepsToReproduce, string? expectedResult,
            string? actualResult, string? environment,
            string? category, string? module, string? severity)
        {
            try
            {
                var (success, content) = await CallApiPutAsync("api/Ticket/update", new
                {
                    ticketId,
                    title,
                    description,
                    ticketType,
                    priority,
                    dueDate,
                    stepsToReproduce,
                    expectedResult,
                    actualResult,
                    environment,
                    category,
                    module,
                    severity
                });

                TempData[success ? "Success" : "Error"] =
                    success ? "✅ Ticket updated!" : "Failed to update";

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Edit), new { id = ticketId });
            }
        }

        #endregion

        #region STATUS CHANGE (Universal)

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int ticketId, string newStatus, string? remarks)
        {
            try
            {
                var (success, content) = await CallApiPutAsync("api/Ticket/status", new
                {
                    ticketId,
                    newStatus,
                    remarks
                });

                // Check if AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(content, _jsonOptions);
                    var message = result.TryGetProperty("message", out var msg)
                        ? msg.GetString() : (success ? "Status updated" : "Failed");
                    return Json(new { success, message });
                }

                TempData[success ? "Success" : "Error"] =
                    success ? $"✅ Status changed to {newStatus}" : "Failed to update status";

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = ex.Message });

                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
        }

        // Quick action shortcuts
        [HttpPost]
        public Task<IActionResult> StartWork(int id, string? remarks = null)
            => ChangeStatus(id, "InProgress", remarks);

        [HttpPost]
        public Task<IActionResult> MarkFixed(int id, string? remarks = null)
            => ChangeStatus(id, "FixedByDev", remarks);

        [HttpPost]
        public Task<IActionResult> SendToQA(int id, string? remarks = null)
            => ChangeStatus(id, "ReadyForQA", remarks);

        [HttpPost]
        public Task<IActionResult> StartTesting(int id, string? remarks = null)
            => ChangeStatus(id, "InTesting", remarks);

        [HttpPost]
        public Task<IActionResult> Resolve(int id, string? remarks = null)
            => ChangeStatus(id, "Resolved", remarks);

        [HttpPost]
        public Task<IActionResult> Close(int id, string? remarks = null)
            => ChangeStatus(id, "Closed", remarks);

        [HttpPost]
        public Task<IActionResult> Reopen(int id, string? remarks = null)
            => ChangeStatus(id, "Reopened", remarks);

        [HttpPost]
        public Task<IActionResult> PutOnHold(int id, string? remarks = null)
            => ChangeStatus(id, "OnHold", remarks);

        [HttpPost]
        public Task<IActionResult> Cancel(int id, string? remarks = null)
            => ChangeStatus(id, "Cancelled", remarks);

        #endregion

        #region ASSIGN

        [HttpPost]
        public async Task<IActionResult> Assign(int ticketId, int assignedTo, string? remarks)
        {
            try
            {
                var (success, _) = await CallApiPutAsync("api/Ticket/assign", new
                {
                    ticketId,
                    assignedTo,
                    remarks
                });

                TempData[success ? "Success" : "Error"] =
                    success ? "✅ Ticket assigned!" : "Failed to assign";

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
        }

        #endregion

        #region COMMENTS

        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId, string comment, bool isInternal = false)
        {
            try
            {
                var (success, _) = await CallApiPostAsync("api/Ticket/comment", new
                {
                    ticketId,
                    comment,
                    isInternal
                });

                // AJAX response
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success });

                TempData[success ? "Success" : "Error"] =
                    success ? "💬 Comment added!" : "Failed to add comment";

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = ex.Message });

                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
        }

        #endregion

        #region ATTACHMENTS

        [HttpPost]
        public async Task<IActionResult> UploadAttachment(int ticketId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["Error"] = "Please select a file";
                    return RedirectToAction(nameof(Details), new { id = ticketId });
                }

                if (file.Length > 10 * 1024 * 1024)
                {
                    TempData["Error"] = "File must be under 10MB";
                    return RedirectToAction(nameof(Details), new { id = ticketId });
                }

                SetAuth();

                using var formContent = new MultipartFormDataContent();
                formContent.Add(new StringContent(ticketId.ToString()), "ticketId");

                var fileStream = new StreamContent(file.OpenReadStream());
                fileStream.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                formContent.Add(fileStream, "file", file.FileName);

                var response = await _client.PostAsync("api/Ticket/attachment", formContent);

                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode ? "📎 File uploaded!" : "Upload failed";

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int attachmentId)
        {
            try
            {
                SetAuth();
                var response = await _client.GetAsync($"api/Ticket/attachment/{attachmentId}/download");

                if (!response.IsSuccessStatusCode) return NotFound();

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "file";
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

                return File(bytes, contentType, fileName);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAttachment(int attachmentId, int ticketId)
        {
            try
            {
                SetAuth();
                var response = await _client.DeleteAsync($"api/Ticket/attachment/{attachmentId}");

                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode ? "Attachment deleted" : "Failed to delete";

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
        }

        #endregion

        #region DELETE

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (UserRole != "Admin")
                {
                    TempData["Error"] = "Only Admin can delete tickets";
                    return RedirectToAction(nameof(Details), new { id });
                }

                SetAuth();
                var response = await _client.DeleteAsync($"api/Ticket/{id}");

                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                    response.IsSuccessStatusCode ? "🗑️ Ticket deleted" : "Failed to delete";

                return response.IsSuccessStatusCode
                    ? RedirectToAction(nameof(MyTickets))
                    : RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #endregion

        #region BULK OPERATIONS

        [HttpPost]
        public async Task<IActionResult> BulkAssign(List<int> ticketIds, int assignedTo)
        {
            try
            {
                if (!ticketIds.Any())
                    return Json(new { success = false, message = "No tickets selected" });

                SetAuth();
                int ok = 0, fail = 0;

                foreach (var id in ticketIds)
                {
                    var (success, _) = await CallApiPutAsync("api/Ticket/assign", new
                    {
                        ticketId = id,
                        assignedTo,
                        remarks = "Bulk assignment"
                    });
                    if (success) ok++; else fail++;
                }

                return Json(new { success = true, message = $"Assigned: {ok}, Failed: {fail}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkStatusChange(List<int> ticketIds, string newStatus, string? remarks)
        {
            try
            {
                if (!ticketIds.Any())
                    return Json(new { success = false, message = "No tickets selected" });

                SetAuth();
                int ok = 0, fail = 0;

                foreach (var id in ticketIds)
                {
                    var (success, _) = await CallApiPutAsync("api/Ticket/status", new
                    {
                        ticketId = id,
                        newStatus,
                        remarks
                    });
                    if (success) ok++; else fail++;
                }

                return Json(new { success = true, message = $"Updated: {ok}, Failed: {fail}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetNextStatuses(int ticketId)
        {
            try
            {
                var data = await CallApiGetAsync($"api/Ticket/{ticketId}/next-statuses");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message, data = new List<object>() });
            }
        }
        #endregion

        #region AJAX

        [HttpGet]
        public async Task<IActionResult> GetTicketCount()
        {
            try
            {
                SetAuth();
                var url = UserRole is "Admin" or "Manager"
                    ? "api/Ticket/list?status=New&pageSize=1"
                    : $"api/Ticket/list?assignedTo={UserId}&pageSize=1";

                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(content, _jsonOptions);

                    if (result.TryGetProperty("data", out var data) &&
                        data.TryGetProperty("totalRecords", out var total))
                    {
                        return Json(new { count = total.GetInt32() });
                    }
                }
                return Json(new { count = 0 });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }

        #endregion
    }
}