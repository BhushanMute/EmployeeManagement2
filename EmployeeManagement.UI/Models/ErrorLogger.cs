public static class ErrorLogger
{
    private static readonly string _logFolder =
        Path.Combine(Directory.GetCurrentDirectory(), "Logs");

    private static string GetFilePath()
    {
        string fileName = $"error-log-{DateTime.Now:yyyy-MM-dd}.txt";
        return Path.Combine(_logFolder, fileName);
    }

    public static void Log(string message)
    {
        EnsureLogFolderExists();
        File.AppendAllText(GetFilePath(),
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
    }

    public static void Log(Exception ex)
    {
        EnsureLogFolderExists();
        File.AppendAllText(GetFilePath(),
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Exception: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}{Environment.NewLine}");
    }

    private static void EnsureLogFolderExists()
    {
        if (!Directory.Exists(_logFolder))
        {
            Directory.CreateDirectory(_logFolder);
        }
    }
}