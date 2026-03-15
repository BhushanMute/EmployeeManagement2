using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.UI.ViewModels
{
    public class ApplyLeaveViewModel
    {
        [Required(ErrorMessage = "Employee is required")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Leave type is required")]
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Reason must be 10-1000 characters")]
        [Display(Name = "Reason")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Half Day")]
        public bool IsHalfDay { get; set; }

        [Display(Name = "Half Day Type")]
        public string? HalfDayType { get; set; }

        [Display(Name = "Emergency Contact")]
        public string? EmergencyContact { get; set; }

        [Display(Name = "Attachment")]
        public IFormFile? Attachment { get; set; }

        // For dropdown
        public List<LeaveTypeViewModel>? LeaveTypes { get; set; }
        public string? EmployeeName { get; set; }
    }
}
