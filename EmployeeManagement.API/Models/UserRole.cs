using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.API.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public int? AssignedBy { get; set; }

        public DateTime? RevokedDate { get; set; }

        public int? RevokedBy { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;
    }
}
