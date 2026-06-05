using Dapper;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
using EmployeeManagement.API.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Salary
{
    /// <summary>
    /// Salary Structure Repository Implementation
    /// वेतन रचना रिपॉझिटरी (Dapper + Stored Procedures)
    /// </summary>
    public class SalaryStructureRepository : ISalaryStructureRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<SalaryStructureRepository> _logger;

        public SalaryStructureRepository(IDbConnectionFactory connectionFactory, ILogger<SalaryStructureRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        #region Salary Components

        /// <summary>
        /// Get all salary components
        /// सर्व वेतन घटक मिळवा
        /// </summary>
        public async Task<List<SalaryComponent>> GetAllSalaryComponentsAsync(bool activeOnly = true)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var components = await connection.QueryAsync<SalaryComponent>(
                    "sp_GetAllSalaryComponents",
                    new { ActiveOnly = activeOnly },
                    commandType: CommandType.StoredProcedure);

                return components.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all salary components");
                throw;
            }
        }

        /// <summary>
        /// Get salary component by ID
        /// </summary>
        public async Task<SalaryComponent?> GetSalaryComponentByIdAsync(int componentId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<SalaryComponent>(
                    "sp_GetSalaryComponentById",
                    new { ComponentId = componentId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary component by ID: {ComponentId}", componentId);
                throw;
            }
        }

        /// <summary>
        /// Get salary component by code
        /// </summary>
        public async Task<SalaryComponent?> GetSalaryComponentByCodeAsync(string componentCode)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<SalaryComponent>(
                    "sp_GetSalaryComponentByCode",
                    new { ComponentCode = componentCode },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary component by code: {ComponentCode}", componentCode);
                throw;
            }
        }

        /// <summary>
        /// Create new salary component
        /// </summary>
        public async Task<int> CreateSalaryComponentAsync(SalaryComponent component, int createdBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@ComponentCode", component.ComponentCode);
                parameters.Add("@ComponentName", component.ComponentName);
                parameters.Add("@ComponentType", component.ComponentType);
                parameters.Add("@Category", component.Category);
                parameters.Add("@CalculationType", component.CalculationType);
                parameters.Add("@CalculationBase", component.CalculationBase);
                parameters.Add("@DefaultPercentage", component.DefaultPercentage);
                parameters.Add("@DefaultAmount", component.DefaultAmount);
                parameters.Add("@DisplayOrder", component.DisplayOrder);
                parameters.Add("@IsStatutory", component.IsStatutory);
                parameters.Add("@IsTaxable", component.IsTaxable);
                parameters.Add("@FormulaExpression", component.FormulaExpression);
                parameters.Add("@MinAmount", component.MinAmount);
                parameters.Add("@MaxAmount", component.MaxAmount);
                parameters.Add("@Description", component.Description);
                parameters.Add("@Remarks", component.Remarks);
                parameters.Add("@CreatedBy", createdBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_CreateSalaryComponent",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Salary component created: {ComponentId} - {ComponentCode}", result, component.ComponentCode);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating salary component: {ComponentCode}", component.ComponentCode);
                throw;
            }
        }

        /// <summary>
        /// Update salary component
        /// </summary>
        public async Task<bool> UpdateSalaryComponentAsync(SalaryComponent component, int updatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@ComponentId", component.Id);
                parameters.Add("@ComponentName", component.ComponentName);
                parameters.Add("@ComponentType", component.ComponentType);
                parameters.Add("@Category", component.Category);
                parameters.Add("@CalculationType", component.CalculationType);
                parameters.Add("@CalculationBase", component.CalculationBase);
                parameters.Add("@DefaultPercentage", component.DefaultPercentage);
                parameters.Add("@DefaultAmount", component.DefaultAmount);
                parameters.Add("@DisplayOrder", component.DisplayOrder);
                parameters.Add("@IsStatutory", component.IsStatutory);
                parameters.Add("@IsTaxable", component.IsTaxable);
                parameters.Add("@FormulaExpression", component.FormulaExpression);
                parameters.Add("@MinAmount", component.MinAmount);
                parameters.Add("@MaxAmount", component.MaxAmount);
                parameters.Add("@Description", component.Description);
                parameters.Add("@Remarks", component.Remarks);
                parameters.Add("@IsActive", component.IsActive);
                parameters.Add("@UpdatedBy", updatedBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_UpdateSalaryComponent",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Salary component updated: {ComponentId}", component.Id);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating salary component: {ComponentId}", component.Id);
                throw;
            }
        }

        /// <summary>
        /// Delete (soft delete) salary component
        /// </summary>
        public async Task<bool> DeleteSalaryComponentAsync(int componentId, int deletedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_DeleteSalaryComponent",
                    new { ComponentId = componentId, DeletedBy = deletedBy },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Salary component deleted: {ComponentId}", componentId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting salary component: {ComponentId}", componentId);
                throw;
            }
        }

        #endregion

        #region Salary Templates

        /// <summary>
        /// Get all salary templates
        /// </summary>
        public async Task<List<SalaryTemplate>> GetAllTemplatesAsync(bool activeOnly = true)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var templates = await connection.QueryAsync<SalaryTemplate>(
                    "sp_GetAllSalaryTemplates",
                    new { ActiveOnly = activeOnly },
                    commandType: CommandType.StoredProcedure);

                return templates.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all salary templates");
                throw;
            }
        }

        /// <summary>
        /// Get salary template by ID
        /// </summary>
        public async Task<SalaryTemplate?> GetTemplateByIdAsync(int templateId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<SalaryTemplate>(
                    "sp_GetSalaryTemplateById",
                    new { TemplateId = templateId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary template by ID: {TemplateId}", templateId);
                throw;
            }
        }

        /// <summary>
        /// Get salary template by code
        /// </summary>
        public async Task<SalaryTemplate?> GetTemplateByCodeAsync(string templateCode)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<SalaryTemplate>(
                    "sp_GetSalaryTemplateByCode",
                    new { TemplateCode = templateCode },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary template by code: {TemplateCode}", templateCode);
                throw;
            }
        }

        /// <summary>
        /// Get template components
        /// </summary>
        public async Task<List<SalaryTemplateComponent>> GetTemplateComponentsAsync(int templateId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var components = await connection.QueryAsync<SalaryTemplateComponent>(
                    "sp_GetTemplateComponents",
                    new { TemplateId = templateId },
                    commandType: CommandType.StoredProcedure);

                return components.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting template components: {TemplateId}", templateId);
                throw;
            }
        }

        /// <summary>
        /// Create new salary template
        /// </summary>
        public async Task<int> CreateTemplateAsync(SalaryTemplate template, int createdBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@TemplateCode", template.TemplateCode);
                parameters.Add("@TemplateName", template.TemplateName);
                parameters.Add("@Description", template.Description);
                parameters.Add("@DepartmentId", template.DepartmentId);
                parameters.Add("@DesignationId", template.DesignationId);
                parameters.Add("@GradeLevel", template.GradeLevel);
                parameters.Add("@TotalCTC", template.TotalCTC);
                parameters.Add("@GrossSalary", template.GrossSalary);
                parameters.Add("@NetSalary", template.NetSalary);
                parameters.Add("@TotalEarnings", template.TotalEarnings);
                parameters.Add("@TotalDeductions", template.TotalDeductions);
                parameters.Add("@EmployerContributions", template.EmployerContributions);
                parameters.Add("@CreatedBy", createdBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_CreateSalaryTemplate",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Salary template created: {TemplateId} - {TemplateCode}", result, template.TemplateCode);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating salary template: {TemplateCode}", template.TemplateCode);
                throw;
            }
        }

        /// <summary>
        /// Update salary template
        /// </summary>
        public async Task<bool> UpdateTemplateAsync(SalaryTemplate template, int updatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@TemplateId", template.TemplateId);
                parameters.Add("@TemplateName", template.TemplateName);
                parameters.Add("@Description", template.Description);
                parameters.Add("@DepartmentId", template.DepartmentId);
                parameters.Add("@DesignationId", template.DesignationId);
                parameters.Add("@GradeLevel", template.GradeLevel);
                parameters.Add("@TotalCTC", template.TotalCTC);
                parameters.Add("@GrossSalary", template.GrossSalary);
                parameters.Add("@NetSalary", template.NetSalary);
                parameters.Add("@TotalEarnings", template.TotalEarnings);
                parameters.Add("@TotalDeductions", template.TotalDeductions);
                parameters.Add("@EmployerContributions", template.EmployerContributions);
                parameters.Add("@IsActive", template);
                parameters.Add("@UpdatedBy", updatedBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_UpdateSalaryTemplate",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Salary template updated: {TemplateId}", template.TemplateId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating salary template: {TemplateId}", template.TemplateId);
                throw;
            }
        }

        /// <summary>
        /// Delete (soft delete) salary template
        /// </summary>
        public async Task<bool> DeleteTemplateAsync(int templateId, int deletedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_DeleteSalaryTemplate",
                    new { TemplateId = templateId, DeletedBy = deletedBy },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Salary template deleted: {TemplateId}", templateId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting salary template: {TemplateId}", templateId);
                throw;
            }
        }

        /// <summary>
        /// Add component to template
        /// </summary>
        public async Task<bool> AddTemplateComponentAsync(SalaryTemplateComponent component, int createdBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@TemplateId", component.TemplateId);
                parameters.Add("@ComponentId", component.ComponentId);
                parameters.Add("@CalculationType", component.CalculationType);
                parameters.Add("@CalculationBase", component.CalculationBase);
                parameters.Add("@Percentage", component.Percentage);
                parameters.Add("@FixedAmount", component.FixedAmount);
                parameters.Add("@MonthlyAmount", component.MonthlyAmount);
                parameters.Add("@AnnualAmount", component.AnnualAmount);
                parameters.Add("@DisplayOrder", component.DisplayOrder);
                parameters.Add("@CreatedBy", createdBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_AddTemplateComponent",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Template component added: TemplateId={TemplateId}, ComponentId={ComponentId}",
                    component.TemplateId, component.ComponentId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding template component: TemplateId={TemplateId}", component.TemplateId);
                throw;
            }
        }

        /// <summary>
        /// Update template component
        /// </summary>
        public async Task<bool> UpdateTemplateComponentAsync(SalaryTemplateComponent component, int updatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@TemplateComponentId", component.Id);
                parameters.Add("@CalculationType", component.CalculationType);
                parameters.Add("@CalculationBase", component.CalculationBase);
                parameters.Add("@Percentage", component.Percentage);
                parameters.Add("@FixedAmount", component.FixedAmount);
                parameters.Add("@MonthlyAmount", component.MonthlyAmount);
                parameters.Add("@AnnualAmount", component.AnnualAmount);
                parameters.Add("@DisplayOrder", component.DisplayOrder);
                parameters.Add("@UpdatedBy", updatedBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_UpdateTemplateComponent",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Template component updated: {ComponentId}", component.Id);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating template component: {ComponentId}", component.Id);
                throw;
            }
        }

        /// <summary>
        /// Remove component from template
        /// </summary>
        public async Task<bool> RemoveTemplateComponentAsync(int componentId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_RemoveTemplateComponent",
                    new { TemplateComponentId = componentId },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Template component removed: {ComponentId}", componentId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing template component: {ComponentId}", componentId);
                throw;
            }
        }

        #endregion

        #region Employee Salary Structure

        /// <summary>
        /// Get employee's current salary structure
        /// कर्मचाऱ्याची सध्याची वेतन रचना मिळवा
        /// </summary>
        public async Task<EmployeeSalaryStructure?> GetEmployeeCurrentSalaryAsync(int employeeId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<EmployeeSalaryStructure>(
                    "sp_GetEmployeeCurrentSalary",
                    new { EmployeeId = employeeId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current salary structure for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        /// <summary>
        /// Get employee salary history
        /// </summary>
        public async Task<List<EmployeeSalaryStructure>> GetEmployeeSalaryHistoryAsync(int employeeId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var history = await connection.QueryAsync<EmployeeSalaryStructure>(
                    "sp_GetEmployeeSalaryHistory",
                    new { EmployeeId = employeeId },
                    commandType: CommandType.StoredProcedure);

                return history.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary history for employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        /// <summary>
        /// Get salary structure by ID
        /// </summary>
        public async Task<EmployeeSalaryStructure?> GetSalaryStructureByIdAsync(int structureId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                return await connection.QueryFirstOrDefaultAsync<EmployeeSalaryStructure>(
                    "sp_GetSalaryStructureById",
                    new { StructureId = structureId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary structure: {StructureId}", structureId);
                throw;
            }
        }

        /// <summary>
        /// Assign salary structure to employee using stored procedure
        /// कर्मचाऱ्याला वेतन रचना नियुक्त करा
        /// </summary>
        public async Task<int> AssignSalaryStructureAsync(AssignSalaryRequest request, int assignedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeId", request.EmployeeId);
                parameters.Add("@TemplateId", request.TemplateId);
                parameters.Add("@EffectiveFrom", request.EffectiveFrom);
                parameters.Add("@CTC", request.CTC);
                parameters.Add("@GrossSalary", request.GrossSalary);
                parameters.Add("@NetSalary", request.NetSalary);
                parameters.Add("@BasicSalary", request.BasicSalary);
                parameters.Add("@TotalEarnings", request.TotalEarnings);
                parameters.Add("@TotalDeductions", request.TotalDeductions);
                parameters.Add("@EmployerContributions", request.EmployerContributions);
                parameters.Add("@RevisionReason", request.RevisionReason);
                parameters.Add("@CreatedBy", assignedBy);

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "sp_AssignSalaryToEmployee",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                int structureId = result?.NewStructureId ?? 0;

                _logger.LogInformation("Salary structure assigned: StructureId={StructureId}, EmployeeId={EmployeeId}",
                    structureId, request.EmployeeId);

                return structureId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning salary structure to employee: {EmployeeId}", request.EmployeeId);
                throw;
            }
        }

        /// <summary>
        /// Get employee salary components
        /// </summary>
        public async Task<List<EmployeeSalaryComponent>> GetEmployeeSalaryComponentsAsync(int structureId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var components = await connection.QueryAsync<EmployeeSalaryComponent>(
                    "sp_GetEmployeeSalaryComponents",
                    new { StructureId = structureId },
                    commandType: CommandType.StoredProcedure);

                return components.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee salary components: {StructureId}", structureId);
                throw;
            }
        }

        /// <summary>
        /// Get detailed salary structure (including components breakdown)
        /// संपूर्ण वेतन रचना तपशील मिळवा
        /// </summary>
        public async Task<SalaryStructureResponse> GetSalaryStructureDetailsAsync(int structureId)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                using var multi = await connection.QueryMultipleAsync(
                    "sp_GetSalaryStructureDetails",
                    new { StructureId = structureId },
                    commandType: CommandType.StoredProcedure);

                var structure = await multi.ReadFirstOrDefaultAsync<dynamic>();
                var components = (await multi.ReadAsync<SalaryComponentDetailResponse>()).ToList();

                var response = new SalaryStructureResponse
                {
                    Id = structure.Id,
                    EmployeeId = structure.EmployeeId,
                    EmployeeName = structure.EmployeeName,
                    EmployeeCode = structure.EmployeeCode,
                    DepartmentName = structure.DepartmentName,
                    TemplateName = structure.TemplateName,
                    TemplateCode = structure.TemplateCode,
                    CTC = structure.CTC,
                    GrossSalary = structure.GrossSalary,
                    NetSalary = structure.NetSalary,
                    BasicSalary = structure.BasicSalary,
                    EffectiveFrom = structure.EffectiveFrom,
                    EffectiveTo = structure.EffectiveTo,
                    IsCurrentStructure = structure.IsCurrentStructure,
                    RevisionNumber = structure.RevisionNumber,
                    RevisionReason = (string?)structure.RevisionReason,

                    Earnings = components
                        .Where(c => string.Equals(c.ComponentType, "Earning", StringComparison.OrdinalIgnoreCase))
                        .ToList(),

                    Deductions = components
                        .Where(c => string.Equals(c.ComponentType, "Deduction", StringComparison.OrdinalIgnoreCase))
                        .ToList(),

                    TotalEarnings = components
                            .Where(c => string.Equals(c.ComponentType, "Earning", StringComparison.OrdinalIgnoreCase))
                            .Sum(c => c.MonthlyAmount ?? 0),

                                            TotalDeductions = components
                            .Where(c => string.Equals(c.ComponentType, "Deduction", StringComparison.OrdinalIgnoreCase))
                            .Sum(c => c.MonthlyAmount ?? 0),

                    CreatedDate = structure.CreatedDate
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary structure details: {StructureId}", structureId);
                throw;
            }
        }

        /// <summary>
        /// Update individual component amount
        /// </summary>
        public async Task<bool> UpdateEmployeeComponentAsync(int componentId, decimal amount, int updatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_UpdateEmployeeComponent",
                    new { ComponentId = componentId, Amount = amount, UpdatedBy = updatedBy },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Employee component updated: {ComponentId}, Amount: {Amount}", componentId, amount);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee component: {ComponentId}", componentId);
                throw;
            }
        }

        /// <summary>
        /// Update salary structure
        /// </summary>
        public async Task<bool> UpdateSalaryStructureAsync(EmployeeSalaryStructure structure, int updatedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@StructureId", structure.Id);
                parameters.Add("@CTC", structure.CTC);
                parameters.Add("@GrossSalary", structure.GrossSalary);
                parameters.Add("@NetSalary", structure.NetSalary);
                parameters.Add("@BasicSalary", structure.BasicSalary);
                parameters.Add("@TotalEarnings", structure.TotalEarnings);
                parameters.Add("@TotalDeductions", structure.TotalDeductions);
                parameters.Add("@EmployerContributions", structure.EmployerContributions);
                parameters.Add("@UpdatedBy", updatedBy);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_UpdateSalaryStructure",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Salary structure updated: {StructureId}", structure.Id);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating salary structure: {StructureId}", structure.Id);
                throw;
            }
        }

        /// <summary>
        /// Revoke salary structure
        /// </summary>
        public async Task<bool> RevokeSalaryStructureAsync(int structureId, int revokedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_RevokeSalaryStructure",
                    new { StructureId = structureId, RevokedBy = revokedBy },
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Salary structure revoked: {StructureId}", structureId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking salary structure: {StructureId}", structureId);
                throw;
            }
        }

        #endregion

        #region Bulk Operations

        /// <summary>
        /// Bulk assign salary to multiple employees
        /// </summary>
        public async Task<bool> BulkAssignSalaryAsync(List<int> employeeIds, int templateId, DateTime effectiveFrom, int assignedBy)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var employeeIdsString = string.Join(",", employeeIds);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_BulkAssignSalary",
                    new
                    {
                        EmployeeIds = employeeIdsString,
                        TemplateId = templateId,
                        EffectiveFrom = effectiveFrom,
                        AssignedBy = assignedBy
                    },
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 300);

                _logger.LogInformation("Bulk salary assignment completed for {Count} employees", employeeIds.Count);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk salary assignment");
                throw;
            }
        }

        /// <summary>
        /// Get employees by salary range
        /// </summary>
        public async Task<List<EmployeeSalaryStructure>> GetEmployeesBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var structures = await connection.QueryAsync<EmployeeSalaryStructure>(
                    "sp_GetEmployeesBySalaryRange",
                    new { MinSalary = minSalary, MaxSalary = maxSalary },
                    commandType: CommandType.StoredProcedure);

                return structures.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees by salary range");
                throw;
            }
        }

        #endregion

        #region Salary Calculation Helpers

        /// <summary>
        /// Calculate salary breakdown based on template and CTC
        /// </summary>
        public async Task<List<SalaryComponentDetailResponse>> CalculateSalaryBreakdownAsync(int templateId, decimal ctc)
        {
            try
            {
                using var connection = (SqlConnection)_connectionFactory.CreateConnection();

                var breakdown = await connection.QueryAsync<SalaryComponentDetailResponse>(
                    "sp_CalculateSalaryBreakdown",
                    new { TemplateId = templateId, CTC = ctc },
                    commandType: CommandType.StoredProcedure);

                return breakdown.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating salary breakdown for template: {TemplateId}", templateId);
                throw;
            }
        }

        #endregion
    }
}