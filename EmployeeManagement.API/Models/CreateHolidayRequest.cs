namespace EmployeeManagement.API.Models
{
    public class CreateHolidayRequest
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Holiday name is required")]
        [System.ComponentModel.DataAnnotations.StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Type { get; set; } = "Public";

        [System.ComponentModel.DataAnnotations.StringLength(500)]
        public string? Description { get; set; }
    }
}
