namespace EmployeeManagement.API.Models.Payroll
{
    public class PayrollRegisterEmployeeData
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string? PANNumber { get; set; }
        public string? UANNumber { get; set; }

        public int WorkingDays { get; set; }
        public decimal PresentDays { get; set; }
        public decimal LOPDays { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal DA { get; set; }
        public decimal OtherAllowances { get; set; }
        public decimal GrossSalary { get; set; }

        public decimal PFEmployee { get; set; }
        public decimal ESIEmployee { get; set; }
        public decimal PT { get; set; }
        public decimal TDS { get; set; }
        public decimal LoanEMI { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal TotalDeductions { get; set; }

        public decimal NetSalary { get; set; }

        public string? BankAccountNumber { get; set; }
        public string? IFSCCode { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;
    }
}
