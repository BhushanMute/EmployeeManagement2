namespace EmployeeManagement.UI.Models
{
    public class PayrollSummaryResponse
    {
        public int PayrollCycleId { get; set; }

        public int CycleId
        {
            get => PayrollCycleId;
            set => PayrollCycleId = value;
        }

        public string CycleName { get; set; } = string.Empty;
        public string CycleCode { get; set; } = string.Empty;

        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FinancialYear { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public bool IsLocked { get; set; }

        public int TotalEmployees { get; set; }
        public int ProcessedEmployees { get; set; }
        public int PendingEmployees { get; set; }

        public decimal TotalGrossSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetSalary { get; set; }

        public decimal TotalGross
        {
            get => TotalGrossSalary;
            set => TotalGrossSalary = value;
        }

        public decimal TotalNetPay
        {
            get => TotalNetSalary;
            set => TotalNetSalary = value;
        }

        public decimal TotalPF { get; set; }
        public decimal TotalESI { get; set; }
        public decimal TotalPT { get; set; }
        public decimal TotalTDS { get; set; }

        public DateTime? ProcessingDate { get; set; }
        public DateTime? SalaryCreditDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedByName { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
