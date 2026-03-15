namespace EmployeeManagement.API.Models
{
    public class RejectUserRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public int UserId { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(500)]
        public string? RejectionReason { get; set; }
    }
}
