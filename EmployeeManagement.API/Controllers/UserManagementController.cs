using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementRepository _userMgmtRepo;
        private readonly IAuditService _auditService;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IUserManagementRepository userMgmtRepo,
            IAuditService auditService,
            ILogger<UserManagementController> logger)
        {
            _userMgmtRepo = userMgmtRepo;
            _auditService = auditService;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }

        /// <summary>
        /// Get pending users (waiting for approval)
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<List<PendingUser>>>> GetPendingUsers()
        {
            try
            {
                var users = await _userMgmtRepo.GetPendingUsers();
                return Ok(ApiResponse<List<PendingUser>>.Success(users, $"Found {users.Count} pending users"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending users");
                return StatusCode(500, ApiResponse<List<PendingUser>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Get all users with roles
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<List<PendingUser>>>> GetAllUsers()
        {
            try
            {
                var users = await _userMgmtRepo.GetAllUsersWithRoles();
                return Ok(ApiResponse<List<PendingUser>>.Success(users, $"Found {users.Count} users"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, ApiResponse<List<PendingUser>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        [HttpGet("roles")]
        public async Task<ActionResult<ApiResponse<List<RoleInfo>>>> GetRoles()
        {
            try
            {
                var roles = await _userMgmtRepo.GetAllRoles();
                return Ok(ApiResponse<List<RoleInfo>>.Success(roles, "Roles retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles");
                return StatusCode(500, ApiResponse<List<RoleInfo>>.Fail("An error occurred"));
            }
        }

        /// <summary>
        /// Approve user and assign role
        /// </summary>
        [HttpPost("approve")]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveUser([FromBody] ApproveUserRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                await _userMgmtRepo.ApproveUserAndAssignRole(
                    request.UserId, request.RoleId, request.DepartmentId, currentUserId);

                _ = _auditService.LogAsync(currentUserId,
                    $"User {request.UserId} approved with Role {request.RoleId}",
                    "Users", request.UserId);

                _logger.LogInformation("User {UserId} approved by {ApprovedBy} with Role {RoleId}",
                    request.UserId, currentUserId, request.RoleId);

                return Ok(ApiResponse<bool>.Success(true, "User approved and role assigned successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving user: {UserId}", request.UserId);
                return StatusCode(500, ApiResponse<bool>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Reject user registration
        /// </summary>
        [HttpPost("reject")]
        public async Task<ActionResult<ApiResponse<bool>>> RejectUser([FromBody] RejectUserRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                await _userMgmtRepo.RejectUser(request.UserId, currentUserId, request.RejectionReason);

                _ = _auditService.LogAsync(currentUserId,
                    $"User {request.UserId} rejected: {request.RejectionReason}",
                    "Users", request.UserId);

                return Ok(ApiResponse<bool>.Success(true, "User registration rejected"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user: {UserId}", request.UserId);
                return StatusCode(500, ApiResponse<bool>.Fail(ex.Message));
            }
        }
       
    }
}