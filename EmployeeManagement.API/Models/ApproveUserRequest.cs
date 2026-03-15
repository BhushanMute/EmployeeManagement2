namespace EmployeeManagement.API.Models
{
    public class ApproveUserRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        public int UserId { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public int RoleId { get; set; }

        public int? DepartmentId { get; set; }
    }
}
