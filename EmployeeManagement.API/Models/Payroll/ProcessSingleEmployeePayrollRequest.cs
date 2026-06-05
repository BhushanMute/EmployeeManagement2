using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class ProcessSingleEmployeePayrollRequest
    {
        [Required]
        public int PayrollCycleId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [Range(1, 31)]
        public int TotalWorkingDays { get; set; }

        [Required]
        [Range(0, 31)]
        public decimal PresentDays { get; set; }

        public decimal PaidLeaveDays { get; set; } = 0;
        public int WeeklyOffDays { get; set; } = 4;
        public int HolidayDays { get; set; } = 0;
        public decimal OvertimeHours { get; set; } = 0;
        public decimal ArrearsAmount { get; set; } = 0;
        public decimal BonusAmount { get; set; } = 0;
    }
}
