using EmployeeManagement.API.Models.Payroll;

namespace EmployeeManagement.UI.Models
{
    public class SalarySlipResponse
    {
        // ===== Basic Slip Info =====
        public int SlipId { get; set; }
        public string? SlipNumber { get; set; }
        public int? PayrollCycleId { get; set; }
        public int EmployeeId { get; set; }

        // ===== Employee Details =====
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Email { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? PanNumber { get; set; }

        // ===== Period =====
        public int Month { get; set; }
        public string? MonthName { get; set; }
        public int Year { get; set; }
        public DateTime? PayPeriodStart { get; set; }
        public DateTime? PayPeriodEnd { get; set; }

        // ===== Status & Tracking =====
        public string? Status { get; set; }
        public bool? EmailSent { get; set; }
        public DateTime? EmailSentDate { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public int ViewCount { get; set; }
        public int DownloadCount { get; set; }

        // ===== Salary Amounts =====
        public decimal? BasicSalary { get; set; }
        public decimal? GrossSalary { get; set; }
        public decimal? TotalEarnings { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? NetSalary { get; set; }
        public string? NetSalaryInWords { get; set; }

        // ===== Attendance =====
        public int? TotalDays { get; set; }
        public int? TotalWorkingDays { get; set; }
        public int? PresentDays { get; set; }
        public int? LeaveDays { get; set; }
        public int? PaidLeaveDays { get; set; }
        public int? AbsentDays { get; set; }
        public decimal? PaidDays { get; set; }
        public decimal? LopDays { get; set; }

        public decimal? LOPDays
        {
            get => LopDays;
            set => LopDays = value;
        }

        // ===== Bank Details =====
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }

        public string? AccountNumber
        {
            get => BankAccountNumber;
            set => BankAccountNumber = value;
        }

        public string? IfscCode { get; set; }

        public string? IFSCCode
        {
            get => IfscCode;
            set => IfscCode = value;
        }

        // ===== Related Objects =====
        public CompanyDetailsResponse? CompanyDetails { get; set; }
        public List<SlipComponentResponse>? Earnings { get; set; }
        public List<SlipComponentResponse>? Deductions { get; set; }
    }

    // ✅ FULL Company Details with ALL properties
    
    // ✅ FULL Component Response with ComponentCode
    public class SlipComponentResponse
    {
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? ComponentCode { get; set; }     // ✅ Required by View
        public decimal Amount { get; set; }
        public string? ComponentType { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsTaxable { get; set; }
    }
}