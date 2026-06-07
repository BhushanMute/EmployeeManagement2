namespace EmployeeManagement.UI.Models
{
    // ===========================================================
    // 1. EmployeePayrollDetailResponse
    // Used in: SalarySlipList.cshtml
    // API Endpoint: GET api/payroll/cycle/{cycleId}/register
    // ===========================================================
    public class EmployeePayrollDetailResponse
    {
        // ===== IDs =====
        public int Id { get; set; }
        public int PayrollProcessingId { get; set; }
        public int PayrollCycleId { get; set; }
        public int EmployeeId { get; set; }
        public int? SlipId { get; set; }

        // ===== Employee Info =====
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }

        // ===== Attendance =====
        public int? TotalDays { get; set; }
        public int? TotalWorkingDays { get; set; }
        public decimal? PresentDays { get; set; }
        public decimal? PaidDays { get; set; }
        public decimal? LopDays { get; set; }
        public int? LeaveDays { get; set; }
        public int? AbsentDays { get; set; }

        // ===== Salary Components =====
        public decimal? BasicSalary { get; set; }
        public decimal? HRA { get; set; }
        public decimal? DA { get; set; }
        public decimal? Allowances { get; set; }
        public decimal? OtherEarnings { get; set; }

        // ===== Deductions =====
        public decimal? PF { get; set; }
        public decimal? ESI { get; set; }
        public decimal? TDS { get; set; }
        public decimal? ProfessionalTax { get; set; }
        public decimal? OtherDeductions { get; set; }

        // ===== Totals =====
        public decimal? GrossSalary { get; set; }
        public decimal? TotalEarnings { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? NetSalary { get; set; }

        // ===== Status =====
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public bool? IsSlipGenerated { get; set; }
        public bool? EmailSent { get; set; }
        public DateTime? EmailSentDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime? GeneratedDate { get; set; }

        // ===== Bank Info =====
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? IfscCode { get; set; }

        // ===== Slip Info =====
        public string? SlipNumber { get; set; }
    }

    // ===========================================================
    // 2. PayrollSummaryResponse
    // Used in: GenerateSalarySlips.cshtml, SalarySlipList.cshtml
    // API Endpoint: GET api/payroll/cycle/{cycleId}/summary
    // ===========================================================
   

    // ===========================================================
    // 4. PayrollCycleResponse (Optional - if needed)
    // ===========================================================
    public class PayrollCycleResponse
    {
        public int CycleId { get; set; }
        public string? CycleName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? MonthName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? PayDate { get; set; }
        public string? Status { get; set; }
        public int TotalEmployees { get; set; }
        public decimal TotalNetSalary { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}