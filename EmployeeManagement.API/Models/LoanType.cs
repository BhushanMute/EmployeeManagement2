namespace EmployeeManagement.API.Models.Payroll
{
    public class LoanType
    {
        public int Id { get; set; }

        public string LoanTypeCode { get; set; } = string.Empty;
        public string LoanTypeName { get; set; } = string.Empty;
        public string? Description { get; set; }

        public decimal InterestRate { get; set; }

        public decimal MaxAmount { get; set; }
        public decimal MinAmount { get; set; }

        public int MaxTenureMonths { get; set; }
        public int MinTenureMonths { get; set; }

        public bool RequiresGuarantor { get; set; }
        public bool RequiresCollateral { get; set; }

        public decimal MaxLoanMultiplier { get; set; }
        public decimal ProcessingFeePercent { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}