using EmployeeManagement.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Repositories
{
    public interface IUserRepository
    {
        Task<(UserModel? user, List<string> roles, string message)> LoginAsync(string username, string password);
        Task<int> RegisterAsync(RegisterRequest model);
    }
}
 
