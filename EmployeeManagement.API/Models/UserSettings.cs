namespace EmployeeManagement.API.Models
{
    public class UserSettings
    {
        public int UserId { get; set; }
        public string Theme { get; set; } = "light";
        public string Language { get; set; } = "en";
        public bool EmailNotifications { get; set; } = true;
        public bool TwoFactorEnabled { get; set; } = false;
    }
}
