using EmployeeManagement.API.Attributes;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveRepository _leaveRepo;
        private readonly IAuditService _auditService;
        private readonly ILogger<LeaveController> _logger;
        private readonly IEmailService _emailService;
        private readonly ICacheService _cacheService;

        public LeaveController(
            ILeaveRepository leaveRepo,
            IAuditService auditService,
            IEmailService emailService,
            ILogger<LeaveController> logger,
            ICacheService cacheService)
        {
            _leaveRepo = leaveRepo;
            _auditService = auditService;
            _emailService = emailService;
            _logger = logger;
            _cacheService = cacheService;
        }

        #region Helper Methods

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private decimal CalculateBusinessDays(DateTime startDate, DateTime endDate)
        {
            decimal businessDays = 0;
            var current = startDate;

            while (current <= endDate)
            {
                if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
                {
                    businessDays++;
                }
                current = current.AddDays(1);
            }

            return businessDays;
        }

        /// <summary>
        /// ✅ Clear all leave-related caches
        /// </summary>
        private void ClearLeaveCache(int? employeeId = null)
        {
            _logger.LogInformation("Clearing leave cache. EmployeeId: {EmployeeId}", employeeId ?? 0);

            // Employee-specific
            if (employeeId.HasValue)
            {
                _cacheService.Remove($"balance_{employeeId}_{DateTime.Now.Year}");
                _cacheService.Remove($"history_{employeeId}_{DateTime.Now.Year}");
            }

            // Shared caches
            _cacheService.Remove("leave_types");
            _cacheService.RemoveByPrefix("holidays_");
            _cacheService.RemoveByPrefix("pending_");
            _cacheService.RemoveByPrefix("calendar_");
            _cacheService.RemoveByPrefix("dashboard_");
            _cacheService.RemoveByPrefix("report_");
            _cacheService.RemoveByPrefix("all_balances_");
        }

        /// <summary>
        /// ✅ Send email in background (non-blocking)
        /// </summary>
        private void SendEmailInBackground(Func<Task> emailAction, string description)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await emailAction();
                    _logger.LogInformation("Email sent: {Description}", description);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send email: {Description}", description);
                }
            });
        }

        #endregion

        #region Leave Types

        /// <summary>
        /// Get all active leave types (Cached 30 min)
        /// </summary>
        [HttpGet("types")]
        [AllowAnonymous]

        public async Task<ActionResult<ApiResponse<List<LeaveType>>>> GetLeaveTypes()
        {
            try
            {
                var cacheKey = "leave_types";
                var cached = _cacheService.Get<List<LeaveType>>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<List<LeaveType>>.Success(cached, "Leave types retrieved (cached)"));

                var leaveTypes = await _leaveRepo.GetAllLeaveTypes();
                _cacheService.Set(cacheKey, leaveTypes, TimeSpan.FromMinutes(30));

                return Ok(ApiResponse<List<LeaveType>>.Success(leaveTypes, "Leave types retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving leave types");
                return StatusCode(500, ApiResponse<List<LeaveType>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Get leave type by ID
        /// </summary>
        [HttpGet("types/{id:int}")]
        public async Task<ActionResult<ApiResponse<LeaveType>>> GetLeaveTypeById(int id)
        {
            try
            {
                // ✅ Try to get from cached list first
                var cachedList = _cacheService.Get<List<LeaveType>>("leave_types");
                var leaveType = cachedList?.FirstOrDefault(lt => lt.Id == id);

                if (leaveType == null)
                    leaveType = await _leaveRepo.GetLeaveTypeById(id);

                if (leaveType == null)
                    return NotFound(ApiResponse<LeaveType>.Fail($"Leave type with ID {id} not found"));

                return Ok(ApiResponse<LeaveType>.Success(leaveType, "Leave type retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving leave type: {Id}", id);
                return StatusCode(500, ApiResponse<LeaveType>.Fail("An error occurred"));
            }
        }

        #endregion

        #region Apply Leave

        /// <summary>
        /// Apply for leave
        /// </summary>
        //[HttpPost("apply")]
        //public async Task<ActionResult<ApiResponse<LeaveRequest>>> ApplyLeave([FromBody] ApplyLeaveRequest request)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            var errors = ModelState.Values
        //                .SelectMany(v => v.Errors)
        //                .Select(e => e.ErrorMessage).ToList();
        //            return BadRequest(ApiResponse<LeaveRequest>.Fail("Validation failed", errors));
        //        }

        //        var currentUserId = GetCurrentUserId();

        //        if (request.StartDate > request.EndDate)
        //            return BadRequest(ApiResponse<LeaveRequest>.Fail("Start date cannot be after end date"));

        //        if (request.StartDate < DateTime.Today)
        //            return BadRequest(ApiResponse<LeaveRequest>.Fail("Cannot apply leave for past dates"));

        //        decimal totalDays = request.IsHalfDay ? 0.5m :
        //            CalculateBusinessDays(request.StartDate, request.EndDate);

        //        var leaveRequest = new LeaveRequest
        //        {
        //            EmployeeId = request.EmployeeId > 0 ? request.EmployeeId : currentUserId,
        //            LeaveTypeId = request.LeaveTypeId,
        //            StartDate = request.StartDate,
        //            EndDate = request.EndDate,
        //            TotalDays = totalDays,
        //            Reason = request.Reason,
        //            IsHalfDay = request.IsHalfDay,
        //            HalfDayType = request.HalfDayType,
        //            EmergencyContact = request.EmergencyContact,
        //            CreatedBy = currentUserId
        //        };

        //        var newId = await _leaveRepo.ApplyLeave(leaveRequest);
        //        leaveRequest.Id = newId;

        //        // ✅ Clear cache after applying
        //        ClearLeaveCache(leaveRequest.EmployeeId);

        //        // ✅ Audit (non-blocking)
        //        _ = _auditService.LogAsync(currentUserId, "Leave Applied", "LeaveRequests", newId);

        //        _logger.LogInformation("Leave applied: Employee {EmpId}, Request {ReqId}",
        //            leaveRequest.EmployeeId, newId);

        //        return CreatedAtAction(nameof(GetLeaveRequestById), new { id = newId },
        //            ApiResponse<LeaveRequest>.Success(leaveRequest, "Leave applied successfully"));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error applying leave");
        //        return StatusCode(500, ApiResponse<LeaveRequest>.Fail(ex.Message));
        //    }
        //}
        [HttpPost("apply")]
        public async Task<ActionResult<ApiResponse<LeaveRequest>>> ApplyLeave([FromBody] ApplyLeaveRequest request)
        {
            try
            {
                // 1. Validation
                if (!ModelState.IsValid) return BadRequest(ApiResponse<LeaveRequest>.Fail("Invalid Data"));

                var currentUserId = GetCurrentUserId(); // Helper method from your base/controller
                if (request.StartDate > request.EndDate)
                    return BadRequest(ApiResponse<LeaveRequest>.Fail("Start date cannot be after end date"));

                // 2. Calculate Days
                decimal totalDays = request.IsHalfDay ? 0.5m : CalculateBusinessDays(request.StartDate, request.EndDate);

                // 3. Prepare Entity
                var leaveRequest = new LeaveRequest
                {
                    EmployeeId = request.EmployeeId > 0 ? request.EmployeeId : currentUserId,
                    LeaveTypeId = request.LeaveTypeId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalDays = totalDays,
                    Reason = request.Reason,
                    IsHalfDay = request.IsHalfDay,
                    HalfDayType = request.HalfDayType,
                    EmergencyContact = request.EmergencyContact,
                    Status = "Pending", // Default status
                    CreatedBy = currentUserId
                };

                // 4. Call SP (Database Save)
                int newRequestId = await _leaveRepo.CreateLeaveRequestAsync(leaveRequest, currentUserId);
                leaveRequest.Id = newRequestId; // Update local object

                // 5. Fire & Forget Email (Non-blocking)
                // Isse user ko response jaldi milega, email background mein jayega.
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // Get Leave Type Name (You might need a quick lookup here or pass from request)
                        string leaveTypeName = "General"; // Ideally fetch from DB cache

                        // Send to Admin & HR (You should fetch these from DB or Config)
                        var adminHrEmails = await GetAdminAndHrEmailsAsync();

                        if (adminHrEmails.Any())
                        {
                            await _emailService.SendLeaveAppliedNotificationAsync(
                                recipientEmails: adminHrEmails,
                                employeeName: leaveRequest.EmployeeName ?? "Employee", // Ideally fetch from DB
                                leaveType: leaveTypeName,
                                startDate: leaveRequest.StartDate,
                                endDate: leaveRequest.EndDate,
                                totalDays: leaveRequest.TotalDays,
                                reason: leaveRequest.Reason,
                                requestId: newRequestId
                            );
                            _logger.LogInformation("Leave notification email sent for Request #{Id}", newRequestId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send leave notification email in background.");
                    }
                });

                // 6. Clear Cache & Audit
                ClearLeaveCache(leaveRequest.EmployeeId);
                _ = _auditService.LogAsync(currentUserId, "Leave Applied", "LeaveRequests", newRequestId);

                return CreatedAtAction(nameof(GetLeaveRequestById), new { id = newRequestId },
                    ApiResponse<LeaveRequest>.Success(leaveRequest, "Leave applied successfully. Notification sent to HR."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ApplyLeave");
                return StatusCode(500, ApiResponse<LeaveRequest>.Fail("Server error while applying leave."));
            }
        }

        // Helper to get Admin/HR emails (Mock implementation)
        private async Task<List<string>> GetAdminAndHrEmailsAsync()
        {
             
            return new List<string> { "jane@examples.com", "hr@company.com" };
        }
        // Helper to get Admin/HR emails (Mock implementation)
        //private async Task<List<string>> GetAdminAndHrEmailsAsync()
        //{
        //    // In production: Query Users table WHERE Role = 'Admin' OR Role = 'HR'
        //    // For now, returning hardcoded/config emails
        //    return new List<string> { "admin@company.com", "hr@company.com" };
        //}

        /// <summary>
        /// Apply leave with file attachment
        /// </summary>
        [HttpPost("apply-with-attachment")]
        public async Task<ActionResult<ApiResponse<LeaveRequest>>> ApplyLeaveWithAttachment(
            [FromForm] ApplyLeaveRequest request, IFormFile? attachment)
        {
            try
            {
                string? attachmentPath = null;

                if (attachment != null && attachment.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
                    var ext = Path.GetExtension(attachment.FileName).ToLower();

                    if (!allowedExtensions.Contains(ext))
                        return BadRequest(ApiResponse<LeaveRequest>.Fail("Invalid file type. Allowed: jpg, png, pdf, doc"));

                    if (attachment.Length > 5 * 1024 * 1024)
                        return BadRequest(ApiResponse<LeaveRequest>.Fail("File size exceeds 5MB limit"));

                    var fileName = $"leave_{request.EmployeeId}_{Guid.NewGuid()}{ext}";
                    var uploadPath = Path.Combine("wwwroot", "uploads", "leave-attachments");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);

                    // ✅ Use async file write
                    await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await attachment.CopyToAsync(stream);
                    }

                    attachmentPath = $"/uploads/leave-attachments/{fileName}";
                }

                var currentUserId = GetCurrentUserId();
                decimal totalDays = request.IsHalfDay ? 0.5m :
                    CalculateBusinessDays(request.StartDate, request.EndDate);

                var leaveRequest = new LeaveRequest
                {
                    EmployeeId = request.EmployeeId > 0 ? request.EmployeeId : currentUserId,
                    LeaveTypeId = request.LeaveTypeId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalDays = totalDays,
                    Reason = request.Reason,
                    IsHalfDay = request.IsHalfDay,
                    HalfDayType = request.HalfDayType,
                    EmergencyContact = request.EmergencyContact,
                    AttachmentPath = attachmentPath,
                    CreatedBy = currentUserId
                };

                var newId = await _leaveRepo.ApplyLeave(leaveRequest);
                leaveRequest.Id = newId;

                // ✅ Clear cache
                ClearLeaveCache(leaveRequest.EmployeeId);

                _ = _auditService.LogAsync(currentUserId, "Leave Applied with Attachment", "LeaveRequests", newId);

                return CreatedAtAction(nameof(GetLeaveRequestById), new { id = newId },
                    ApiResponse<LeaveRequest>.Success(leaveRequest, "Leave applied successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying leave with attachment");
                return StatusCode(500, ApiResponse<LeaveRequest>.Fail(ex.Message));
            }
        }

        #endregion

        #region View Leave

        /// <summary>
        /// Get leave request by ID
        /// </summary>
        [HttpGet("request/{id:int}")]
        public async Task<ActionResult<ApiResponse<LeaveRequest>>> GetLeaveRequestById(int id)
        {
            try
            {
                var request = await _leaveRepo.GetLeaveRequestById(id);
                if (request == null)
                    return NotFound(ApiResponse<LeaveRequest>.Fail($"Leave request with ID {id} not found"));

                return Ok(ApiResponse<LeaveRequest>.Success(request, "Leave request retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving leave request: {Id}", id);
                return StatusCode(500, ApiResponse<LeaveRequest>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Get employee leave history (Cached 1 min)
        /// </summary>
        [HttpGet("history/{employeeId:int}")]
        public async Task<ActionResult<ApiResponse<List<LeaveRequest>>>> GetLeaveHistory(
            int employeeId, [FromQuery] int? year = null)
        {
            try
            {
                var yr = year ?? DateTime.Now.Year;
                var cacheKey = $"history_{employeeId}_{yr}";
                var cached = _cacheService.Get<List<LeaveRequest>>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<List<LeaveRequest>>.Success(cached, $"Found {cached.Count} records (cached)"));

                var history = await _leaveRepo.GetEmployeeLeaveHistory(employeeId, yr);
                _cacheService.Set(cacheKey, history, TimeSpan.FromMinutes(1));

                return Ok(ApiResponse<List<LeaveRequest>>.Success(history, $"Found {history.Count} leave records"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving leave history: {EmployeeId}", employeeId);
                return StatusCode(500, ApiResponse<List<LeaveRequest>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Get pending leave requests (Cached 30 sec)
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<List<LeaveRequest>>>> GetPendingRequests(
            [FromQuery] int? departmentId = null)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var cacheKey = $"pending_{currentUserId}_{departmentId}";
                var cached = _cacheService.Get<List<LeaveRequest>>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<List<LeaveRequest>>.Success(cached, $"Found {cached.Count} pending (cached)"));

                var requests = await _leaveRepo.GetPendingLeaveRequests(currentUserId, departmentId);
                _cacheService.Set(cacheKey, requests, TimeSpan.FromSeconds(30));

                return Ok(ApiResponse<List<LeaveRequest>>.Success(requests, $"Found {requests.Count} pending requests"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending leave requests");
                return StatusCode(500, ApiResponse<List<LeaveRequest>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Get all leave requests with filters
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<PagedResult<LeaveRequest>>>> GetAllLeaveRequests(
            [FromQuery] string? status = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? leaveTypeId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _leaveRepo.GetAllLeaveRequests(
                    status, departmentId, leaveTypeId, startDate, endDate, pageNumber, pageSize);

                return Ok(ApiResponse<PagedResult<LeaveRequest>>.Success(result, "Leave requests retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all leave requests");
                return StatusCode(500, ApiResponse<PagedResult<LeaveRequest>>.Fail("An error occurred"));
            }
        }

        #endregion

        #region Approve / Reject / Cancel

        /// <summary>
        /// ✅ FIXED: Approve leave — was sending both approve AND reject emails before
        /// </summary>
        [HttpPost("approve")]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveLeave([FromBody] LeaveActionRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // ✅ Get leave details ONCE (was calling twice before)
                var leaveRequest = await _leaveRepo.GetLeaveRequestById(request.LeaveRequestId);
                if (leaveRequest == null)
                    return NotFound(ApiResponse<bool>.Fail("Leave request not found"));

                // Approve in DB
                await _leaveRepo.ApproveLeave(request.LeaveRequestId, currentUserId, request.Remarks);

                // ✅ Clear cache for the employee whose leave was approved
                ClearLeaveCache(leaveRequest.EmployeeId);

                // ✅ Audit (non-blocking)
                _ = _auditService.LogAsync(currentUserId, "Leave Approved", "LeaveRequests", request.LeaveRequestId);

                // ✅ FIXED: Send ONLY approval email (was sending both approve + reject before!)
                SendEmailInBackground(async () =>
                {
                    await _emailService.SendLeaveApprovedNotification(
                        leaveRequest.EmployeeEmail ?? "",
                        leaveRequest.EmployeeName ?? "",
                        leaveRequest.LeaveTypeName ?? "",
                        leaveRequest.StartDate,
                        leaveRequest.EndDate,
                        leaveRequest.TotalDays,
                        request.Remarks);
                }, $"Leave Approved for {leaveRequest.EmployeeName}");

                _logger.LogInformation("Leave {Id} approved by User {UserId}", request.LeaveRequestId, currentUserId);

                return Ok(ApiResponse<bool>.Success(true, "Leave approved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave: {Id}", request.LeaveRequestId);
                return StatusCode(500, ApiResponse<bool>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// ✅ FIXED: Reject leave — sends rejection email in background
        /// </summary>
        [HttpPost("reject")]
        public async Task<ActionResult<ApiResponse<bool>>> RejectLeave([FromBody] LeaveActionRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // ✅ Get leave details ONCE
                var leaveRequest = await _leaveRepo.GetLeaveRequestById(request.LeaveRequestId);
                if (leaveRequest == null)
                    return NotFound(ApiResponse<bool>.Fail("Leave request not found"));

                // Reject in DB
                await _leaveRepo.RejectLeave(request.LeaveRequestId, currentUserId, request.Remarks);

                // ✅ Clear cache
                ClearLeaveCache(leaveRequest.EmployeeId);

                // ✅ Audit
                _ = _auditService.LogAsync(currentUserId, "Leave Rejected", "LeaveRequests", request.LeaveRequestId);

                // ✅ Send rejection email in background
                SendEmailInBackground(async () =>
                {
                    await _emailService.SendLeaveRejectedNotification(
                        leaveRequest.EmployeeEmail ?? "",
                        leaveRequest.EmployeeName ?? "",
                        leaveRequest.LeaveTypeName ?? "",
                        leaveRequest.StartDate,
                        leaveRequest.EndDate,
                        request.Remarks);
                }, $"Leave Rejected for {leaveRequest.EmployeeName}");

                _logger.LogInformation("Leave {Id} rejected by User {UserId}", request.LeaveRequestId, currentUserId);

                return Ok(ApiResponse<bool>.Success(true, "Leave rejected successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting leave: {Id}", request.LeaveRequestId);
                return StatusCode(500, ApiResponse<bool>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Cancel leave request
        /// </summary>
        [HttpPost("cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelLeave([FromBody] CancelLeaveRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                await _leaveRepo.CancelLeave(request.LeaveRequestId, currentUserId, request.CancelReason);

                // ✅ Clear cache
                ClearLeaveCache(currentUserId);

                _ = _auditService.LogAsync(currentUserId, "Leave Cancelled", "LeaveRequests", request.LeaveRequestId);

                _logger.LogInformation("Leave {Id} cancelled by User {UserId}", request.LeaveRequestId, currentUserId);

                return Ok(ApiResponse<bool>.Success(true, "Leave cancelled successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling leave: {Id}", request.LeaveRequestId);
                return StatusCode(500, ApiResponse<bool>.Fail(ex.Message));
            }
        }

        #endregion

        #region Leave Balance

        /// <summary>
        /// Get employee leave balance (Cached 2 min)
        /// </summary>
        [HttpGet("balance/{employeeId:int}")]
        public async Task<ActionResult<ApiResponse<List<LeaveBalance>>>> GetLeaveBalance(
            int employeeId, [FromQuery] int? year = null)
        {
            try
            {
                var yr = year ?? DateTime.Now.Year;
                var cacheKey = $"balance_{employeeId}_{yr}";
                var cached = _cacheService.Get<List<LeaveBalance>>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<List<LeaveBalance>>.Success(cached, "Balance retrieved (cached)"));

                var balances = await _leaveRepo.GetLeaveBalance(employeeId, yr);
                _cacheService.Set(cacheKey, balances, TimeSpan.FromMinutes(2));

                return Ok(ApiResponse<List<LeaveBalance>>.Success(balances, "Balance retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving balance for employee: {EmployeeId}", employeeId);
                return StatusCode(500, ApiResponse<List<LeaveBalance>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Get all employee balances (Cached 2 min)
        /// </summary>
        [HttpGet("balance/all")]
        public async Task<ActionResult<ApiResponse<List<LeaveBalance>>>> GetAllBalances(
            [FromQuery] int? year = null)
        {
            try
            {
                var yr = year ?? DateTime.Now.Year;
                var cacheKey = $"all_balances_{yr}";
                var cached = _cacheService.Get<List<LeaveBalance>>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<List<LeaveBalance>>.Success(cached, $"Retrieved {cached.Count} records (cached)"));

                var balances = await _leaveRepo.GetAllEmployeeBalances(yr);
                _cacheService.Set(cacheKey, balances, TimeSpan.FromMinutes(2));

                return Ok(ApiResponse<List<LeaveBalance>>.Success(balances, $"Retrieved {balances.Count} balance records"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all balances");
                return StatusCode(500, ApiResponse<List<LeaveBalance>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Allocate leave balance
        /// </summary>
        [HttpPost("balance/allocate")]
        [AuthorizePermission("Employee.View")]
        public async Task<ActionResult<ApiResponse<bool>>> AllocateLeaveBalance([FromBody] AllocateLeaveRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                await _leaveRepo.AllocateLeaveBalance(
                    request.EmployeeId, request.LeaveTypeId, request.Year,
                    request.TotalAllocated, request.CarryForward, currentUserId);

                // ✅ Clear cache
                ClearLeaveCache(request.EmployeeId);

                _ = _auditService.LogAsync(currentUserId, "Leave Balance Allocated", "LeaveBalances");

                return Ok(ApiResponse<bool>.Success(true, "Leave balance allocated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating leave balance");
                return StatusCode(500, ApiResponse<bool>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Allocate default leave for all employees
        /// </summary>
        [HttpPost("balance/allocate-all")]
        [AuthorizePermission("Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> AllocateDefaultForAll([FromQuery] int year)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                await _leaveRepo.AllocateDefaultLeaveForAllEmployees(year, currentUserId);

                // ✅ Clear ALL balance caches
                _cacheService.RemoveByPrefix("balance_");
                _cacheService.RemoveByPrefix("all_balances_");

                _ = _auditService.LogAsync(currentUserId, $"Default Leave Allocated for Year {year}", "LeaveBalances");

                return Ok(ApiResponse<bool>.Success(true, $"Default leave allocated for year {year}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating default leave for year: {Year}", year);
                return StatusCode(500, ApiResponse<bool>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Allocate fixed leaves to ALL employees
        /// </summary>
        [HttpPost("balance/allocate-fixed-all")]
        public async Task<ActionResult<ApiResponse<object>>> AllocateFixedForAll(
            [FromBody] AllocateFixedLeaveRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var count = await _leaveRepo.AllocateFixedLeaveForAllEmployees(
                    request.Year, request.LeavesPerType, currentUserId);

                // ✅ Clear ALL balance caches
                _cacheService.RemoveByPrefix("balance_");
                _cacheService.RemoveByPrefix("all_balances_");

                _ = _auditService.LogAsync(currentUserId,
                    $"Allocated {request.LeavesPerType} leaves/type for all - Year {request.Year}",
                    "LeaveBalances");

                return Ok(ApiResponse<object>.Success(
                    new { RecordsInserted = count },
                    $"Allocated {request.LeavesPerType} leaves/type. {count} records created."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating fixed leave for all");
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Allocate leaves to a single employee
        /// </summary>
        [HttpPost("balance/allocate-single")]
        public async Task<ActionResult<ApiResponse<bool>>> AllocateForSingle(
            [FromBody] AllocateSingleEmployeeRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                await _leaveRepo.AllocateLeaveForSingleEmployee(
                    request.EmployeeId, request.Year, request.LeavesPerType, currentUserId);

                // ✅ Clear cache for this employee
                _cacheService.Remove($"balance_{request.EmployeeId}_{request.Year}");
                _cacheService.Remove($"all_balances_{request.Year}");

                _ = _auditService.LogAsync(currentUserId,
                    $"Allocated {request.LeavesPerType} leaves for Employee {request.EmployeeId}",
                    "LeaveBalances");

                return Ok(ApiResponse<bool>.Success(true, "Leave allocated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating leave for employee");
                return StatusCode(500, ApiResponse<bool>.Fail(ex.Message));
            }
        }

        #endregion

        #region Holidays

        /// <summary>
        /// Get holidays list (Cached 1 hour)
        /// </summary>
        [HttpGet("holidays")]
        [OutputCache(PolicyName = "Holidays")]
        public async Task<ActionResult<ApiResponse<List<Holiday>>>> GetHolidays([FromQuery] int? year = null)
        {
            try
            {
                var yr = year ?? DateTime.Now.Year;
                var cacheKey = $"holidays_{yr}";
                var cached = _cacheService.Get<List<Holiday>>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<List<Holiday>>.Success(cached, "Holidays retrieved (cached)"));

                var holidays = await _leaveRepo.GetHolidays(yr);
                _cacheService.Set(cacheKey, holidays, TimeSpan.FromHours(1));

                return Ok(ApiResponse<List<Holiday>>.Success(holidays, $"Found {holidays.Count} holidays"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving holidays");
                return StatusCode(500, ApiResponse<List<Holiday>>.Fail("An error occurred"));
            }
        }

        [HttpGet("holidays/{id:int}")]
        public async Task<ActionResult<ApiResponse<Holiday>>> GetHolidayById(int id)
        {
            try
            {
                var holiday = await _leaveRepo.GetHolidayById(id);
                if (holiday == null)
                    return NotFound(ApiResponse<Holiday>.Fail($"Holiday with ID {id} not found"));

                return Ok(ApiResponse<Holiday>.Success(holiday, "Holiday retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving holiday: {Id}", id);
                return StatusCode(500, ApiResponse<Holiday>.Fail("An error occurred"));
            }
        }

        [HttpGet("holidays/stats")]
        public async Task<ActionResult<ApiResponse<HolidayStats>>> GetHolidayStats([FromQuery] int? year = null)
        {
            try
            {
                var yr = year ?? DateTime.Now.Year;
                var cacheKey = $"holiday_stats_{yr}";
                var cached = _cacheService.Get<HolidayStats>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<HolidayStats>.Success(cached, "Holiday stats (cached)"));

                var stats = await _leaveRepo.GetHolidaysCount(yr);
                _cacheService.Set(cacheKey, stats, TimeSpan.FromHours(1));

                return Ok(ApiResponse<HolidayStats>.Success(stats, "Holiday stats retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving holiday stats");
                return StatusCode(500, ApiResponse<HolidayStats>.Fail("An error occurred"));
            }
        }

        [HttpPost("holidays")]
        public async Task<ActionResult<ApiResponse<Holiday>>> AddHoliday([FromBody] CreateHolidayRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<Holiday>.Fail("Validation failed", errors));
                }

                var currentUserId = GetCurrentUserId();

                var holiday = new Holiday
                {
                    Name = request.Name,
                    Date = request.Date,
                    Type = request.Type,
                    Description = request.Description,
                    CreatedBy = currentUserId
                };

                var newId = await _leaveRepo.AddHoliday(holiday);
                holiday.Id = newId;
                holiday.Day = request.Date.DayOfWeek.ToString();
                holiday.Year = request.Date.Year;

                // ✅ Clear holiday cache
                _cacheService.RemoveByPrefix("holidays_");
                _cacheService.RemoveByPrefix("holiday_stats_");

                _ = _auditService.LogAsync(currentUserId, "Holiday Created", "Holidays", newId);

                return CreatedAtAction(nameof(GetHolidayById), new { id = newId },
                    ApiResponse<Holiday>.Success(holiday, "Holiday added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding holiday");
                return StatusCode(500, ApiResponse<Holiday>.Fail(ex.Message));
            }
        }

        [HttpPut("holidays/{id:int}")]
        public async Task<ActionResult<ApiResponse<Holiday>>> UpdateHoliday(int id, [FromBody] UpdateHolidayRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<Holiday>.Fail("Validation failed", errors));
                }

                var currentUserId = GetCurrentUserId();
                var existing = await _leaveRepo.GetHolidayById(id);

                if (existing == null)
                    return NotFound(ApiResponse<Holiday>.Fail($"Holiday with ID {id} not found"));

                existing.Name = request.Name;
                existing.Date = request.Date;
                existing.Day = request.Date.DayOfWeek.ToString();
                existing.Type = request.Type;
                existing.Description = request.Description;
                existing.IsActive = request.IsActive;
                existing.Year = request.Date.Year;
                existing.UpdatedBy = currentUserId;

                await _leaveRepo.UpdateHoliday(existing);

                // ✅ Clear holiday cache
                _cacheService.RemoveByPrefix("holidays_");
                _cacheService.RemoveByPrefix("holiday_stats_");

                _ = _auditService.LogAsync(currentUserId, "Holiday Updated", "Holidays", id);

                return Ok(ApiResponse<Holiday>.Success(existing, "Holiday updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating holiday: {Id}", id);
                return StatusCode(500, ApiResponse<Holiday>.Fail(ex.Message));
            }
        }

        [HttpDelete("holidays/{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteHoliday(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var holiday = await _leaveRepo.GetHolidayById(id);

                if (holiday == null)
                    return NotFound(ApiResponse<bool>.Fail($"Holiday with ID {id} not found"));

                await _leaveRepo.DeleteHoliday(id, currentUserId);

                // ✅ Clear holiday cache
                _cacheService.RemoveByPrefix("holidays_");
                _cacheService.RemoveByPrefix("holiday_stats_");

                _ = _auditService.LogAsync(currentUserId, $"Holiday Deleted: {holiday.Name}", "Holidays", id);

                return Ok(ApiResponse<bool>.Success(true, "Holiday deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting holiday: {Id}", id);
                return StatusCode(500, ApiResponse<bool>.Fail(ex.Message));
            }
        }

        #endregion

        #region Reports & Export

        /// <summary>
        /// Employee Leave Report (Cached 5 min)
        /// </summary>
        [HttpGet("reports/employee")]
        public async Task<ActionResult<ApiResponse<List<EmployeeLeaveReportItem>>>> GetEmployeeLeaveReport(
            [FromQuery] int? year = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? employeeId = null)
        {
            try
            {
                var yr = year ?? DateTime.Now.Year;
                var cacheKey = $"report_emp_{yr}_{departmentId}_{employeeId}";
                var cached = _cacheService.Get<List<EmployeeLeaveReportItem>>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<List<EmployeeLeaveReportItem>>.Success(cached, "Report (cached)"));

                var report = await _leaveRepo.GetEmployeeLeaveReport(yr, departmentId, employeeId);
                _cacheService.Set(cacheKey, report, TimeSpan.FromMinutes(5));

                return Ok(ApiResponse<List<EmployeeLeaveReportItem>>.Success(report, "Report generated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating employee leave report");
                return StatusCode(500, ApiResponse<List<EmployeeLeaveReportItem>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Department Leave Report (Cached 5 min)
        /// </summary>
        [HttpGet("reports/department")]
        public async Task<ActionResult<ApiResponse<List<DepartmentLeaveReportItem>>>> GetDepartmentLeaveReport(
            [FromQuery] int? year = null)
        {
            try
            {
                var yr = year ?? DateTime.Now.Year;
                var cacheKey = $"report_dept_{yr}";
                var cached = _cacheService.Get<List<DepartmentLeaveReportItem>>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<List<DepartmentLeaveReportItem>>.Success(cached, "Report (cached)"));

                var report = await _leaveRepo.GetDepartmentLeaveReport(yr);
                _cacheService.Set(cacheKey, report, TimeSpan.FromMinutes(5));

                return Ok(ApiResponse<List<DepartmentLeaveReportItem>>.Success(report, "Report generated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating department leave report");
                return StatusCode(500, ApiResponse<List<DepartmentLeaveReportItem>>.Fail("An error occurred"));
            }
        }

        [HttpGet("reports/monthly")]
        public async Task<ActionResult<ApiResponse<List<MonthlyLeaveReportItem>>>> GetMonthlyReport(
            [FromQuery] int month, [FromQuery] int year, [FromQuery] int? departmentId = null)
        {
            try
            {
                var report = await _leaveRepo.GetMonthlyLeaveReport(month, year, departmentId);
                return Ok(ApiResponse<List<MonthlyLeaveReportItem>>.Success(report, "Monthly report generated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating monthly report");
                return StatusCode(500, ApiResponse<List<MonthlyLeaveReportItem>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Leave Calendar Data (Cached 2 min)
        /// </summary>
        [HttpGet("calendar")]
        public async Task<ActionResult<ApiResponse<List<LeaveCalendarItem>>>> GetLeaveCalendar(
            [FromQuery] int month, [FromQuery] int year, [FromQuery] int? departmentId = null)
        {
            try
            {
                var cacheKey = $"calendar_{month}_{year}_{departmentId}";
                var cached = _cacheService.Get<List<LeaveCalendarItem>>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<List<LeaveCalendarItem>>.Success(cached, "Calendar data (cached)"));

                var data = await _leaveRepo.GetLeaveCalendarData(month, year, departmentId);
                _cacheService.Set(cacheKey, data, TimeSpan.FromMinutes(2));

                return Ok(ApiResponse<List<LeaveCalendarItem>>.Success(data, "Calendar data retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calendar data");
                return StatusCode(500, ApiResponse<List<LeaveCalendarItem>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Dashboard Stats (Cached 1 min)
        /// </summary>
        [HttpGet("reports/dashboard-stats")]
        public async Task<ActionResult<ApiResponse<LeaveDashboardStats>>> GetDashboardStats(
            [FromQuery] int? year = null)
        {
            try
            {
                var yr = year ?? DateTime.Now.Year;
                var cacheKey = $"dashboard_stats_{yr}";
                var cached = _cacheService.Get<LeaveDashboardStats>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<LeaveDashboardStats>.Success(cached, "Stats (cached)"));

                var stats = await _leaveRepo.GetLeaveDashboardStats(yr);
                _cacheService.Set(cacheKey, stats, TimeSpan.FromMinutes(1));

                return Ok(ApiResponse<LeaveDashboardStats>.Success(stats, "Stats retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return StatusCode(500, ApiResponse<LeaveDashboardStats>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Admin Dashboard (Cached 1 min)
        /// </summary>
        [HttpGet("admin-dashboard")]
        public async Task<ActionResult<ApiResponse<AdminLeaveDashboard>>> GetAdminDashboard(
            [FromQuery] int? year = null)
        {
            try
            {
                var yr = year ?? DateTime.Now.Year;
                var cacheKey = $"admin_dashboard_{yr}";
                var cached = _cacheService.Get<AdminLeaveDashboard>(cacheKey);

                if (cached != null)
                    return Ok(ApiResponse<AdminLeaveDashboard>.Success(cached, "Dashboard (cached)"));

                var dashboard = await _leaveRepo.GetAdminLeaveDashboard(yr);
                _cacheService.Set(cacheKey, dashboard, TimeSpan.FromMinutes(1));

                return Ok(ApiResponse<AdminLeaveDashboard>.Success(dashboard, "Dashboard loaded"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                return StatusCode(500, ApiResponse<AdminLeaveDashboard>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Export to Excel
        /// </summary>
        [HttpGet("reports/export/excel")]
        public async Task<IActionResult> ExportEmployeeReportExcel(
            [FromQuery] int? year = null, [FromQuery] int? departmentId = null)
        {
            try
            {
                var report = await _leaveRepo.GetEmployeeLeaveReport(year, departmentId, null);

                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var ws = workbook.Worksheets.Add("Leave Report");

                ws.Cell(1, 1).Value = $"Employee Leave Report - {year ?? DateTime.Now.Year}";
                ws.Range(1, 1, 1, 11).Merge().Style.Font.Bold = true;
                ws.Range(1, 1, 1, 11).Style.Font.FontSize = 14;

                var headers = new[] { "Employee", "Email", "Department", "Leave Type", "Code",
                    "Allocated", "Used", "Pending", "Carry Forward", "Available", "Approved Count" };

                for (int i = 0; i < headers.Length; i++)
                    ws.Cell(3, i + 1).Value = headers[i];

                var headerRange = ws.Range(3, 1, 3, headers.Length);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.CornflowerBlue;
                headerRange.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

                var row = 4;
                foreach (var item in report)
                {
                    ws.Cell(row, 1).Value = item.EmployeeName;
                    ws.Cell(row, 2).Value = item.EmployeeEmail;
                    ws.Cell(row, 3).Value = item.DepartmentName;
                    ws.Cell(row, 4).Value = item.LeaveTypeName;
                    ws.Cell(row, 5).Value = item.LeaveTypeCode;
                    ws.Cell(row, 6).Value = item.TotalAllocated;
                    ws.Cell(row, 7).Value = item.TotalUsed;
                    ws.Cell(row, 8).Value = item.TotalPending;
                    ws.Cell(row, 9).Value = item.CarryForward;
                    ws.Cell(row, 10).Value = item.TotalAvailable;
                    ws.Cell(row, 11).Value = item.ApprovedCount;

                    if (item.TotalAvailable <= 0)
                        ws.Range(row, 1, row, 11).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightPink;

                    row++;
                }

                ws.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);

                _ = _auditService.LogAsync(GetCurrentUserId(), "Leave Report Exported (Excel)", "Reports");

                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"LeaveReport_{year ?? DateTime.Now.Year}_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to Excel");
                return StatusCode(500, "Error exporting report");
            }
        }

        /// <summary>
        /// Export to CSV
        /// </summary>
        [HttpGet("reports/export/csv")]
        public async Task<IActionResult> ExportEmployeeReportCsv(
            [FromQuery] int? year = null, [FromQuery] int? departmentId = null)
        {
            try
            {
                var report = await _leaveRepo.GetEmployeeLeaveReport(year, departmentId, null);

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Employee,Email,Department,Leave Type,Code,Allocated,Used,Pending,CarryForward,Available,ApprovedCount");

                foreach (var item in report)
                {
                    sb.AppendLine($"\"{item.EmployeeName}\",\"{item.EmployeeEmail}\",\"{item.DepartmentName}\"," +
                        $"\"{item.LeaveTypeName}\",\"{item.LeaveTypeCode}\",{item.TotalAllocated},{item.TotalUsed}," +
                        $"{item.TotalPending},{item.CarryForward},{item.TotalAvailable},{item.ApprovedCount}");
                }

                _ = _auditService.LogAsync(GetCurrentUserId(), "Leave Report Exported (CSV)", "Reports");

                return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/csv",
                    $"LeaveReport_{year ?? DateTime.Now.Year}_{DateTime.Now:yyyyMMddHHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to CSV");
                return StatusCode(500, "Error exporting report");
            }
        }
        /// <summary>
        /// Department Leave Report
        /// </summary>
       
        #endregion
    }
}