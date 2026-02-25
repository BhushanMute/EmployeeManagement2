namespace EmployeeManagement.API.services
{
    public interface IPasswordService
    {
        (string Hash, string Salt) HashPassword(string password);
        bool VerifyPassword(string password, string hash, string salt);
    }
}
