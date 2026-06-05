namespace EmployeeManagement.UI.Models
{
    public class BulkSalaryAssignRequest
    {
        public int TemplateId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<int> EmployeeIds { get; set; } = new();
    }
}
