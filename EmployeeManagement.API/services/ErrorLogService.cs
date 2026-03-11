using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace EmployeeManagement.API.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<ErrorLogService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);

        public ErrorLogService( IDbConnectionFactory connectionFactory, ILogger<ErrorLogService> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
        }

        public async Task<Guid> LogErrorAsync( Exception exception, HttpContext httpContext, int? userId = null, string? username = null)
        {
            var errorId = Guid.NewGuid();

            try
            {
                // Get request body
                string? requestBody = null;
                if (httpContext.Request.Body.CanSeek)
                {
                    httpContext.Request.Body.Position = 0;
                    using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
                    requestBody = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;
                }

                var errorLog = new ErrorLog
                {
                    ErrorId = errorId,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    InnerException = exception.InnerException?.ToString(),
                    ExceptionType = exception.GetType().FullName,
                    Source = exception.Source,
                    RequestMethod = httpContext.Request.Method,
                    RequestPath = httpContext.Request.Path,
                    QueryString = httpContext.Request.QueryString.ToString(),
                    RequestBody = requestBody,
                    UserId = userId,
                    Username = username ?? httpContext.User?.Identity?.Name,
                    IpAddress = GetIpAddress(httpContext),
                    UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                    StatusCode = httpContext.Response.StatusCode,
                    ServerName = System.Environment.MachineName,
                    Environment = _environment.EnvironmentName,
                    Timestamp = DateTime.UtcNow
                };

                // Log to database
                await LogToDatabaseAsync(errorLog);

                // Log to file system
                await LogToFileAsync(errorLog);

                return errorId;
            }
            catch (Exception ex)
            {
                // If logging fails, at least log to console/debug
                _logger.LogError(ex, "Failed to log error. Original error: {OriginalError}", exception.Message);
                return errorId;
            }
        }

        public async Task<Guid> LogErrorAsync(ErrorLog errorLog)
        {
            if (errorLog.ErrorId == Guid.Empty)
                errorLog.ErrorId = Guid.NewGuid();

            await LogToDatabaseAsync(errorLog);
            await LogToFileAsync(errorLog);

            return errorLog.ErrorId;
        }

        private async Task LogToDatabaseAsync(ErrorLog errorLog)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_LogError", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ErrorId", errorLog.ErrorId);
                command.Parameters.AddWithValue("@Message", errorLog.Message);
                command.Parameters.AddWithValue("@StackTrace", (object?)errorLog.StackTrace ?? DBNull.Value);
                command.Parameters.AddWithValue("@InnerException", (object?)errorLog.InnerException ?? DBNull.Value);
                command.Parameters.AddWithValue("@ExceptionType", (object?)errorLog.ExceptionType ?? DBNull.Value);
                command.Parameters.AddWithValue("@Source", (object?)errorLog.Source ?? DBNull.Value);
                command.Parameters.AddWithValue("@RequestMethod", (object?)errorLog.RequestMethod ?? DBNull.Value);
                command.Parameters.AddWithValue("@RequestPath", (object?)errorLog.RequestPath ?? DBNull.Value);
                command.Parameters.AddWithValue("@QueryString", (object?)errorLog.QueryString ?? DBNull.Value);
                command.Parameters.AddWithValue("@RequestBody", (object?)errorLog.RequestBody ?? DBNull.Value);
                command.Parameters.AddWithValue("@UserId", (object?)errorLog.UserId ?? DBNull.Value);
                command.Parameters.AddWithValue("@Username", (object?)errorLog.Username ?? DBNull.Value);
                command.Parameters.AddWithValue("@IpAddress", (object?)errorLog.IpAddress ?? DBNull.Value);
                command.Parameters.AddWithValue("@UserAgent", (object?)errorLog.UserAgent ?? DBNull.Value);
                command.Parameters.AddWithValue("@StatusCode", (object?)errorLog.StatusCode ?? DBNull.Value);
                command.Parameters.AddWithValue("@ServerName", (object?)errorLog.ServerName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Environment", (object?)errorLog.Environment ?? DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Error logged to database: {ErrorId}", errorLog.ErrorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log error to database");
                // Don't throw - we don't want logging failures to break the app
            }
        }

        private async Task LogToFileAsync(ErrorLog errorLog)
        {
            await _fileLock.WaitAsync();

            try
            {
                // Create logs directory
                var logsPath = Path.Combine(_environment.ContentRootPath, "Logs");
                Directory.CreateDirectory(logsPath);

                // Create daily log file
                var fileName = $"error-log-{DateTime.UtcNow:yyyy-MM-dd}.txt";
                var filePath = Path.Combine(logsPath, fileName);

                var logEntry = new StringBuilder();
                logEntry.AppendLine($"================================================================================");
                logEntry.AppendLine($"ERROR ID: {errorLog.ErrorId}");
                logEntry.AppendLine($"TIMESTAMP: {errorLog.Timestamp:yyyy-MM-dd HH:mm:ss} UTC");
                logEntry.AppendLine($"ENVIRONMENT: {errorLog.Environment}");
                logEntry.AppendLine($"SERVER: {errorLog.ServerName}");
                logEntry.AppendLine($"--------------------------------------------------------------------------------");
                logEntry.AppendLine($"EXCEPTION TYPE: {errorLog.ExceptionType}");
                logEntry.AppendLine($"MESSAGE: {errorLog.Message}");

                if (!string.IsNullOrEmpty(errorLog.Source))
                    logEntry.AppendLine($"SOURCE: {errorLog.Source}");

                logEntry.AppendLine($"--------------------------------------------------------------------------------");
                logEntry.AppendLine($"REQUEST: {errorLog.RequestMethod} {errorLog.RequestPath}");

                if (!string.IsNullOrEmpty(errorLog.QueryString))
                    logEntry.AppendLine($"QUERY STRING: {errorLog.QueryString}");

                logEntry.AppendLine($"STATUS CODE: {errorLog.StatusCode}");
                logEntry.AppendLine($"--------------------------------------------------------------------------------");

                if (errorLog.UserId.HasValue)
                    logEntry.AppendLine($"USER ID: {errorLog.UserId}");

                if (!string.IsNullOrEmpty(errorLog.Username))
                    logEntry.AppendLine($"USERNAME: {errorLog.Username}");

                logEntry.AppendLine($"IP ADDRESS: {errorLog.IpAddress}");

                if (!string.IsNullOrEmpty(errorLog.UserAgent))
                    logEntry.AppendLine($"USER AGENT: {errorLog.UserAgent}");

                logEntry.AppendLine($"--------------------------------------------------------------------------------");

                if (!string.IsNullOrEmpty(errorLog.RequestBody))
                {
                    logEntry.AppendLine($"REQUEST BODY:");
                    logEntry.AppendLine(errorLog.RequestBody);
                    logEntry.AppendLine($"--------------------------------------------------------------------------------");
                }

                if (!string.IsNullOrEmpty(errorLog.StackTrace))
                {
                    logEntry.AppendLine($"STACK TRACE:");
                    logEntry.AppendLine(errorLog.StackTrace);
                    logEntry.AppendLine($"--------------------------------------------------------------------------------");
                }

                if (!string.IsNullOrEmpty(errorLog.InnerException))
                {
                    logEntry.AppendLine($"INNER EXCEPTION:");
                    logEntry.AppendLine(errorLog.InnerException);
                    logEntry.AppendLine($"--------------------------------------------------------------------------------");
                }

                logEntry.AppendLine();

                await File.AppendAllTextAsync(filePath, logEntry.ToString());

                _logger.LogInformation("Error logged to file: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log error to file");
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<List<ErrorLog>> GetErrorLogsAsync(int top = 100, bool? isResolved = null)
        {
            var errorLogs = new List<ErrorLog>();

            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_GetErrorLogs", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Top", top);
                command.Parameters.AddWithValue("@IsResolved", (object?)isResolved ?? DBNull.Value);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    errorLogs.Add(new ErrorLog
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        ErrorId = reader.GetGuid(reader.GetOrdinal("ErrorId")),
                        Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp")),
                        Message = reader.GetString(reader.GetOrdinal("Message")),
                        ExceptionType = reader.IsDBNull(reader.GetOrdinal("ExceptionType"))
                            ? null : reader.GetString(reader.GetOrdinal("ExceptionType")),
                        RequestMethod = reader.IsDBNull(reader.GetOrdinal("RequestMethod"))
                            ? null : reader.GetString(reader.GetOrdinal("RequestMethod")),
                        RequestPath = reader.IsDBNull(reader.GetOrdinal("RequestPath"))
                            ? null : reader.GetString(reader.GetOrdinal("RequestPath")),
                        UserId = reader.IsDBNull(reader.GetOrdinal("UserId"))
                            ? null : reader.GetInt32(reader.GetOrdinal("UserId")),
                        Username = reader.IsDBNull(reader.GetOrdinal("Username"))
                            ? null : reader.GetString(reader.GetOrdinal("Username")),
                        IpAddress = reader.IsDBNull(reader.GetOrdinal("IpAddress"))
                            ? null : reader.GetString(reader.GetOrdinal("IpAddress")),
                        StatusCode = reader.IsDBNull(reader.GetOrdinal("StatusCode"))
                            ? null : reader.GetInt32(reader.GetOrdinal("StatusCode")),
                        IsResolved = reader.GetBoolean(reader.GetOrdinal("IsResolved"))
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving error logs");
            }

            return errorLogs;
        }

        public async Task<ErrorLog?> GetErrorDetailsAsync(Guid errorId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                using var command = new SqlCommand("sp_GetErrorDetails", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ErrorId", errorId);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new ErrorLog
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        ErrorId = reader.GetGuid(reader.GetOrdinal("ErrorId")),
                        Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp")),
                        Message = reader.GetString(reader.GetOrdinal("Message")),
                        StackTrace = reader.IsDBNull(reader.GetOrdinal("StackTrace"))
                            ? null : reader.GetString(reader.GetOrdinal("StackTrace")),
                        InnerException = reader.IsDBNull(reader.GetOrdinal("InnerException"))
                            ? null : reader.GetString(reader.GetOrdinal("InnerException")),
                        ExceptionType = reader.IsDBNull(reader.GetOrdinal("ExceptionType"))
                            ? null : reader.GetString(reader.GetOrdinal("ExceptionType")),
                        Source = reader.IsDBNull(reader.GetOrdinal("Source"))
                            ? null : reader.GetString(reader.GetOrdinal("Source")),
                        RequestMethod = reader.IsDBNull(reader.GetOrdinal("RequestMethod"))
                            ? null : reader.GetString(reader.GetOrdinal("RequestMethod")),
                        RequestPath = reader.IsDBNull(reader.GetOrdinal("RequestPath"))
                            ? null : reader.GetString(reader.GetOrdinal("RequestPath")),
                        QueryString = reader.IsDBNull(reader.GetOrdinal("QueryString"))
                            ? null : reader.GetString(reader.GetOrdinal("QueryString")),
                        RequestBody = reader.IsDBNull(reader.GetOrdinal("RequestBody"))
                            ? null : reader.GetString(reader.GetOrdinal("RequestBody")),
                        UserId = reader.IsDBNull(reader.GetOrdinal("UserId"))
                            ? null : reader.GetInt32(reader.GetOrdinal("UserId")),
                        Username = reader.IsDBNull(reader.GetOrdinal("Username"))
                            ? null : reader.GetString(reader.GetOrdinal("Username")),
                        IpAddress = reader.IsDBNull(reader.GetOrdinal("IpAddress"))
                            ? null : reader.GetString(reader.GetOrdinal("IpAddress")),
                        UserAgent = reader.IsDBNull(reader.GetOrdinal("UserAgent"))
                            ? null : reader.GetString(reader.GetOrdinal("UserAgent")),
                        StatusCode = reader.IsDBNull(reader.GetOrdinal("StatusCode"))
                            ? null : reader.GetInt32(reader.GetOrdinal("StatusCode")),
                        ServerName = reader.IsDBNull(reader.GetOrdinal("ServerName"))
                            ? null : reader.GetString(reader.GetOrdinal("ServerName")),
                        Environment = reader.IsDBNull(reader.GetOrdinal("Environment"))
                            ? null : reader.GetString(reader.GetOrdinal("Environment")),
                        IsResolved = reader.GetBoolean(reader.GetOrdinal("IsResolved"))
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving error details for {ErrorId}", errorId);
            }

            return null;
        }

        public async Task<bool> MarkAsResolvedAsync(Guid errorId, int resolvedBy, string? notes = null)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();
                using var command = new SqlCommand(@"
                    UPDATE ErrorLogs 
                    SET IsResolved = 1, 
                        ResolvedBy = @ResolvedBy, 
                        ResolvedDate = GETUTCDATE(),
                        Notes = @Notes
                    WHERE ErrorId = @ErrorId", connection);

                command.Parameters.AddWithValue("@ErrorId", errorId);
                command.Parameters.AddWithValue("@ResolvedBy", resolvedBy);
                command.Parameters.AddWithValue("@Notes", (object?)notes ?? DBNull.Value);

                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking error as resolved");
                return false;
            }
        }

        private string? GetIpAddress(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return context.Request.Headers["X-Forwarded-For"].ToString().Split(',').FirstOrDefault()?.Trim();
            }

            if (context.Request.Headers.ContainsKey("X-Real-IP"))
            {
                return context.Request.Headers["X-Real-IP"].ToString();
            }

            return context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }
    }
}