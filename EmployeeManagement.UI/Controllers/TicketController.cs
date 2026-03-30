using EmployeeManagement.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmployeeManagement.UI.Controllers
{
    /// <summary>
    /// Ticket Management UI Controller
    /// Handles QA-Developer Ticket System Views
    /// </summary>
    [Authorize]
    public class TicketController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<TicketController> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public TicketController(IHttpClientFactory factory, ILogger<TicketController> logger)
        {
            _client = factory.CreateClient("API");
            _logger = logger;
        }

        #region HELPER METHODS

        /// <summary>
        /// Set Authorization Header with JWT Token
        /// </summary>
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
        /// Get Current User ID from Session
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdString, out var userId) ? userId : 0;
        }

        /// <summary>
        /// Get Current User Role from Session
        /// </summary>
        private string GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("UserRole") ?? "Employee";
        }

        /// <summary>
        /// Get Current User Name from Session
        /// </summary>
        private string GetCurrentUserName()
        {
            return HttpContext.Session.GetString("UserName") ?? "User";
        }

        #endregion

        #region TICKET LIST & DASHBOARD

        /// <summary>
        /// Ticket Dashboard
        /// GET: /Ticket/Index
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                SetAuthorizationHeader();

                // Get dashboard data
                var response = await _client.GetAsync("api/Ticket/dashboard?myTicketsOnly=false");
                var content = await response.Content.ReadAsStringAsync();

                ViewBag.DashboardData = content;
                ViewBag.UserRole = GetCurrentUserRole();
                ViewBag.UserName = GetCurrentUserName();
                ViewBag.ApiSuccess = response.IsSuccessStatusCode;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ticket dashboard");
                TempData["Error"] = $"Error loading dashboard: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// My Tickets (Role-based)
        /// GET: /Ticket/MyTickets
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyTickets(string? status = null)
        {
            try
            {
                SetAuthorizationHeader();

                var url = $"api/Ticket/my-tickets";
                if (!string.IsNullOrEmpty(status))
                {
                    url += $"?status={status}";
                }

                var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                ViewBag.TicketsData = content;
                ViewBag.SelectedStatus = status;
                ViewBag.UserRole = GetCurrentUserRole();
                ViewBag.ApiSuccess = response.IsSuccessStatusCode;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading my tickets");
                TempData["Error"] = $"Error: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// All Tickets List with Filters
        /// GET: /Ticket/List
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List(
            string? status = null,
            string? priority = null,
            string? ticketType = null,
            string? searchTerm = null,
            int pageNumber = 1)
        {
            try
            {
                SetAuthorizationHeader();

                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
                if (!string.IsNullOrEmpty(priority)) queryParams.Add($"priority={priority}");
                if (!string.IsNullOrEmpty(ticketType)) queryParams.Add($"ticketType={ticketType}");
                if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"searchTerm={searchTerm}");
                queryParams.Add($"pageNumber={pageNumber}");
                queryParams.Add($"pageSize=20");

                var url = "api/Ticket/list";
                if (queryParams.Any())
                    url += "?" + string.Join("&", queryParams);

                var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                ViewBag.TicketsData = content;
                ViewBag.Status = status;
                ViewBag.Priority = priority;
                ViewBag.TicketType = ticketType;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.PageNumber = pageNumber;
                ViewBag.ApiSuccess = response.IsSuccessStatusCode;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tickets list");
                TempData["Error"] = $"Error: {ex.Message}";
                return View();
            }
        }

        #endregion

        #region CREATE TICKET

        /// <summary>
        /// Show Create Ticket Form
        /// GET: /Ticket/Create
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                SetAuthorizationHeader();

                // Get dropdown data
                var response = await _client.GetAsync("api/Ticket/dropdowns");
                var content = await response.Content.ReadAsStringAsync();

                ViewBag.DropdownData = content;
                ViewBag.ApiSuccess = response.IsSuccessStatusCode;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create ticket form");
                TempData["Error"] = $"Error: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// Create New Ticket
        /// POST: /Ticket/Create
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string title,
            string description,
            string ticketType,
            string priority,
            int? assignedTo,
            DateTime? dueDate,
            string? stepsToReproduce,
            string? expectedResult,
            string? actualResult,
            string? environment)
        {
            try
            {
                SetAuthorizationHeader();

                var requestData = new
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
                    environment
                };

                var json = JsonSerializer.Serialize(requestData);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("api/Ticket/create", httpContent);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ticket created successfully!";
                    return RedirectToAction(nameof(MyTickets));
                }
                else
                {
                    TempData["Error"] = $"Failed to create ticket: {content}";
                    return RedirectToAction(nameof(Create));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ticket");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Create));
            }
        }

        #endregion

        #region VIEW TICKET DETAILS

        /// <summary>
        /// View Ticket Details
        /// GET: /Ticket/Details/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _client.GetAsync($"api/Ticket/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Ticket not found";
                    return RedirectToAction(nameof(MyTickets));
                }

                ViewBag.TicketData = content;
                ViewBag.UserRole = GetCurrentUserRole();
                ViewBag.UserId = GetCurrentUserId();
                ViewBag.ApiSuccess = true;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ticket details");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(MyTickets));
            }
        }

        #endregion

        #region EDIT TICKET

        /// <summary>
        /// Show Edit Ticket Form
        /// GET: /Ticket/Edit/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                SetAuthorizationHeader();

                // Get ticket data
                var ticketResponse = await _client.GetAsync($"api/Ticket/{id}");
                var ticketContent = await ticketResponse.Content.ReadAsStringAsync();

                if (!ticketResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Ticket not found";
                    return RedirectToAction(nameof(MyTickets));
                }

                // Get dropdown data
                var dropdownResponse = await _client.GetAsync("api/Ticket/dropdowns");
                var dropdownContent = await dropdownResponse.Content.ReadAsStringAsync();

                ViewBag.TicketData = ticketContent;
                ViewBag.DropdownData = dropdownContent;
                ViewBag.ApiSuccess = true;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit ticket form");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(MyTickets));
            }
        }

        /// <summary>
        /// Update Ticket
        /// POST: /Ticket/Edit
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int ticketId,
            string title,
            string description,
            string ticketType,
            string priority,
            DateTime? dueDate,
            string? stepsToReproduce,
            string? expectedResult,
            string? actualResult,
            string? environment)
        {
            try
            {
                SetAuthorizationHeader();

                var requestData = new
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
                    environment
                };

                var json = JsonSerializer.Serialize(requestData);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync("api/Ticket/update", httpContent);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ticket updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = ticketId });
                }
                else
                {
                    TempData["Error"] = $"Failed to update ticket: {content}";
                    return RedirectToAction(nameof(Edit), new { id = ticketId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ticket");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Edit), new { id = ticketId });
            }
        }

        #endregion

        #region STATUS OPERATIONS

        /// <summary>
        /// Update Ticket Status
        /// POST: /Ticket/UpdateStatus
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int ticketId, string newStatus, string? remarks)
        {
            try
            {
                SetAuthorizationHeader();

                var requestData = new
                {
                    ticketId,
                    newStatus,
                    remarks
                };

                var json = JsonSerializer.Serialize(requestData);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync("api/Ticket/status", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = $"Ticket status updated to {newStatus}";
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Failed to update status: {content}";
                }

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ticket status");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
        }

        /// <summary>
        /// Resolve Ticket
        /// POST: /Ticket/Resolve/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Resolve(int id, string? remarks)
        {
            try
            {
                SetAuthorizationHeader();

                var json = JsonSerializer.Serialize(remarks ?? "Ticket resolved");
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"api/Ticket/{id}/resolve", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ticket marked as resolved";
                }
                else
                {
                    TempData["Error"] = "Failed to resolve ticket";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving ticket");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// Close Ticket
        /// POST: /Ticket/Close/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Close(int id, string? remarks)
        {
            try
            {
                SetAuthorizationHeader();

                var json = JsonSerializer.Serialize(remarks ?? "Ticket closed");
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"api/Ticket/{id}/close", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ticket closed successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to close ticket";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing ticket");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// Reopen Ticket
        /// POST: /Ticket/Reopen/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Reopen(int id, string? remarks)
        {
            try
            {
                SetAuthorizationHeader();

                var json = JsonSerializer.Serialize(remarks ?? "Ticket reopened");
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"api/Ticket/{id}/reopen", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ticket reopened successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to reopen ticket";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reopening ticket");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// Mark In Progress
        /// POST: /Ticket/InProgress/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InProgress(int id)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _client.PutAsync($"api/Ticket/{id}/inprogress", null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ticket marked as In Progress";
                }
                else
                {
                    TempData["Error"] = "Failed to update status";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking ticket in progress");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// Mark as Blocked
        /// POST: /Ticket/Block/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Block(int id, string? remarks)
        {
            try
            {
                SetAuthorizationHeader();

                var json = JsonSerializer.Serialize(remarks ?? "Ticket blocked");
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"api/Ticket/{id}/block", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ticket marked as blocked";
                }
                else
                {
                    TempData["Error"] = "Failed to block ticket";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking ticket");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #endregion

        #region ASSIGN TICKET

        /// <summary>
        /// Assign Ticket to Developer
        /// POST: /Ticket/Assign
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Assign(int ticketId, int assignedTo, string? remarks)
        {
            try
            {
                SetAuthorizationHeader();

                var requestData = new
                {
                    ticketId,
                    assignedTo,
                    remarks
                };

                var json = JsonSerializer.Serialize(requestData);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync("api/Ticket/assign", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ticket assigned successfully";
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Failed to assign ticket: {content}";
                }

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning ticket");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
        }

        #endregion

        #region COMMENT OPERATIONS

        /// <summary>
        /// Add Comment to Ticket
        /// POST: /Ticket/AddComment
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId, string comment)
        {
            try
            {
                SetAuthorizationHeader();

                var requestData = new
                {
                    ticketId,
                    comment
                };

                var json = JsonSerializer.Serialize(requestData);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("api/Ticket/comment", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Comment added successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to add comment";
                }

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
        }

        #endregion

        #region ATTACHMENT OPERATIONS

        /// <summary>
        /// Upload Attachment
        /// POST: /Ticket/UploadAttachment
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UploadAttachment(int ticketId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["Error"] = "Please select a file to upload";
                    return RedirectToAction(nameof(Details), new { id = ticketId });
                }

                SetAuthorizationHeader();

                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(ticketId.ToString()), "ticketId");

                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.FileName);

                var response = await _client.PostAsync("api/Ticket/attachment/upload", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "File uploaded successfully";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Failed to upload file: {errorContent}";
                }

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading attachment");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
        }

        /// <summary>
        /// Download Attachment
        /// GET: /Ticket/DownloadAttachment/{attachmentId}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int attachmentId)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _client.GetAsync($"api/Ticket/attachment/{attachmentId}/download");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "File not found";
                    return RedirectToAction(nameof(MyTickets));
                }

                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "attachment";
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading attachment");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(MyTickets));
            }
        }

        /// <summary>
        /// Delete Attachment
        /// POST: /Ticket/DeleteAttachment/{attachmentId}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteAttachment(int attachmentId, int ticketId)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _client.DeleteAsync($"api/Ticket/attachment/{attachmentId}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Attachment deleted successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to delete attachment";
                }

                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting attachment");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }
        }

        #endregion

        #region DELETE TICKET

        /// <summary>
        /// Delete Ticket
        /// POST: /Ticket/Delete/{id}
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _client.DeleteAsync($"api/Ticket/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ticket deleted successfully";
                    return RedirectToAction(nameof(MyTickets));
                }
                else
                {
                    TempData["Error"] = "Failed to delete ticket";
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ticket");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #endregion
    }
}