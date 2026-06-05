namespace EmployeeManagement.API.Models.Payroll
{
    public class SalarySlipResponse
    {
        public int SlipId { get; set; }
        public string SlipNumber { get; set; } = string.Empty;

        // Employee Details
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public DateTime? JoiningDate { get; set; }
        public string? PANNumber { get; set; }
        public string? UAN { get; set; }

        // Period Details
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public DateTime? PaymentDate { get; set; }

        // Salary Summary
        public decimal BasicSalary { get; set; }
        public decimal? GrossSalary { get; set; }
        public decimal? TotalEarnings { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? NetSalary { get; set; }
        public string NetSalaryInWords { get; set; } = string.Empty;

        // Attendance Summary
        public int TotalWorkingDays { get; set; }
        public decimal PresentDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal LOPDays { get; set; }
        public int WeeklyOffDays { get; set; }
        public int HolidayDays { get; set; }

        // Earnings Breakdown
        public List<SlipComponentResponse> Earnings { get; set; } = new();

        // Deductions Breakdown
        public List<SlipComponentResponse> Deductions { get; set; } = new();

        // Bank Details
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }

        // Year-to-Date Summary
        public YTDSummaryResponse? YTDSummary { get; set; }

        // Company Details
        public CompanyDetailsResponse CompanyDetails { get; set; } = new();

        // PDF Details
        public string? PDFFilePath { get; set; }
        public DateTime? PDFGeneratedDate { get; set; }
        public bool IsPasswordProtected { get; set; }

        // Email Status
        public bool EmailSent { get; set; }
        public DateTime? EmailSentDate { get; set; }

        // View Status
        public bool ViewedByEmployee { get; set; }
        public int ViewCount { get; set; }
        public int DownloadCount { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
    }
}
