namespace EmployeeManagement.API.Models.Payroll
{
    public class OverdueEMI
    {
        public int LoanId { get; set; }
        public string LoanNumber { get; set; } = string.Empty;

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public int EMINumber { get; set; }
        public DateTime DueDate { get; set; }

        public decimal EMIAmount { get; set; }
        public decimal OutstandingAmount { get; set; }

        public int DaysOverdue { get; set; }
        public decimal LateFee { get; set; }

        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
    }
}