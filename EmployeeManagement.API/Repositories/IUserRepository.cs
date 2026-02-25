using EmployeeManagement.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Repositories
{
    public interface IUserRepository
    {
        //Task<(UserModel? user, List<string> roles, string message)> LoginAsync(string username, string password);
        //Task<int> RegisterAsync(RegisterRequest model);
        //Task<UserModel?> GoogleLogin(string email, string googleId);
        //Task<UserModel?> SocialLogin(string email, string provider, string? socialId = null);
        //Task SaveRefreshTokenAsync(int userId, string refreshToken);

        //Task<UserModel?> GetUserByRefreshTokenAsync(string refreshToken);
        //Task<User?> GetUserByEmailAsync(string email);
        //Task<List<string>> GetRolesAsync(int userId);
        //Task<User?> GetByIdAsync(int id);
        //Task<(List<User> Users, int TotalRecords)> GetAllAsync(int pageNumber, int pageSize,
        //    string? searchTerm, int? roleId, bool? isActive);
        //Task<bool> UpdateAsync(User user);
        //Task<bool> DeleteAsync(int id, int deletedBy);
        //Task<bool> AssignRoleAsync(int userId, int roleId, int assignedBy);
        //Task<bool> RemoveRoleAsync(int userId, int roleId);

        //Task<(UserModel?, List<string>, string)> LoginAsync(string username, string password);
          Task<(UserModel?, List<string>, string)> LoginAsync(string username, string password);
        Task<int> RegisterAsync(RegisterRequest model);
        (int UserId, string Username) Login(string username, string passwordHash);
        List<string> GetRoles(int userId);
        Task<UserModel?> GoogleLogin(string email, string googleId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<UserModel?> SocialLogin(string email, string provider, string socialId);

           Task SaveRefreshTokenAsync(int userId, string refreshToken);
        Task<UserModel?> GetUserByRefreshTokenAsync(string refreshToken);
        Task<List<string>> GetRolesAsync(int userId);
    }

}
 
