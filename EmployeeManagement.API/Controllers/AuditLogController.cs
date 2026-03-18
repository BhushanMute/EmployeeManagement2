using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuditLogController> _logger;
        private readonly IConfiguration _configuration;

        public AuditLogController(
            IAuditLogService auditLogService,
            ILogger<AuditLogController> logger,
            IConfiguration configuration)
        {
            _auditLogService = auditLogService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponse<AuditLogResponse>>> GetLogs([FromBody] AuditLogRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                _logger.LogInformation("AuditLog search by User: {Username}, Roles: {Roles}",
                    username, string.Join(", ", roles));

                if (!roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogWarning("Access denied - User {Username} does not have Admin role", username);
                    return StatusCode(403, ApiResponse<AuditLogResponse>.Fail("Access denied. Admin role required."));
                }

                var result = await _auditLogService.GetLogsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetLogs");
                return StatusCode(500, ApiResponse<AuditLogResponse>.Fail("An error occurred"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AuditLog>>> GetById(int id)
        {
            try
            {
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                if (!roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                {
                    return StatusCode(403, ApiResponse<AuditLog>.Fail("Access denied."));
                }

                var result = await _auditLogService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit log {Id}", id);
                return StatusCode(500, ApiResponse<AuditLog>.Fail("An error occurred"));
            }
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            try
            {
                _logger.LogInformation("=== TEST ENDPOINT START ===");

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                using var command = new SqlCommand("SELECT COUNT(*) FROM AuditLogs", connection);
                var count = (int)await command.ExecuteScalarAsync();

                _logger.LogInformation("Direct SQL count: {Count}", count);

                var request = new AuditLogRequest
                {
                    PageNumber = 1,
                    PageSize = 50
                };

                var result = await _auditLogService.GetLogsAsync(request);

                _logger.LogInformation("Service result: Status={Status}, Count={Count}",
                    result.Status, result.Data?.Logs?.Count ?? 0);

                return Ok(new
                {
                    DirectSqlCount = count,
                    ServiceStatus = result.Status,
                    ServiceMessage = result.Message,
                    LogCount = result.Data?.Logs?.Count ?? 0,
                    TotalRecords = result.Data?.TotalRecords ?? 0,
                    SampleLog = result.Data?.Logs?.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test failed");
                return Ok(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }
    }
}