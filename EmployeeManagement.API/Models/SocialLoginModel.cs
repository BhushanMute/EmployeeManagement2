namespace EmployeeManagement.API.Models
{
    public class SocialLoginModel
    {
        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        // Google, Facebook, etc.
        public string Provider { get; set; } = string.Empty;

        // GoogleId or FacebookId
        public string SocialId { get; set; } = string.Empty;
    }
}
