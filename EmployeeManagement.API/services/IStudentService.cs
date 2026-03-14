namespace EmployeeManagement.API.Services
{
    public interface IStudentService
    {
        Task UpdateStudentPhotoAsync(int studentId, string photoPath);

    }
}
