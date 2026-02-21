namespace EmployeeManagement.API.Models
{
    public class User
    {
        public int Id { get; set; }

        // Common fields
        public string Username { get; set; }

        public string Email { get; set; }

        // NULL for Google/Facebook login
        public string? PasswordHash { get; set; }

        // Local, Google, Facebook
        public string Provider { get; set; }

        // Social login IDs
        public string? GoogleId { get; set; }

        public string? FacebookId { get; set; }

        // Audit field
        public DateTime CreatedDate { get; set; }
    }
}
