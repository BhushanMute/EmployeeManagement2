namespace EmployeeManagement.API.Models.Payroll
{
    public class LoanDetailsResponse
    {
        public int LoanId { get; set; }
        public string LoanNumber { get; set; } = string.Empty;

        // Employee Details
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;

        // Loan Type
        public int LoanTypeId { get; set; }
        public string LoanTypeName { get; set; } = string.Empty;

        // Loan Details
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal EMIAmount { get; set; }
        public decimal TotalRepayableAmount { get; set; }

        // Application Details
        public DateTime ApplicationDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public string? Purpose { get; set; }

        // Status
        public string Status { get; set; } = string.Empty;
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? DisbursementDate { get; set; }

        // Repayment Summary
        public DateTime? FirstEMIDate { get; set; }
        public DateTime? LastEMIDate { get; set; }
        public int TotalEMIs { get; set; }
        public int TotalEMIsPaid { get; set; }
        public int RemainingEMIs { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public decimal PrincipalPaid { get; set; }
        public decimal InterestPaid { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal OutstandingPrincipal { get; set; }
        public decimal OutstandingInterest { get; set; }

        // EMI Schedule
        public List<LoanEMIResponse> EMISchedule { get; set; } = new();

        // Guarantor Details
        public string? GuarantorName { get; set; }
        public string? GuarantorRelation { get; set; }

        public bool IsFullyPaid { get; set; }
        public DateTime? ClosureDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? DepartmentName { get; set; }
        public string? DisbursedByName { get; set; }
        public string? GuarantorEmployeeName { get; set; }

        // Add this also (VERY IMPORTANT)
        public List<LoanPrepayment> Prepayments { get; set; } = new();
    }
}
