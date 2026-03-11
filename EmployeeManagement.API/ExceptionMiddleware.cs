using EmployeeManagement.API.Common;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using System.Net;
using System.Text.Json;

namespace EmployeeManagement.API
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IErrorLogService errorLogService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex, errorLogService);
            }
        }

        private async Task HandleExceptionAsync( HttpContext context, Exception exception, IErrorLogService errorLogService)
        {
            try
            {
                // Get user information if available
                var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var username = context.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

                // Log error to database and file
                var errorId = await errorLogService.LogErrorAsync(
                    exception,
                    context,
                    userId != null ? int.Parse(userId) : null,
                    username
                );

                // Prepare response
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new ApiResponse<object>
                {
                    Status = false,
                    Message = "An internal server error occurred",
                    Timestamp = DateTime.UtcNow,
                    Errors = new List<string>()
                };

                // Add error reference ID
                response.Errors.Add($"Error Reference ID: {errorId}");

#if DEBUG
                // In development, include detailed error information
                response.Errors.Add($"Exception: {exception.Message}");

                if (exception.InnerException != null)
                {
                    response.Errors.Add($"Inner Exception: {exception.InnerException.Message}");
                }

                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    response.Errors.Add($"Stack Trace: {exception.StackTrace}");
                }
#else
                // In production, only show generic message
                response.Errors.Add("Please contact support with the error reference ID.");
#endif

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
            catch (Exception loggingEx)
            {
                // If logging fails, at least log to console
                _logger.LogError(loggingEx, "Failed to log error");

                // Still return a response
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("{\"status\":false,\"message\":\"An error occurred\"}");
            }
        }
    }
}