using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.API.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? IpAddress { get; set; }

        public bool IsRevoked { get; set; } = false;

        public DateTime? RevokedDate { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime? UsedDate { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // Computed property
        public bool IsActive => !IsRevoked && !IsUsed && DateTime.UtcNow < ExpiryDate;
    }
}
