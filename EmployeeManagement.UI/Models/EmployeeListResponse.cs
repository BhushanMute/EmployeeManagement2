namespace EmployeeManagement.UI.Models
{
    public class EmployeeListResponse
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string CurrentTemplate { get; set; }
    }
}
