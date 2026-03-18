namespace EmployeeManagement.UI.ViewModels
{
    public class AuditLogResponseViewModel
    {
        public List<AuditLogViewModel> Logs { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public AuditLogFilterViewModel Filter { get; set; } = new();
    }
}
