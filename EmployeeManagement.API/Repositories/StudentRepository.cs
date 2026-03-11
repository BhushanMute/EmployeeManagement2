using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;
 
namespace EmployeeManagement.API.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(IConfiguration config, ILogger<StudentRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found");
            _logger = logger;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            var students = new List<Student>();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetAllStudents", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    students.Add(MapStudent(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all students");
                throw;
            }

            return students;
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetStudentById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapStudent(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student by ID: {Id}", id);
                throw;
            }

            return null;
        }

        public async Task<Student?> GetByStudentIdAsync(string studentId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetStudentByStudentId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentId", studentId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapStudent(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student by StudentId: {StudentId}", studentId);
                throw;
            }

            return null;
        }

        public async Task<int> AddAsync(Student student)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_AddStudent", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StudentId", student.StudentId);
                cmd.Parameters.AddWithValue("@FirstName", student.FirstName);
                cmd.Parameters.AddWithValue("@LastName", student.LastName);
                cmd.Parameters.AddWithValue("@FullName", student.FullName);
                cmd.Parameters.AddWithValue("@Class", student.Class);
                cmd.Parameters.AddWithValue("@Subjects", (object?)student.Subjects ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Age", (object?)student.Age ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DateOfBirth", (object?)student.DateOfBirth ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@JoiningDate", student.JoiningDate);
                cmd.Parameters.AddWithValue("@BatchTime", (object?)student.BatchTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PassportPhotoPath", (object?)student.PassportPhotoPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object?)student.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)student.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object?)student.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ParentName", (object?)student.ParentName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ParentPhone", (object?)student.ParentPhone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ParentEmail", (object?)student.ParentEmail ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)student.CreatedBy ?? DBNull.Value);

                var outputParam = new SqlParameter("@NewId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)outputParam.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding student");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_UpdateStudent", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", student.Id);
                cmd.Parameters.AddWithValue("@FirstName", student.FirstName);
                cmd.Parameters.AddWithValue("@LastName", student.LastName);
                cmd.Parameters.AddWithValue("@FullName", student.FullName);
                cmd.Parameters.AddWithValue("@Class", student.Class);
                cmd.Parameters.AddWithValue("@Subjects", (object?)student.Subjects ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Age", (object?)student.Age ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DateOfBirth", (object?)student.DateOfBirth ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BatchTime", (object?)student.BatchTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object?)student.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)student.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object?)student.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ParentName", (object?)student.ParentName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ParentPhone", (object?)student.ParentPhone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ParentEmail", (object?)student.ParentEmail ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", student.IsActive);
                cmd.Parameters.AddWithValue("@UpdatedBy", (object?)student.UpdatedBy ?? DBNull.Value);

                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student: {Id}", student.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_DeleteStudent", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@DeletedBy", DBNull.Value);

                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student: {Id}", id);
                throw;
            }
        }

        public async Task<List<Student>> SearchAsync(string searchTerm)
        {
            var students = new List<Student>();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_SearchStudents", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    students.Add(MapStudent(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching students");
                throw;
            }

            return students;
        }

        public async Task<List<Student>> GetByClassAsync(string className)
        {
            var students = new List<Student>();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetStudentsByClass", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Class", className);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    students.Add(MapStudent(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting students by class");
                throw;
            }

            return students;
        }

        private Student MapStudent(SqlDataReader reader)
        {
            return new Student
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader.GetString("StudentId"),
                FirstName = reader.GetString("FirstName"),
                LastName = reader.GetString("LastName"),
                FullName = reader.GetString("FullName"),
                Class = reader.GetString("Class"),
                Subjects = reader.GetNullableString("Subjects"),
                Age = reader.GetNullableInt32("Age"),
                DateOfBirth = reader.GetNullableDateTime("DateOfBirth"),
                JoiningDate = reader.GetDateTime("JoiningDate"),
                BatchTime = reader.GetNullableString("BatchTime"),
                PassportPhotoPath = reader.GetNullableString("PassportPhotoPath"),
                PhoneNumber = reader.GetNullableString("PhoneNumber"),
                Email = reader.GetNullableString("Email"),
                Address = reader.GetNullableString("Address"),
                ParentName = reader.GetNullableString("ParentName"),
                ParentPhone = reader.GetNullableString("ParentPhone"),
                ParentEmail = reader.GetNullableString("ParentEmail"),
                IsActive = reader.GetBoolean("IsActive"),
                IsDeleted = reader.GetBoolean("IsDeleted"),
                CreatedBy = reader.GetNullableInt32("CreatedBy"),
                CreatedDate = reader.GetDateTime("CreatedDate"),
                UpdatedBy = reader.GetNullableInt32("UpdatedBy"),
                UpdatedDate = reader.GetNullableDateTime("UpdatedDate")
            };
        }
    }
}
