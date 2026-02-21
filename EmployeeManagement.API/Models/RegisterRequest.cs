using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }   // must exist

    }
}
