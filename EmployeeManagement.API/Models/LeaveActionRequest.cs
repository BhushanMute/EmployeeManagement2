using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class LeaveActionRequest
    {
        [Required]
        public int LeaveRequestId { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
