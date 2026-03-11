namespace EmployeeManagement.UI.Services
{
    public class StudentService
    {
        public async Task UpdateStudentPhotoAsync(int studentId, string photoPath)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student != null)
            {
                student.PassportPhotoPath = photoPath;
                await _context.SaveChangesAsync();
            }
        }
    }
}
