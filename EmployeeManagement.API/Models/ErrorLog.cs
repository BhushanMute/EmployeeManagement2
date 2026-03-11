namespace EmployeeManagement.API.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public Guid ErrorId { get; set; }
        public DateTime Timestamp { get; set; }

        // Error Details
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
        public string? ExceptionType { get; set; }
        public string? Source { get; set; }

        // Request Details
        public string? RequestMethod { get; set; }
        public string? RequestPath { get; set; }
        public string? QueryString { get; set; }
        public string? RequestBody { get; set; }

        // User Context
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        // Response
        public int? StatusCode { get; set; }

        // Additional Info
        public string? ServerName { get; set; }
        public string? Environment { get; set; }
        public bool IsResolved { get; set; }
        public int? ResolvedBy { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? Notes { get; set; }
    }
}
