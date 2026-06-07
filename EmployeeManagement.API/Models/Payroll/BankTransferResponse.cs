namespace EmployeeManagement.API.Models.Payroll
{
    public class BankTransferResponse
    {
        public string BatchNumber { get; set; } = string.Empty;
        public int PayrollCycleId { get; set; }
        public string CycleName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }

        public int TotalEmployees { get; set; }
        public decimal TotalAmount { get; set; }

        public string? CompanyBankAccount { get; set; }
        public string? CompanyIFSC { get; set; }

        public List<BankTransferEmployeeData> Employees { get; set; } = new();

        public DateTime GeneratedDate { get; set; }
    }
}
