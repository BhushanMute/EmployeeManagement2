namespace EmployeeManagement.API.Models
{
    public class AuditLogResponse
    {
        public List<AuditLog> Logs { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
    }
}
