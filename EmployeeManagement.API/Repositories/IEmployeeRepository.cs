using EmployeeManagement.API.Models;

namespace EmployeeManagement.API.Repositories
{
    public interface IEmployeeRepository
    {
        // ========== Basic CRUD ==========
        Task<List<Employee>> GetAll();
        Task<Employee?> GetById(int id);
        Task<Employee?> GetByIdIncludeDeleted(int id);
        Task<Employee?> GetByEmail(string email);
        Task<int> Add(Employee emp);
        Task Update(Employee emp);
        Task Delete(int id);
        Task HardDelete(int id);
        Task<List<Employee>> GetAllFiltered(string? department = null, bool? isActive = null);

        // ========== Search & Filter ==========
        Task<List<Employee>> Search(string term);
        Task<List<Employee>> GetByDepartment(int departmentId);
        Task<List<Employee>> GetActive();
        Task<List<Employee>> GetInactive();
        Task<List<Employee>> GetDeleted();

        // ========== Pagination ==========
        Task<PagedResult<Employee>> GetAllPaged(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            int? departmentId = null,
            bool? isActive = null,
            string? sortBy = "Id",
            string? sortOrder = "ASC");

        // ========== Statistics ==========
        Task<int> GetTotalCount();
        Task<int> GetActiveCount();
        Task<int> GetInactiveCount();
        Task<int> GetDeletedCount();
        Task<int> GetCountByDepartment(int departmentId);

        // ========== Bulk Operations ==========
        Task<int> BulkInsert(List<Employee> employees);
        Task BulkUpdate(List<Employee> employees);
        Task BulkDelete(List<int> ids);

        // ========== Restore ==========
        Task Restore(int id);

        // ========== Department Summary ==========
        Task<List<DepartmentSummary>> GetDepartmentSummary();
    }

   
}