using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class ProcessPayrollRequest
    {
        [Required(ErrorMessage = "Payroll cycle ID is required")]
        public int PayrollCycleId { get; set; }

        public List<EmployeeAttendanceData>? AttendanceData { get; set; }

        public bool IncludeReimbursements { get; set; } = true;
        public bool ProcessLoans { get; set; } = true;
        public bool ProcessAdvances { get; set; } = true;
        public bool CalculateTDS { get; set; } = true;
    }
}
