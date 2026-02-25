using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace EmployeeManagement.API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(IConfiguration config, ILogger<EmployeeRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found in configuration.");
            _logger = logger;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region Basic CRUD

        public async Task<List<Employee>> GetAll()
        {
            var employees = new List<Employee>();

            try
            {
                using SqlConnection con = GetConnection();
                using SqlCommand cmd = new SqlCommand("sp_GetAllEmployees", con);
                cmd.CommandType = CommandType.StoredProcedure;

                await con.OpenAsync();
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(MapEmployee(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all employees");
                throw;
            }

            return employees;
        }

        public async Task<Employee?> GetById(int id)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapEmployee(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee by id: {Id}", id);
                throw;
            }

            return null;
        }

        public async Task<Employee?> GetByIdIncludeDeleted(int id)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeByIdIncludeDeleted", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapEmployee(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee by id (include deleted): {Id}", id);
                throw;
            }

            return null;
        }

        public async Task<Employee?> GetByEmail(string email)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeByEmail", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapEmployee(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee by email: {Email}", email);
                throw;
            }

            return null;
        }

        public async Task<int> Add(Employee emp)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_AddEmployee", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", emp.Name);
                cmd.Parameters.AddWithValue("@Email", emp.Email);
                cmd.Parameters.AddWithValue("@DepartmentId", emp.DepartmentId);
                cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object?)emp.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object?)emp.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DateOfBirth", (object?)emp.DateOfBirth ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@JoiningDate", (object?)emp.JoiningDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Role", (object?)emp.Role ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProfileImagePath", (object?)emp.ProfileImagePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", emp.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)emp.CreatedBy ?? DBNull.Value);

                var returnParam = new SqlParameter("@NewId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(returnParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return (int)returnParam.Value;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                _logger.LogWarning("Duplicate email attempt: {Email}", emp.Email);
                throw new Exception("Email already exists. Please use a different email.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding employee");
                throw;
            }
        }

        public async Task Update(Employee emp)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_UpdateEmployee", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", emp.Id);
                cmd.Parameters.AddWithValue("@Name", emp.Name);
                cmd.Parameters.AddWithValue("@Email", emp.Email);
                cmd.Parameters.AddWithValue("@DepartmentId", emp.DepartmentId);
                cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object?)emp.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object?)emp.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Role", (object?)emp.Role ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProfileImagePath", (object?)emp.ProfileImagePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", emp.IsActive);
                cmd.Parameters.AddWithValue("@IsDeleted", emp.IsDeleted);
                cmd.Parameters.AddWithValue("@UpdatedBy", (object?)emp.UpdatedBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DeletedBy", (object?)emp.DeletedBy ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee: {Id}", emp.Id);
                throw;
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_DeleteEmployee", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee: {Id}", id);
                throw;
            }
        }

        public async Task HardDelete(int id)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_HardDeleteEmployee", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hard deleting employee: {Id}", id);
                throw;
            }
        }

        #endregion

        #region Search & Filter

        public async Task<List<Employee>> Search(string term)
        {
            var employees = new List<Employee>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_SearchEmployees", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SearchTerm", term);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(MapEmployee(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching employees with term: {Term}", term);
                throw;
            }

            return employees;
        }

        public async Task<List<Employee>> GetByDepartment(int departmentId)
        {
            var employees = new List<Employee>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeesByDepartment", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(MapEmployee(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees by department: {DepartmentId}", departmentId);
                throw;
            }

            return employees;
        }

        public async Task<List<Employee>> GetActive()
        {
            var employees = new List<Employee>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetActiveEmployees", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(MapEmployee(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active employees");
                throw;
            }

            return employees;
        }

        public async Task<List<Employee>> GetInactive()
        {
            var employees = new List<Employee>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetInactiveEmployees", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(MapEmployee(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inactive employees");
                throw;
            }

            return employees;
        }

        public async Task<List<Employee>> GetDeleted()
        {
            var employees = new List<Employee>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetDeletedEmployees", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(MapEmployee(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting deleted employees");
                throw;
            }

            return employees;
        }

        #endregion

        #region Pagination

        public async Task<PagedResult<Employee>> GetAllPaged( int pageNumber, int pageSize, string? searchTerm = null, int? departmentId = null, bool? isActive = null, string? sortBy = "Id", string? sortOrder = "ASC")
        {
            var result = new PagedResult<Employee>
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeesPaged", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@SearchTerm", (object?)searchTerm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", (object?)isActive ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SortBy", sortBy ?? "Id");
                cmd.Parameters.AddWithValue("@SortOrder", sortOrder ?? "ASC");

                var totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(totalRecordsParam);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Items.Add(MapEmployee(reader));
                }

                await reader.CloseAsync();
                result.TotalRecords = (int)totalRecordsParam.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged employees");
                throw;
            }

            return result;
        }

        #endregion

        #region Statistics

        public async Task<int> GetTotalCount()
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeTotalCount", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                return (int)(await cmd.ExecuteScalarAsync() ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total count");
                throw;
            }
        }

        public async Task<int> GetActiveCount()
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeActiveCount", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                return (int)(await cmd.ExecuteScalarAsync() ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active count");
                throw;
            }
        }

        public async Task<int> GetInactiveCount()
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeInactiveCount", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                return (int)(await cmd.ExecuteScalarAsync() ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inactive count");
                throw;
            }
        }

        public async Task<int> GetDeletedCount()
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeDeletedCount", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                return (int)(await cmd.ExecuteScalarAsync() ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting deleted count");
                throw;
            }
        }

        public async Task<int> GetCountByDepartment(int departmentId)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeCountByDepartment", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                await conn.OpenAsync();
                return (int)(await cmd.ExecuteScalarAsync() ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting count by department: {DepartmentId}", departmentId);
                throw;
            }
        }

        #endregion

        #region Bulk Operations

        public async Task<int> BulkInsert(List<Employee> employees)
        {
            int successCount = 0;

            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                using var transaction = conn.BeginTransaction();
                try
                {
                    foreach (var emp in employees)
                    {
                        using var cmd = new SqlCommand("sp_AddEmployee", conn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Name", emp.Name);
                        cmd.Parameters.AddWithValue("@Email", emp.Email);
                        cmd.Parameters.AddWithValue("@DepartmentId", emp.DepartmentId);
                        cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                        cmd.Parameters.AddWithValue("@PhoneNumber", (object?)emp.PhoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object?)emp.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateOfBirth", (object?)emp.DateOfBirth ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@JoiningDate", (object?)emp.JoiningDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Role", (object?)emp.Role ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProfileImagePath", (object?)emp.ProfileImagePath ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", emp.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object?)emp.CreatedBy ?? DBNull.Value);

                        cmd.Parameters.Add(new SqlParameter("@NewId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        });

                        await cmd.ExecuteNonQueryAsync();
                        successCount++;
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk insert");
                throw;
            }

            return successCount;
        }

        public async Task BulkUpdate(List<Employee> employees)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                using var transaction = conn.BeginTransaction();
                try
                {
                    foreach (var emp in employees)
                    {
                        using var cmd = new SqlCommand("sp_UpdateEmployee", conn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", emp.Id);
                        cmd.Parameters.AddWithValue("@Name", emp.Name);
                        cmd.Parameters.AddWithValue("@Email", emp.Email);
                        cmd.Parameters.AddWithValue("@DepartmentId", emp.DepartmentId);
                        cmd.Parameters.AddWithValue("@Salary", emp.Salary);
                        cmd.Parameters.AddWithValue("@PhoneNumber", (object?)emp.PhoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object?)emp.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Role", (object?)emp.Role ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProfileImagePath", (object?)emp.ProfileImagePath ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", emp.IsActive);
                        cmd.Parameters.AddWithValue("@IsDeleted", emp.IsDeleted);
                        cmd.Parameters.AddWithValue("@UpdatedBy", (object?)emp.UpdatedBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DeletedBy", (object?)emp.DeletedBy ?? DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk update");
                throw;
            }
        }

        public async Task BulkDelete(List<int> ids)
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();

                using var transaction = conn.BeginTransaction();
                try
                {
                    foreach (var id in ids)
                    {
                        using var cmd = new SqlCommand("sp_DeleteEmployee", conn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);

                        await cmd.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk delete");
                throw;
            }
        }

        #endregion

        #region Restore

        public async Task Restore(int id)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_RestoreEmployee", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring employee: {Id}", id);
                throw;
            }
        }

        #endregion

        #region Department Summary

        public async Task<List<DepartmentSummary>> GetDepartmentSummary()
        {
            var summaries = new List<DepartmentSummary>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetDepartmentSummary", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    summaries.Add(new DepartmentSummary
                    {
                        DepartmentId = reader.GetInt32("DepartmentId"),
                        DepartmentName = reader.GetString("DepartmentName"),
                        EmployeeCount = reader.GetInt32("EmployeeCount"),
                        TotalSalary = reader.GetDecimal("TotalSalary"),
                        AverageSalary = reader.GetDecimal("AverageSalary")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting department summary");
                throw;
            }

            return summaries;
        }

        public async Task<List<Employee>> GetAllFiltered(string? department = null, bool? isActive = null)
        {
            var employees = new List<Employee>();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeesFiltered", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Department", (object?)department ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", (object?)isActive ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(MapEmployee(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered employees");
                throw;
            }

            return employees;
        }
        #endregion

        #region Helper Methods

        private Employee MapEmployee(SqlDataReader reader)
        {
            return new Employee
            {
                Id = reader.GetInt32("Id"),
                Name = reader["Name"].ToString() ?? string.Empty,
                Email = reader["Email"].ToString() ?? string.Empty,
                DepartmentId = reader.GetInt32("DepartmentId"),
                DepartmentName = reader["DepartmentName"]?.ToString(),
                Salary = reader.GetDecimal("Salary"),
                PhoneNumber = reader["PhoneNumber"]?.ToString(),
                Address = reader["Address"]?.ToString(),
                DateOfBirth = reader["DateOfBirth"] == DBNull.Value ? null : reader.GetDateTime("DateOfBirth"),
                JoiningDate = reader["JoiningDate"] == DBNull.Value ? null : reader.GetDateTime("JoiningDate"),
                ProfileImagePath = reader["ProfileImagePath"]?.ToString(),
                Role = reader["Role"]?.ToString(),
                IsActive = reader.GetBoolean("IsActive"),
                IsDeleted = reader.GetBoolean("IsDeleted"),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : reader.GetInt32("CreatedBy"),
                CreatedDate = reader.GetDateTime("CreatedDate"),
                UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : reader.GetInt32("UpdatedBy"),
                UpdatedDate = reader["UpdatedDate"] == DBNull.Value ? null : reader.GetDateTime("UpdatedDate"),
                DeletedBy = reader["DeletedBy"] == DBNull.Value ? null : reader.GetInt32("DeletedBy"),
                DeletedDate = reader["DeletedDate"] == DBNull.Value ? null : reader.GetDateTime("DeletedDate")
            };
        }

        #endregion
    }
}