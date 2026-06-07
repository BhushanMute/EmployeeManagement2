using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;

namespace EmployeeManagement.API.Services
{
    /// <summary>
    /// Interface for Salary Structure Service
    /// वेतन रचना सेवा इंटरफेस
    /// </summary>
    public interface ISalaryStructureService
    {
        // Salary Components
        Task<ApiResponse<List<SalaryComponent>>> GetAllSalaryComponentsAsync(bool activeOnly = true);
        Task<ApiResponse<SalaryComponent>> GetSalaryComponentByIdAsync(int componentId);
        Task<ApiResponse<int>> CreateSalaryComponentAsync(SalaryComponent component, int userId);
        Task<ApiResponse<bool>> UpdateSalaryComponentAsync(SalaryComponent component, int userId);
        Task<ApiResponse<bool>> DeleteSalaryComponentAsync(int componentId, int userId);

        // Salary Templates
        Task<ApiResponse<List<SalaryTemplate>>> GetAllTemplatesAsync(bool activeOnly = true);
        Task<ApiResponse<SalaryTemplate>> GetTemplateByIdAsync(int templateId);
        Task<ApiResponse<List<SalaryTemplateComponent>>> GetTemplateComponentsAsync(int templateId);

        // Employee Salary Structure
        Task<ApiResponse<SalaryStructureResponse>> GetEmployeeCurrentSalaryAsync(int employeeId);
        Task<ApiResponse<List<SalaryStructureResponse>>> GetEmployeeSalaryHistoryAsync(int employeeId);
        Task<ApiResponse<int>> AssignSalaryToEmployeeAsync(AssignSalaryRequest request, int userId);
        Task<ApiResponse<SalaryStructureResponse>> GetSalaryStructureDetailsAsync(int structureId);
        Task<ApiResponse<bool>> UpdateSalaryComponentAsync(int componentId, decimal amount, int userId);

        // Bulk Operations
        Task<ApiResponse<bool>> BulkAssignSalaryAsync(List<int> employeeIds, int templateId, DateTime effectiveFrom, int userId);
    }
}