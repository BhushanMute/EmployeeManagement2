namespace EmployeeManagement.UI.Models
{
    public class ApiEndpointTest
    {
        public string Name { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public bool RequiresAuth { get; set; }

        public ApiEndpointTest(string name, string method, string url, string category, bool requiresAuth)
        {
            Name = name;
            Method = method;
            Url = url;
            Category = category;
            RequiresAuth = requiresAuth;
        }
    }
}
