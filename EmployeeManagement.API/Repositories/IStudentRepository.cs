using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByStudentIdAsync(string studentId);
        Task<int> AddAsync(Student student);
        Task<bool> UpdateAsync(Student student);
        Task<bool> DeleteAsync(int id);
        Task<List<Student>> SearchAsync(string searchTerm);
        Task<List<Student>> GetByClassAsync(string className);
    }
}
