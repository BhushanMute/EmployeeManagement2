namespace EmployeeManagement.API.Models
{
    public class UserListResponse
    {
        public List<UserListItem> Users { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
