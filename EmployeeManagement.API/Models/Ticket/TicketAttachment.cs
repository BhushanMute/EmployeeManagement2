namespace EmployeeManagement.API.Models.Ticket
{
    public class TicketAttachment
    {
        public int AttachmentId { get; set; }
        public int TicketId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? FileType { get; set; }
        public int UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
        public bool IsDeleted { get; set; }

        // User Info (from JOIN)
        public string? UploadedByName { get; set; }

        // Computed Properties
        public string FileSizeFormatted => FormatFileSize(FileSize);
        public string FileExtension => System.IO.Path.GetExtension(FileName)?.ToLower() ?? "";

        public bool IsImage => FileExtension switch
        {
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" => true,
            _ => false
        };

        public string FileIcon => FileExtension switch
        {
            ".pdf" => "bi-file-earmark-pdf text-danger",
            ".doc" or ".docx" => "bi-file-earmark-word text-primary",
            ".xls" or ".xlsx" => "bi-file-earmark-excel text-success",
            ".txt" => "bi-file-earmark-text",
            ".zip" or ".rar" => "bi-file-earmark-zip",
            ".jpg" or ".jpeg" or ".png" or ".gif" => "bi-file-earmark-image text-info",
            ".mp4" or ".avi" or ".mov" => "bi-file-earmark-play",
            ".log" => "bi-file-earmark-code",
            _ => "bi-file-earmark"
        };

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }
}
