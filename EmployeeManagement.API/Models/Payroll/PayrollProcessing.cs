namespace EmployeeManagement.API.Models.Payroll
{
    public class PayrollProcessing
    {
        public int Id { get; set; }
        public int PayrollCycleId { get; set; }
        public int EmployeeId { get; set; }
        public int EmployeeSalaryStructureId { get; set; }

        // Salary Breakup
        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetSalary { get; set; }
        public decimal CTC { get; set; }

        // Attendance-based Calculations
        public int TotalWorkingDays { get; set; }
        public decimal PresentDays { get; set; }
        public decimal AbsentDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal UnpaidLeaveDays { get; set; }
        public int WeeklyOffDays { get; set; }
        public int HolidayDays { get; set; }

        // LOP
        public decimal LOPDays { get; set; }
        public decimal LOPAmount { get; set; }

        // Overtime
        public decimal OvertimeHours { get; set; }
        public decimal OvertimeAmount { get; set; }

        // Arrears & Adjustments
        public decimal ArrearsAmount { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public decimal BonusAmount { get; set; }

        // Reimbursements
        public decimal TotalReimbursements { get; set; }

        // Statutory Deductions
        public decimal PFEmployee { get; set; }
        public decimal PFEmployer { get; set; }
        public decimal ESIEmployee { get; set; }
        public decimal ESIEmployer { get; set; }
        public decimal ProfessionalTax { get; set; }
        public decimal TDS { get; set; }

        // Loan & Advance Deductions
        public decimal LoanEMI { get; set; }
        public decimal AdvanceRecovery { get; set; }

        // Payment Details
        public string? PaymentMode { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime? PaymentDate { get; set; }
        public string? PaymentReferenceNo { get; set; }

        // Bank Details
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankIFSC { get; set; }

        // Status & Approval
        public string Status { get; set; } = "Draft"; // Draft, Calculated, Verified, Approved, Paid
        public bool IsOnHold { get; set; }
        public string? HoldReason { get; set; }

        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // Calculation Metadata
        public DateTime? CalculatedDate { get; set; }
        public DateTime? LastRecalculatedDate { get; set; }

        // Remarks
        public string? Remarks { get; set; }

        // Metadata
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual PayrollCycle? PayrollCycle { get; set; }
        public virtual ICollection<PayrollProcessingDetail> Details { get; set; } = new List<PayrollProcessingDetail>();
    }
}
