namespace EmployeeManagement.UI.Models
{
    public class ApiEndpointResult
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
        public string Method { get; set; } = "GET";
        public string Category { get; set; } = "";
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public long ResponseTimeMs { get; set; }
        public string ResponseSize { get; set; } = "0 B";
        public long ResponseSizeBytes { get; set; }
        public int? RecordCount { get; set; }
        public string ContentType { get; set; } = "";
        public string? Error { get; set; }
    }
}
