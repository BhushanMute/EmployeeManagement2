using System;

namespace EmployeeManagement.API.Models.Payroll
{
    public class PFReportData
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;

        // PF Details
        public string? UAN { get; set; }
        public string? PFNumber { get; set; }

        public decimal PFWages { get; set; }
        public decimal EmployeeContribution { get; set; }
        public decimal EmployerContribution { get; set; }

        // Optional breakdown
        public decimal EPSContribution { get; set; }
        public decimal EPFContribution { get; set; }

        // Payroll संदर्भ
        public int CycleId { get; set; }
        public string? CycleName { get; set; }
        public DateTime? WageMonth { get; set; }
    }
}