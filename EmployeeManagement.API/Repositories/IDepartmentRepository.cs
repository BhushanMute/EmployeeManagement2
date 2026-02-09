using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAll();
    }
}
