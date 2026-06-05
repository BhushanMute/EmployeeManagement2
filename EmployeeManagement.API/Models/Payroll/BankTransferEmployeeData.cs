namespace EmployeeManagement.API.Models.Payroll
{
    public class BankTransferEmployeeData
    {
        public int SrNo { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string IFSCCode { get; set; } = string.Empty;
        public decimal NetSalary { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}
