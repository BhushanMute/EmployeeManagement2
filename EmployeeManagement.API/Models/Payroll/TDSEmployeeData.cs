namespace EmployeeManagement.API.Models.Payroll
{
    public class TDSEmployeeData
    {
        public string PANNumber { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public decimal GrossIncome { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal TDSAmount { get; set; }
        public string TaxRegime { get; set; } = string.Empty;
    }
}
