namespace EmployeeManagement.API.Models.Payroll
{
    public class YTDSummaryResponse
    {
        public decimal YTDEarnings { get; set; }
        public decimal YTDDeductions { get; set; }
        public decimal YTDNetSalary { get; set; }
        public decimal YTDPF { get; set; }
        public decimal YTDESI { get; set; }
        public decimal YTDPT { get; set; }
        public decimal YTDTDS { get; set; }
    }
}
