namespace EmployeeManagement.API.Models
{
    public class GoogleLoginModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; } = "Google";
        public string? SocialId { get; set; }


    }
}
