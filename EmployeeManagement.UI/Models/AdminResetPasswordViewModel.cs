using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.UI.Models
{
    public class AdminResetPasswordViewModel
    {
        [Required]
        public int UserId { get; set; }

        public string? Username { get; set; }
        public string? Email { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Send Email Notification")]
        public bool SendEmailNotification { get; set; } = true;
    }
}
