using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Payroll
{
    public class EmployeeAttendanceData
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [Range(0, 31, ErrorMessage = "Present days must be between 0 and 31")]
        public decimal PresentDays { get; set; }

        [Range(0, 31, ErrorMessage = "Paid leave days must be between 0 and 31")]
        public decimal PaidLeaveDays { get; set; } = 0;

        [Range(0, 31, ErrorMessage = "LOP days must be between 0 and 31")]
        public decimal LOPDays { get; set; } = 0;

        [Range(0, 500, ErrorMessage = "Overtime hours must be between 0 and 500")]
        public decimal OvertimeHours { get; set; } = 0;

        public decimal ArrearsAmount { get; set; } = 0;
        public decimal BonusAmount { get; set; } = 0;
        public decimal AdjustmentAmount { get; set; } = 0;
    }
}
