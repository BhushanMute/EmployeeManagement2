using EmployeeManagement.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Repositories
{
    public interface IUserRepository
    {
        Task<(UserModel? user, List<string> roles, string message)> LoginAsync(string username, string password);
        Task<int> RegisterAsync(RegisterRequest model);
        Task<UserModel?> GoogleLogin(string email, string googleId);
        Task<UserModel?> SocialLogin(string email, string provider, string? socialId = null);
        Task SaveRefreshTokenAsync(int userId, string refreshToken);

        Task<UserModel?> GetUserByRefreshTokenAsync(string refreshToken);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<string>> GetRolesAsync(int userId);
    }

}
 
