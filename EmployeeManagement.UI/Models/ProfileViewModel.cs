// Models/ProfileViewModel.cs (UI Project)
namespace EmployeeManagement.UI.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }

        // ✅ Profile Picture Properties
        public string? ProfilePicture { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // ✅ Display property
        public string ProfilePictureDisplay => !string.IsNullOrEmpty(ProfilePictureUrl)
            ? ProfilePictureUrl
            : !string.IsNullOrEmpty(ProfilePicture)
                ? ProfilePicture
                : "/images/default-avatar.png";

        // ✅ Check if has picture
        public bool HasProfilePicture => !string.IsNullOrEmpty(ProfilePicture) ||
                                          !string.IsNullOrEmpty(ProfilePictureUrl);

        // ✅ Get initials
        public string Initials => $"{FirstName?.FirstOrDefault()}{LastName?.FirstOrDefault()}".ToUpper();

        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }
}