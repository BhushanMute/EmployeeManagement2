namespace EmployeeManagement.UI.Models
{
    public class CacheBenchmarkResult
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
        public long ColdTimeMs { get; set; }
        public long WarmTimeMs { get; set; }
        public double ImprovementPercent { get; set; }
        public bool IsCacheEffective { get; set; }
    }
}
