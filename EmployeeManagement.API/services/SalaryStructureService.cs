using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Models.Payroll;
 using EmployeeManagement.API.Salary;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services.Interfaces;

namespace EmployeeManagement.API.Services
{
    /// <summary>
    /// Salary Structure Service Implementation
    /// वेतन रचना सेवा अंमलबजावणी
    /// </summary>
    public class SalaryStructureService : ISalaryStructureService
    {
        private readonly ISalaryStructureRepository _salaryStructureRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<SalaryStructureService> _logger;

        public SalaryStructureService(
            ISalaryStructureRepository salaryStructureRepository,
            IAuditService auditService,
            ILogger<SalaryStructureService> logger)
        {
            _salaryStructureRepository = salaryStructureRepository;
            _auditService = auditService;
            _logger = logger;
        }

        #region Salary Components

        /// <summary>
        /// Get all salary components
        /// सर्व वेतन घटक मिळवा
        /// </summary>
        public async Task<ApiResponse<List<SalaryComponent>>> GetAllSalaryComponentsAsync(bool activeOnly = true)
        {
            try
            {
                var components = await _salaryStructureRepository.GetAllSalaryComponentsAsync(activeOnly);
                return ApiResponse<List<SalaryComponent>>.Success(components);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all salary components");
                return ApiResponse<List<SalaryComponent>>.Fail("An error occurred while fetching salary components");
            }
        }

        /// <summary>
        /// Get salary component by ID
        /// </summary>
        public async Task<ApiResponse<SalaryComponent>> GetSalaryComponentByIdAsync(int componentId)
        {
            try
            {
                var component = await _salaryStructureRepository.GetSalaryComponentByIdAsync(componentId);

                if (component == null)
                {
                    return ApiResponse<SalaryComponent>.Fail("Salary component not found");
                }

                return ApiResponse<SalaryComponent>.Success(component);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary component: {ComponentId}", componentId);
                return ApiResponse<SalaryComponent>.Fail("An error occurred while fetching salary component");
            }
        }

        /// <summary>
        /// Create new salary component
        /// नवीन वेतन घटक तयार करा
        /// </summary>
        public async Task<ApiResponse<int>> CreateSalaryComponentAsync(SalaryComponent component, int userId)
        {
            try
            {
                // Validate component code uniqueness
                var existingComponent = await _salaryStructureRepository.GetSalaryComponentByCodeAsync(component.ComponentCode);
                if (existingComponent != null)
                {
                    return ApiResponse<int>.Fail($"Component with code '{component.ComponentCode}' already exists");
                }

                // Validate calculation type
                if (component.CalculationType == "Percentage" && string.IsNullOrEmpty(component.CalculationBase))
                {
                    return ApiResponse<int>.Fail("Calculation base is required for percentage-based components");
                }

                var componentId = await _salaryStructureRepository.CreateSalaryComponentAsync(component, userId);

                // Audit log
                await _auditService.LogAsync(
                    userId,
                    "CREATE",
                    "SalaryComponent",
                    componentId,
                    null,
                    $"Component: {component.ComponentCode} - {component.ComponentName}"
                );

                _logger.LogInformation("Salary component created: {ComponentId} - {ComponentCode} by user {UserId}",
                    componentId, component.ComponentCode, userId);

                return ApiResponse<int>.Success(componentId, "Salary component created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating salary component: {ComponentCode}", component.ComponentCode);
                return ApiResponse<int>.Fail("An error occurred while creating salary component");
            }
        }

        /// <summary>
        /// Update salary component
        /// </summary>
        public async Task<ApiResponse<bool>> UpdateSalaryComponentAsync(SalaryComponent component, int userId)
        {
            try
            {
                var existingComponent = await _salaryStructureRepository.GetSalaryComponentByIdAsync(component.Id);
                if (existingComponent == null)
                {
                    return ApiResponse<bool>.Fail("Salary component not found");
                }

                var result = await _salaryStructureRepository.UpdateSalaryComponentAsync(component, userId);

                if (result)
                {
                    // Audit log
                    await _auditService.LogAsync(
                        userId,
                        "UPDATE",
                        "SalaryComponent",
                        component.Id,
                        $"{existingComponent.ComponentName}",
                        $"{component.ComponentName}"
                    );

                    _logger.LogInformation("Salary component updated: {ComponentId} by user {UserId}", component.Id, userId);
                }

                return ApiResponse<bool>.Success(result, "Salary component updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating salary component: {ComponentId}", component.Id);
                return ApiResponse<bool>.Fail("An error occurred while updating salary component");
            }
        }

        /// <summary>
        /// Delete salary component
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteSalaryComponentAsync(int componentId, int userId)
        {
            try
            {
                var component = await _salaryStructureRepository.GetSalaryComponentByIdAsync(componentId);
                if (component == null)
                {
                    return ApiResponse<bool>.Fail("Salary component not found");
                }

                var result = await _salaryStructureRepository.DeleteSalaryComponentAsync(componentId, userId);

                if (result)
                {
                    // Audit log
                    await _auditService.LogAsync(
                        userId,
                        "DELETE",
                        "SalaryComponent",
                        componentId,
                        $"{component.ComponentCode} - {component.ComponentName}",
                        null
                    );

                    _logger.LogInformation("Salary component deleted: {ComponentId} by user {UserId}", componentId, userId);
                }

                return ApiResponse<bool>.Success(result, "Salary component deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting salary component: {ComponentId}", componentId);
                return ApiResponse<bool>.Fail("An error occurred while deleting salary component");
            }
        }

        #endregion

        #region Salary Templates

        /// <summary>
        /// Get all salary templates
        /// सर्व वेतन टेम्पलेट मिळवा
        /// </summary>
        public async Task<ApiResponse<List<SalaryTemplate>>> GetAllTemplatesAsync(bool activeOnly = true)
        {
            try
            {
                var templates = await _salaryStructureRepository.GetAllTemplatesAsync(activeOnly);
                return ApiResponse<List<SalaryTemplate>>.Success(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all salary templates");
                return ApiResponse<List<SalaryTemplate>>.Fail("An error occurred while fetching salary templates");
            }
        }

        /// <summary>
        /// Get salary template by ID
        /// </summary>
        public async Task<ApiResponse<SalaryTemplate>> GetTemplateByIdAsync(int templateId)
        {
            try
            {
                var template = await _salaryStructureRepository.GetTemplateByIdAsync(templateId);

                if (template == null)
                {
                    return ApiResponse<SalaryTemplate>.Fail("Salary template not found");
                }

                return ApiResponse<SalaryTemplate>.Success(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary template: {TemplateId}", templateId);
                return ApiResponse<SalaryTemplate>.Fail("An error occurred while fetching salary template");
            }
        }

        /// <summary>
        /// Get template components
        /// </summary>
        public async Task<ApiResponse<List<SalaryTemplateComponent>>> GetTemplateComponentsAsync(int templateId)
        {
            try
            {
                var components = await _salaryStructureRepository.GetTemplateComponentsAsync(templateId);
                return ApiResponse<List<SalaryTemplateComponent>>.Success(components);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting template components: {TemplateId}", templateId);
                return ApiResponse<List<SalaryTemplateComponent>>.Fail("An error occurred while fetching template components");
            }
        }

        #endregion

        #region Employee Salary Structure

        /// <summary>
        /// Get employee's current salary structure
        /// कर्मचाऱ्याची सध्याची वेतन रचना मिळवा
        /// </summary>
        public async Task<ApiResponse<SalaryStructureResponse>> GetEmployeeCurrentSalaryAsync(int employeeId)
        {
            try
            {
                var structure = await _salaryStructureRepository.GetEmployeeCurrentSalaryAsync(employeeId);

                if (structure == null)
                {
                    return ApiResponse<SalaryStructureResponse>.Fail("No active salary structure found for this employee");
                }

                var details = await _salaryStructureRepository.GetSalaryStructureDetailsAsync(structure.Id);
                return ApiResponse<SalaryStructureResponse>.Success(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current salary for employee: {EmployeeId}", employeeId);
                return ApiResponse<SalaryStructureResponse>.Fail("An error occurred while fetching employee salary");
            }
        }

        /// <summary>
        /// Get employee salary history
        /// </summary>
        public async Task<ApiResponse<List<SalaryStructureResponse>>> GetEmployeeSalaryHistoryAsync(int employeeId)
        {
            try
            {
                var history = await _salaryStructureRepository.GetEmployeeSalaryHistoryAsync(employeeId);

                var historyDetails = new List<SalaryStructureResponse>();

                foreach (var structure in history)
                {
                    var details = await _salaryStructureRepository.GetSalaryStructureDetailsAsync(structure.Id);
                    historyDetails.Add(details);
                }

                return ApiResponse<List<SalaryStructureResponse>>.Success(historyDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary history for employee: {EmployeeId}", employeeId);
                return ApiResponse<List<SalaryStructureResponse>>.Fail("An error occurred while fetching salary history");
            }
        }

        /// <summary>
        /// Assign salary structure to employee
        /// कर्मचाऱ्याला वेतन रचना नियुक्त करा
        /// </summary>
        public async Task<ApiResponse<int>> AssignSalaryToEmployeeAsync(AssignSalaryRequest request, int userId)
        {
            try
            {
                // Validate template if provided
                if (request.TemplateId.HasValue)
                {
                    var template = await _salaryStructureRepository.GetTemplateByIdAsync(request.TemplateId.Value);
                    if (template == null)
                    {
                        return ApiResponse<int>.Fail("Salary template not found");
                    }
                }

                // Validate salary amounts
                if (request.NetSalary > request.GrossSalary)
                {
                    return ApiResponse<int>.Fail("Net salary cannot be greater than gross salary");
                }

                if (request.GrossSalary > request.CTC)
                {
                    return ApiResponse<int>.Fail("Gross salary cannot be greater than CTC");
                }

                // Assign salary
                var structureId = await _salaryStructureRepository.AssignSalaryStructureAsync(request, userId);

                if (structureId > 0)
                {
                    // Audit log
                    await _auditService.LogAsync(
                        userId,
                        "ASSIGN_SALARY",
                        "EmployeeSalaryStructure",
                        structureId,
                        null,
                        $"EmployeeId: {request.EmployeeId}, CTC: {request.CTC}, Effective: {request.EffectiveFrom:yyyy-MM-dd}"
                    );

                    _logger.LogInformation("Salary assigned: StructureId={StructureId}, EmployeeId={EmployeeId}, CTC={CTC}",
                        structureId, request.EmployeeId, request.CTC);
                }

                return ApiResponse<int>.Success(structureId, "Salary structure assigned successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning salary to employee: {EmployeeId}", request.EmployeeId);
                return ApiResponse<int>.Fail("An error occurred while assigning salary structure");
            }
        }

        /// <summary>
        /// Get salary structure details
        /// संपूर्ण वेतन रचना तपशील मिळवा
        /// </summary>
        public async Task<ApiResponse<SalaryStructureResponse>> GetSalaryStructureDetailsAsync(int structureId)
        {
            try
            {
                var details = await _salaryStructureRepository.GetSalaryStructureDetailsAsync(structureId);
                return ApiResponse<SalaryStructureResponse>.Success(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary structure details: {StructureId}", structureId);
                return ApiResponse<SalaryStructureResponse>.Fail("An error occurred while fetching salary structure details");
            }
        }

        /// <summary>
        /// Update individual salary component
        /// </summary>
        public async Task<ApiResponse<bool>> UpdateSalaryComponentAsync(int componentId, decimal amount, int userId)
        {
            try
            {
                if (amount < 0)
                {
                    return ApiResponse<bool>.Fail("Amount cannot be negative");
                }

                var result = await _salaryStructureRepository.UpdateEmployeeComponentAsync(componentId, amount, userId);

                if (result)
                {
                    // Audit log
                    await _auditService.LogAsync(
                        userId,
                        "UPDATE_COMPONENT",
                        "EmployeeSalaryComponent",
                        componentId,
                        null,
                        $"Amount updated to: {amount}"
                    );
                }

                return ApiResponse<bool>.Success(result, "Component amount updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating salary component: {ComponentId}", componentId);
                return ApiResponse<bool>.Fail("An error occurred while updating component amount");
            }
        }

        #endregion

        #region Bulk Operations

        /// <summary>
        /// Bulk assign salary to multiple employees
        /// अनेक कर्मचाऱ्यांना एकत्रित वेतन नियुक्त करा
        /// </summary>
        public async Task<ApiResponse<bool>> BulkAssignSalaryAsync(List<int> employeeIds, int templateId, DateTime effectiveFrom, int userId)
        {
            try
            {
                if (employeeIds == null || !employeeIds.Any())
                {
                    return ApiResponse<bool>.Fail("No employees selected");
                }

                // Validate template
                var template = await _salaryStructureRepository.GetTemplateByIdAsync(templateId);
                if (template == null)
                {
                    return ApiResponse<bool>.Fail("Salary template not found");
                }

                var result = await _salaryStructureRepository.BulkAssignSalaryAsync(employeeIds, templateId, effectiveFrom, userId);

                if (result)
                {
                    // Audit log
                    await _auditService.LogAsync(
                        userId,
                        "BULK_ASSIGN_SALARY",
                        "EmployeeSalaryStructure",
                        null,
                        null,
                        $"Template: {template.TemplateName}, Employees: {employeeIds.Count}, Effective: {effectiveFrom:yyyy-MM-dd}"
                    );

                    _logger.LogInformation("Bulk salary assignment completed: {Count} employees, Template: {TemplateId}",
                        employeeIds.Count, templateId);
                }

                return ApiResponse<bool>.Success(result, $"Salary assigned to {employeeIds.Count} employees successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk salary assignment");
                return ApiResponse<bool>.Fail("An error occurred during bulk salary assignment");
            }
        }

        #endregion
    }
}