namespace EmployeeManagement.UI.Models
{
    public class BenchmarkResult
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
        public int Iterations { get; set; }
        public long MinTimeMs { get; set; }
        public long MaxTimeMs { get; set; }
        public double AverageTimeMs { get; set; }
        public bool Success { get; set; }
        public int SuccessCount { get; set; }
        public string Rating { get; set; } = "";
    }
}
