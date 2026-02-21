using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }
}
