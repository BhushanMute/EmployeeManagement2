namespace EmployeeManagement.UI.Services
{
    public interface IStudentService
    {
        Task UpdateStudentPhotoAsync(int studentId, string photoPath);

    }
}
