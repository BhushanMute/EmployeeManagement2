using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class CancelLeaveRequest
    {
        [Required]
        public int LeaveRequestId { get; set; }

        [StringLength(500)]
        public string? CancelReason { get; set; }
    }
}
