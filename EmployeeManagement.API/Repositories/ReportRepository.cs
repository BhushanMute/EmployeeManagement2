using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(IConfiguration config, ILogger<ReportRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection not found.");
            _logger = logger;
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        #region Employee Report

        public async Task<EmployeeReportData> GetEmployeeReport(
            int? departmentId, bool? isActive,
            DateTime? joiningFrom, DateTime? joiningTo, string? searchTerm)
        {
            var report = new EmployeeReportData();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetEmployeeReport", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", (object?)isActive ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@JoiningFrom", (object?)joiningFrom ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@JoiningTo", (object?)joiningTo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SearchTerm", (object?)searchTerm ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                // Summary
                if (await reader.ReadAsync())
                {
                    report.Summary = new EmployeeReportSummary
                    {
                        TotalEmployees = reader.GetInt32(reader.GetOrdinal("TotalEmployees")),
                        ActiveEmployees = reader.GetInt32(reader.GetOrdinal("ActiveEmployees")),
                        InactiveEmployees = reader.GetInt32(reader.GetOrdinal("InactiveEmployees")),
                        TotalDepartments = reader.GetInt32(reader.GetOrdinal("TotalDepartments")),
                        AverageSalary = reader.GetDecimal(reader.GetOrdinal("AverageSalary")),
                        TotalSalary = reader.GetDecimal(reader.GetOrdinal("TotalSalary")),
                        NewJoinersThisMonth = reader.GetInt32(reader.GetOrdinal("NewJoinersThisMonth")),
                        UpcomingBirthdays = reader.GetInt32(reader.GetOrdinal("UpcomingBirthdays"))
                    };
                }

                // Employees
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        report.Employees.Add(new EmployeeReportItem
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Email = reader["Email"]?.ToString(),
                            PhoneNumber = reader["PhoneNumber"]?.ToString(),
                            DepartmentName = reader["DepartmentName"]?.ToString(),
                            Role = reader["Role"]?.ToString(),
                            Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                            DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                            JoiningDate = reader.IsDBNull(reader.GetOrdinal("JoiningDate")) ? null : reader.GetDateTime(reader.GetOrdinal("JoiningDate")),
                            Address = reader["Address"]?.ToString(),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            ProfileImagePath = reader["ProfileImagePath"]?.ToString(),
                            YearsOfService = reader.IsDBNull(reader.GetOrdinal("YearsOfService")) ? 0 : reader.GetInt32(reader.GetOrdinal("YearsOfService")),
                            Age = reader.IsDBNull(reader.GetOrdinal("Age")) ? 0 : reader.GetInt32(reader.GetOrdinal("Age")),
                            LeavesThisYear = reader.GetInt32(reader.GetOrdinal("LeavesThisYear")),
                            LeaveDaysThisYear = reader.GetDecimal(reader.GetOrdinal("LeaveDaysThisYear"))
                        });
                    }
                }

                // Department Distribution
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        report.DepartmentDistribution.Add(new DepartmentDistribution
                        {
                            DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty,
                            EmployeeCount = reader.GetInt32(reader.GetOrdinal("EmployeeCount")),
                            AvgSalary = reader.GetDecimal(reader.GetOrdinal("AvgSalary")),
                            TotalSalary = reader.GetDecimal(reader.GetOrdinal("TotalSalary")),
                            ActiveCount = reader.GetInt32(reader.GetOrdinal("ActiveCount")),
                            InactiveCount = reader.GetInt32(reader.GetOrdinal("InactiveCount"))
                        });
                    }
                }

                // Role Distribution
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        report.RoleDistribution.Add(new RoleDistribution
                        {
                            RoleName = reader["RoleName"]?.ToString() ?? "Not Assigned",
                            EmployeeCount = reader.GetInt32(reader.GetOrdinal("EmployeeCount"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee report");
                throw;
            }

            return report;
        }

        #endregion

        #region Attendance Report

        public async Task<AttendanceReportData> GetAttendanceReport(
            int month, int year, int? departmentId, int? employeeId)
        {
            var report = new AttendanceReportData();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetAttendanceReport", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EmployeeId", (object?)employeeId ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                // Summary
                if (await reader.ReadAsync())
                {
                    report.Summary = new AttendanceReportSummary
                    {
                        TotalWorkingDays = reader.GetInt32(reader.GetOrdinal("TotalWorkingDays")),
                        HolidaysInMonth = reader.GetInt32(reader.GetOrdinal("HolidaysInMonth")),
                        ReportMonth = reader.GetInt32(reader.GetOrdinal("ReportMonth")),
                        ReportYear = reader.GetInt32(reader.GetOrdinal("ReportYear")),
                        MonthName = reader["MonthName"]?.ToString() ?? string.Empty
                    };
                }

                // Employee Attendance
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        report.Employees.Add(new EmployeeAttendanceItem
                        {
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            EmployeeName = reader["EmployeeName"]?.ToString() ?? string.Empty,
                            Email = reader["Email"]?.ToString(),
                            DepartmentName = reader["DepartmentName"]?.ToString(),
                            Role = reader["Role"]?.ToString(),
                            TotalWorkingDays = reader.GetInt32(reader.GetOrdinal("TotalWorkingDays")),
                            LeaveDays = reader.GetDecimal(reader.GetOrdinal("LeaveDays")),
                            PresentDays = reader.GetDecimal(reader.GetOrdinal("PresentDays")),
                            CasualLeave = reader.GetDecimal(reader.GetOrdinal("CasualLeave")),
                            SickLeave = reader.GetDecimal(reader.GetOrdinal("SickLeave")),
                            EarnedLeave = reader.GetDecimal(reader.GetOrdinal("EarnedLeave")),
                            LWP = reader.GetDecimal(reader.GetOrdinal("LWP")),
                            AttendancePercentage = reader.GetDecimal(reader.GetOrdinal("AttendancePercentage"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance report");
                throw;
            }

            return report;
        }

        #endregion

        #region Salary Report

        public async Task<SalaryReportData> GetSalaryReport(
            int? departmentId, int? month, int? year)
        {
            var report = new SalaryReportData();

            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand("sp_GetSalaryReport", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                // Summary
                if (await reader.ReadAsync())
                {
                    report.Summary = new SalaryReportSummary
                    {
                        TotalEmployees = reader.GetInt32(reader.GetOrdinal("TotalEmployees")),
                        TotalMonthlySalary = reader.GetDecimal(reader.GetOrdinal("TotalMonthlySalary")),
                        AverageSalary = reader.GetDecimal(reader.GetOrdinal("AverageSalary")),
                        HighestSalary = reader.GetDecimal(reader.GetOrdinal("HighestSalary")),
                        LowestSalary = reader.GetDecimal(reader.GetOrdinal("LowestSalary")),
                        WorkingDays = reader.GetInt32(reader.GetOrdinal("WorkingDays")),
                        MonthName = reader["MonthName"]?.ToString() ?? string.Empty,
                        ReportYear = reader.GetInt32(reader.GetOrdinal("ReportYear"))
                    };
                }

                // Employee Salary Details
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        report.Employees.Add(new EmployeeSalaryItem
                        {
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            EmployeeName = reader["EmployeeName"]?.ToString() ?? string.Empty,
                            Email = reader["Email"]?.ToString(),
                            DepartmentName = reader["DepartmentName"]?.ToString(),
                            Role = reader["Role"]?.ToString(),
                            MonthlySalary = reader.GetDecimal(reader.GetOrdinal("MonthlySalary")),
                            DailySalary = reader.GetDecimal(reader.GetOrdinal("DailySalary")),
                            WorkingDays = reader.GetInt32(reader.GetOrdinal("WorkingDays")),
                            LeaveDays = reader.GetDecimal(reader.GetOrdinal("LeaveDays")),
                            UnpaidLeaveDays = reader.GetDecimal(reader.GetOrdinal("UnpaidLeaveDays")),
                            PresentDays = reader.GetDecimal(reader.GetOrdinal("PresentDays")),
                            NetSalary = reader.GetDecimal(reader.GetOrdinal("NetSalary")),
                            DeductionAmount = reader.GetDecimal(reader.GetOrdinal("DeductionAmount")),
                            JoiningDate = reader.IsDBNull(reader.GetOrdinal("JoiningDate")) ? null : reader.GetDateTime(reader.GetOrdinal("JoiningDate"))
                        });
                    }
                }

                // Department Summary
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        report.DepartmentSummary.Add(new DepartmentSalarySummary
                        {
                            DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty,
                            EmployeeCount = reader.GetInt32(reader.GetOrdinal("EmployeeCount")),
                            TotalSalary = reader.GetDecimal(reader.GetOrdinal("TotalSalary")),
                            AvgSalary = reader.GetDecimal(reader.GetOrdinal("AvgSalary")),
                            MaxSalary = reader.GetDecimal(reader.GetOrdinal("MaxSalary")),
                            MinSalary = reader.GetDecimal(reader.GetOrdinal("MinSalary"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary report");
                throw;
            }

            return report;
        }

        #endregion
    }
}