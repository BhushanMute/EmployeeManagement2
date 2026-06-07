using System;

namespace EmployeeManagement.API.Models.Payroll
{
    public class ESIReportData
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;

        // ESI Details
        public string? ESINumber { get; set; }

        public decimal GrossWages { get; set; }
        public decimal EmployeeContribution { get; set; }
        public decimal EmployerContribution { get; set; }

        // Payroll संदर्भ
        public int CycleId { get; set; }
        public string? CycleName { get; set; }
        public DateTime? WageMonth { get; set; }
    }
}