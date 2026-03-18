namespace EmployeeManagement.API.Models
{
    public class AuditLogRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int? UserId { get; set; }
        public string? Action { get; set; }
        public string? EntityName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
