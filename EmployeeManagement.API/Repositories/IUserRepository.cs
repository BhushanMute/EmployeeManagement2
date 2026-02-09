using EmployeeManagement.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Repositories
{
    public interface IUserRepository
    {
        Task<(UserModel? user, string message)> LoginAsync(string username, string password);
        Task<int> RegisterAsync(RegisterRequest model);
    }
}
 
