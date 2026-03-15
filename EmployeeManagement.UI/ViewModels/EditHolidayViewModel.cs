using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.UI.ViewModels
{
    public class EditHolidayViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Holiday name is required")]
        [StringLength(200, MinimumLength = 2)]
        [Display(Name = "Holiday Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Holiday Date")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Holiday Type")]
        public string Type { get; set; } = "Public";

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
