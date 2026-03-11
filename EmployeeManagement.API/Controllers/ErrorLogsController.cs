using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee.View")]
    [Authorize(Roles = "Employee.Update")]
    public class ErrorLogsController : ControllerBase
    {
        private readonly IErrorLogService _errorLogService;
        private readonly ILogger<ErrorLogsController> _logger;

        public ErrorLogsController(IErrorLogService errorLogService, ILogger<ErrorLogsController> logger)
        {
            _errorLogService = errorLogService;
            _logger = logger;
        }

        /// <summary>
        /// Get error logs with filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ErrorLog>>>> GetErrorLogs( [FromQuery] int top = 100, [FromQuery] bool? isResolved = null)
        {
            try
            {
                var logs = await _errorLogService.GetErrorLogsAsync(top, isResolved);
                return Ok(ApiResponse<List<ErrorLog>>.Success(logs, $"Retrieved {logs.Count} error logs"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving error logs");
                return StatusCode(500, ApiResponse<List<ErrorLog>>.Fail("Failed to retrieve error logs"));
            }
        }

        /// <summary>
        /// Get detailed error information
        /// </summary>
        [HttpGet("{errorId:guid}")]
        public async Task<ActionResult<ApiResponse<ErrorLog>>> GetErrorDetails(Guid errorId)
        {
            try
            {
                var errorLog = await _errorLogService.GetErrorDetailsAsync(errorId);

                if (errorLog == null)
                {
                    return NotFound(ApiResponse<ErrorLog>.Fail("Error log not found"));
                }

                return Ok(ApiResponse<ErrorLog>.Success(errorLog));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving error details for {ErrorId}", errorId);
                return StatusCode(500, ApiResponse<ErrorLog>.Fail("Failed to retrieve error details"));
            }
        }

        /// <summary>
        /// Mark error as resolved
        /// </summary>
        [HttpPut("{errorId:guid}/resolve")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAsResolved( Guid errorId, [FromBody] ResolveErrorRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(ApiResponse<bool>.Fail("Invalid user"));
                }

                var success = await _errorLogService.MarkAsResolvedAsync(errorId, userId, request.Notes);

                if (!success)
                {
                    return NotFound(ApiResponse<bool>.Fail("Error log not found"));
                }

                return Ok(ApiResponse<bool>.Success(true, "Error marked as resolved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking error as resolved");
                return StatusCode(500, ApiResponse<bool>.Fail("Failed to mark error as resolved"));
            }
        }

        /// <summary>
        /// Test endpoint to generate an error (for testing only)
        /// </summary>
        [HttpGet("test-error")]
        [AllowAnonymous]
        public IActionResult TestError()
        {
            throw new Exception("This is a test error for logging purposes");
        }
        [HttpGet("generate-error")]
        [AllowAnonymous]
        public IActionResult GenerateError([FromQuery] string type = "general")
        {
            switch (type.ToLower())
            {
                case "null":
                    // Null Reference Exception
                    string? nullString = null;
                    return Ok(nullString.Length);

                case "divide":
                    // Divide by Zero Exception
                    int zero = 0;
                    int result = 100 / zero;
                    return Ok(result);

                case "argument":
                    // Argument Exception
                    throw new ArgumentException("Invalid argument provided", nameof(type));

                case "notfound":
                    // Not Found Exception
                    throw new KeyNotFoundException("The requested resource was not found");

                case "unauthorized":
                    // Unauthorized Exception
                    throw new UnauthorizedAccessException("You do not have permission to access this resource");

                case "database":
                    // Simulate Database Exception
                    throw new InvalidOperationException("Database connection failed");

                case "validation":
                    // Validation Exception
                    throw new ValidationException("Validation failed for one or more fields");

                case "timeout":
                    // Timeout Exception
                    throw new TimeoutException("The operation has timed out");

                case "custom":
                    // Custom Exception
                    throw new CustomBusinessException("This is a custom business logic error");

                default:
                    // General Exception
                    throw new Exception("This is a general test error with some detailed information");
            }
        }
    }

    
}