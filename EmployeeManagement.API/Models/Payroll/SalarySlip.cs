namespace EmployeeManagement.API.Models.Payroll
{
    public class SalarySlip
    {
        public int Id { get; set; }
        public string SlipNumber { get; set; } = string.Empty;

        // Links
        public int PayrollProcessingId { get; set; }
        public int PayrollCycleId { get; set; }
        public int EmployeeId { get; set; }

        // Period Info
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public DateTime? PaymentDate { get; set; }

        // Salary Summary
        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetSalary { get; set; }

        // Attendance Summary
        public int TotalWorkingDays { get; set; }
        public decimal PresentDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal LOPDays { get; set; }

        // SSRS Report Integration
        public string? SSRSReportPath { get; set; }
        public string? SSRSReportParameters { get; set; }

        // PDF Generation
        public DateTime? PDFGeneratedDate { get; set; }
        public string? PDFFilePath { get; set; }
        public long? PDFFileSize { get; set; }
        public string? PDFGenerationStatus { get; set; }
        public string? PDFGenerationError { get; set; }

        // Digital Signature
        public bool IsDigitallySigned { get; set; }
        public int? SignedBy { get; set; }
        public DateTime? SignedDate { get; set; }
        public string? DigitalSignaturePath { get; set; }

        // Email Distribution
        public DateTime? EmailSentDate { get; set; }
        public string? EmailStatus { get; set; }
        public string? EmailSentTo { get; set; }
        public string? EmailFailureReason { get; set; }

        // Employee Self-Service Access
        public bool ViewedByEmployee { get; set; }
        public DateTime? FirstViewedDate { get; set; }
        public int ViewCount { get; set; }
        public DateTime? LastViewedDate { get; set; }
        public int DownloadCount { get; set; }
        public DateTime? LastDownloadedDate { get; set; }

        // Password Protection
        public bool IsPasswordProtected { get; set; }
        public string? PasswordHash { get; set; }

        // Status
        public string Status { get; set; } = "Draft";
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }

        // Metadata
        public int? GeneratedBy { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public string? Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual PayrollProcessing? PayrollProcessing { get; set; }
    }
}
