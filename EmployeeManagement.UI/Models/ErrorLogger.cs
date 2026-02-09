
using System;
using System.IO;
namespace EmployeeManagement.UI.Models
{
    public static class ErrorLogger
    {
        // Path to the log file
        private static readonly string _logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        private static readonly string _filePath = Path.Combine(_logFolder, "error-log.txt");

        // Log a simple message
        public static void Log(string message)
        {
            EnsureLogFolderExists();
            File.AppendAllText(_filePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
        }

        // Log an exception
        public static void Log(Exception ex)
        {
            EnsureLogFolderExists();
            File.AppendAllText(_filePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Exception: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}{Environment.NewLine}");
        }

        // Ensure the folder exists, create if missing
        private static void EnsureLogFolderExists()
        {
            if (!Directory.Exists(_logFolder))
            {
                Directory.CreateDirectory(_logFolder);
            }
        }
    }
}
