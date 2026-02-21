namespace EmployeeManagement.UI.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task RemoveTokenAsync();
        Task<string> GetRefreshTokenAsync();
        Task SetRefreshTokenAsync(string token);
        bool IsTokenExpired(string token);
    }
}