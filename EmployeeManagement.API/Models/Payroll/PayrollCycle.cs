namespace EmployeeManagement.API.Models.Payroll
{
    public class PayrollCycle
    {
        public int Id { get; set; }
        public string CycleName { get; set; } = string.Empty;
        public string CycleCode { get; set; } = string.Empty;

        // Period Definition
        public string PeriodType { get; set; } = "Monthly"; // Monthly, Weekly, Bi-Weekly
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Processing Dates
        public DateTime? ProcessingDate { get; set; }
        public DateTime? SalaryCreditDate { get; set; }

        // Financial Year
        public string FinancialYear { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }

        // Status Management
        public string Status { get; set; } = "Draft"; // Draft, InProgress, Processed, Approved, Paid, Closed

        // Lock Mechanism
        public bool IsLocked { get; set; }
        public int? LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }

        // Approval Workflow
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovalRemarks { get; set; }

        // Statistics
        public int TotalEmployees { get; set; }
        public int ProcessedEmployees { get; set; }
        public decimal TotalGrossSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetSalary { get; set; }

        // Metadata
        public string? Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual ICollection<PayrollProcessing> PayrollProcessings { get; set; } = new List<PayrollProcessing>();
    }
}
