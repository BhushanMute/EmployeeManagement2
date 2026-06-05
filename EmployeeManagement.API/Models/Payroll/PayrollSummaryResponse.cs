namespace EmployeeManagement.API.Models.Payroll
{
    public class PayrollSummaryResponse
    {
        public int PayrollCycleId { get; set; }
        public string CycleName { get; set; } = string.Empty;
        public string CycleCode { get; set; } = string.Empty;

        // Period Details
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FinancialYear { get; set; } = string.Empty;

        // Status
        public string Status { get; set; } = string.Empty;
        public bool IsLocked { get; set; }

        // Statistics
        public int TotalEmployees { get; set; }
        public int ProcessedEmployees { get; set; }
        public int PendingEmployees { get; set; }

        // Financial Summary
        public decimal TotalGrossSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetSalary { get; set; }

        // Statutory Summary
        public decimal TotalPF { get; set; }
        public decimal TotalESI { get; set; }
        public decimal TotalPT { get; set; }
        public decimal TotalTDS { get; set; }

        // Processing Info
        public DateTime? ProcessingDate { get; set; }
        public DateTime? SalaryCreditDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedByName { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
