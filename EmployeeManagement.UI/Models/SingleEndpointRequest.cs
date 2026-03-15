namespace EmployeeManagement.UI.Models
{
    public class SingleEndpointRequest
    {
        public string Url { get; set; } = "";
        public string? Method { get; set; } = "GET";
        public string? Body { get; set; }
    }
}
