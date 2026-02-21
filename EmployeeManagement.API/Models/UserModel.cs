namespace EmployeeManagement.API.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Provider { get; set; }

        public string GoogleId { get; set; }

        public string FacebookId { get; set; }
    }
}
