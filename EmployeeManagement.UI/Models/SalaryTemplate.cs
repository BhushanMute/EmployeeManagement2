namespace EmployeeManagement.UI.Models
{
    public class SalaryTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string? TemplateCode { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
