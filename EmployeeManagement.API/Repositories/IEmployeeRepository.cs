using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAll();
        Task<Employee?> GetById(int id);
        Task Add(Employee emp);
        Task Update(Employee emp);
        Task Delete(int id);
         
    }
}
