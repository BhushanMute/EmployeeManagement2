using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models.Ticket
{
    public class AddAttachmentRequest
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public long FileSize { get; set; }
        public string? FileType { get; set; }
    }
}
