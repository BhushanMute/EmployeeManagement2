namespace EmployeeManagement.API.Models.Payroll
{
    public class EmployeePayrollDetailResponse
    {
        public int PayrollProcessingId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;

        // Salary Breakup
        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetSalary { get; set; }

        // Attendance Details
        public int TotalWorkingDays { get; set; }
        public decimal PresentDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal LOPDays { get; set; }
        public decimal LOPAmount { get; set; }

        // Overtime
        public decimal OvertimeHours { get; set; }
        public decimal OvertimeAmount { get; set; }

        // Additional Amounts
        public decimal ArrearsAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal ReimbursementAmount { get; set; }

        // Statutory Deductions
        public decimal PFEmployee { get; set; }
        public decimal ESIEmployee { get; set; }
        public decimal ProfessionalTax { get; set; }
        public decimal TDS { get; set; }

        // Loan & Advance
        public decimal LoanEMI { get; set; }
        public decimal AdvanceRecovery { get; set; }

        // Bank Details
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }

        // Status
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public bool IsOnHold { get; set; }
        public string? HoldReason { get; set; }

        // Component-wise breakdown
        public List<PayrollComponentResponse> EarningsBreakdown { get; set; } = new();
        public List<PayrollComponentResponse> DeductionsBreakdown { get; set; } = new();

        public DateTime CalculatedDate { get; set; }
    }
}
