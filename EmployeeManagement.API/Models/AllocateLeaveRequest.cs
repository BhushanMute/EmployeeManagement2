using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class AllocateLeaveRequest
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [Range(0, 365)]
        public decimal TotalAllocated { get; set; }

        [Range(0, 365)]
        public decimal CarryForward { get; set; }
    }
}
