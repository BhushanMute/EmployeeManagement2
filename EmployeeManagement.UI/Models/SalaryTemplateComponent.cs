namespace EmployeeManagement.UI.Models
{
    public class SalaryTemplateComponent
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int ComponentId { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public string ComponentType { get; set; } = string.Empty; // Earning / Deduction
        public string? CalculationType { get; set; }
        public decimal? DefaultAmount { get; set; }
        public decimal? DefaultPercentage { get; set; }
        public string AmountOrPercentage
        {
            get
            {
                if (CalculationType == "Percentage" && DefaultPercentage.HasValue)
                    return $"{DefaultPercentage.Value}%";
                if (DefaultAmount.HasValue)
                    return $"₹ {DefaultAmount.Value:N2}";
                return "-";
            }
        }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

    }
}
