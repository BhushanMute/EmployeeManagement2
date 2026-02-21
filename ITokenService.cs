namespace EmployeeManagement.UI.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task RemoveTokenAsync();
        bool IsTokenExpired(string token);
    }
}