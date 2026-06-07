namespace EmployeeManagement.UI.Models
{
    public class AssignSalaryRequest
    {
        public int EmployeeId { get; set; }
        public int? TemplateId { get; set; }
        public DateTime EffectiveDate { get; set; } = DateTime.Now;

        // These are needed if API expects them
        public decimal CTC { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
    }
}
