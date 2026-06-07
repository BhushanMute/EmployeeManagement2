using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
 

namespace EmployeeManagement.API.Salary
{
    /// <summary>
    /// Interface for Salary Structure Repository
    /// वेतन रचना रिपॉझिटरी इंटरफेस
    /// </summary>
    public interface ISalaryStructureRepository
    {
        // Salary Components
        Task<List<SalaryComponent>> GetAllSalaryComponentsAsync(bool activeOnly = true);
        Task<SalaryComponent?> GetSalaryComponentByIdAsync(int componentId);
        Task<SalaryComponent?> GetSalaryComponentByCodeAsync(string componentCode);
        Task<int> CreateSalaryComponentAsync(SalaryComponent component, int createdBy);
        Task<bool> UpdateSalaryComponentAsync(SalaryComponent component, int updatedBy);
        Task<bool> DeleteSalaryComponentAsync(int componentId, int deletedBy);

        // Salary Templates
        Task<List<SalaryTemplate>> GetAllTemplatesAsync(bool activeOnly = true);
        Task<SalaryTemplate?> GetTemplateByIdAsync(int templateId);
        Task<SalaryTemplate?> GetTemplateByCodeAsync(string templateCode);
        Task<int> CreateTemplateAsync(SalaryTemplate template, int createdBy);
        Task<bool> UpdateTemplateAsync(SalaryTemplate template, int updatedBy);
        Task<bool> DeleteTemplateAsync(int templateId, int deletedBy);

        // Template Components
        Task<List<SalaryTemplateComponent>> GetTemplateComponentsAsync(int templateId);
        Task<bool> AddTemplateComponentAsync(SalaryTemplateComponent component, int createdBy);
        Task<bool> UpdateTemplateComponentAsync(SalaryTemplateComponent component, int updatedBy);
        Task<bool> RemoveTemplateComponentAsync(int componentId);

        // Employee Salary Structure
        Task<EmployeeSalaryStructure?> GetEmployeeCurrentSalaryAsync(int employeeId);
        Task<List<EmployeeSalaryStructure>> GetEmployeeSalaryHistoryAsync(int employeeId);
        Task<EmployeeSalaryStructure?> GetSalaryStructureByIdAsync(int structureId);
        Task<int> AssignSalaryStructureAsync(AssignSalaryRequest request, int assignedBy);
        Task<bool> UpdateSalaryStructureAsync(EmployeeSalaryStructure structure, int updatedBy);
        Task<bool> RevokeSalaryStructureAsync(int structureId, int revokedBy);

        // Employee Salary Components
        Task<List<EmployeeSalaryComponent>> GetEmployeeSalaryComponentsAsync(int structureId);
        Task<SalaryStructureResponse> GetSalaryStructureDetailsAsync(int structureId);
        Task<bool> UpdateEmployeeComponentAsync(int componentId, decimal amount, int updatedBy);

        // Bulk Operations
        Task<bool> BulkAssignSalaryAsync(List<int> employeeIds, int templateId, DateTime effectiveFrom, int assignedBy);
        Task<List<EmployeeSalaryStructure>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary);
    }
}