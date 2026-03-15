using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.UI.ViewModels
{
    public class CreateHolidayViewModel
    {
        [Required(ErrorMessage = "Holiday name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be 2-200 characters")]
        [Display(Name = "Holiday Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Holiday Date")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Type is required")]
        [Display(Name = "Holiday Type")]
        public string Type { get; set; } = "Public";

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
