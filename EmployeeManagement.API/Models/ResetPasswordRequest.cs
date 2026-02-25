using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class ResetPasswordRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
