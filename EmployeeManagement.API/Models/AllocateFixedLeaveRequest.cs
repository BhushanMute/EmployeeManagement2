namespace EmployeeManagement.API.Models
{
    public class AllocateFixedLeaveRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public int Year { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.Range(1, 365)]
        public decimal LeavesPerType { get; set; } = 20;
    }
}
