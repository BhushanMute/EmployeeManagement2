namespace EmployeeManagement.API.Models
{
    public class DisburseLoanRequest
    {
        public DateTime DisbursementDate { get; set; }
        public string Mode { get; set; } = string.Empty; // BankTransfer, Cash, Cheque
        public string? ReferenceNo { get; set; }
    }
}
