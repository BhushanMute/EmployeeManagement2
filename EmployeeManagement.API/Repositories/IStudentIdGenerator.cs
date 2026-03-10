namespace EmployeeManagement.API.Repositories
{
    public interface IStudentIdGenerator
    {
        Task<string> GenerateNextIdAsync();
    }
}
