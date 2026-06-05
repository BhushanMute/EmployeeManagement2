namespace EmployeeManagement.UI.Models
{
    namespace EmployeeManagement.UI.Models.DTOs
    {
        public class SalarySlipResponse
        {
            public int Id { get; set; }
            public int SlipId { get; set; }
            public string? SlipNumber { get; set; }
            public string? MonthName { get; set; }
            public int Year { get; set; }

            public decimal GrossSalary { get; set; }
            public decimal TotalDeductions { get; set; }
            public decimal NetSalary { get; set; }

            public string? NetSalaryInWords { get; set; }
            public string? Status { get; set; }

            public bool EmailSent { get; set; }
            public DateTime? EmailSentDate { get; set; }

            // Optional (avoid errors)
            public int WorkingDays { get; set; }
            public string? DepartmentName { get; set; }
            public string? PAN { get; set; }
            public string? PFNumber { get; set; }
            public int AbsentDays { get; set; }
            public int LeaveDays { get; set; }
        }
    }
}
