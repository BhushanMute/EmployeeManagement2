using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; }
     }
}
