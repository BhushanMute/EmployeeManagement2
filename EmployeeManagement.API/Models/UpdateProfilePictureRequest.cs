using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class UpdateProfilePictureRequest
    {
        [Required]
        public IFormFile ProfilePicture { get; set; } = null!;
    }
}
