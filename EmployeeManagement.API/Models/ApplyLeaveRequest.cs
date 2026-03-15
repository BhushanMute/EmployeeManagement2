using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class ApplyLeaveRequest
    {
        [Required(ErrorMessage = "Employee ID is required")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Leave type is required")]
        public int LeaveTypeId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Reason must be 10-1000 characters")]
        public string Reason { get; set; } = string.Empty;

        public bool IsHalfDay { get; set; }
        public string? HalfDayType { get; set; }
        public string? EmergencyContact { get; set; }
    }
}
