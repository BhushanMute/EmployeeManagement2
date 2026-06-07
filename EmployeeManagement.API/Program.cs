using EmployeeManagement.API;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.Salary;
using EmployeeManagement.API.Repositories.Ticket;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using EmployeeManagement.API.Services.Implementation;
using EmployeeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// ADD CONTROLLERS
// =============================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// =============================================
// GET JWT SETTINGS
// =============================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("JWT Settings not configured properly in appsettings.json");
}

// =============================================
// CORE SERVICES
// =============================================
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<DbHelper>();

// =============================================
// EMPLOYEE MANAGEMENT SERVICES (EXISTING)
// =============================================
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITokenGenerationService, TokenGenerationService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
builder.Services.AddScoped<IDummyUpiPaymentService, DummyUpiPaymentService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

// =============================================
// PAYROLL SYSTEM - REPOSITORIES (NEW)
// =============================================
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped<ISalaryStructureRepository, SalaryStructureRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
// Add more as needed:
// builder.Services.AddScoped<IAdvanceRepository, AdvanceRepository>();
// builder.Services.AddScoped<IReimbursementRepository, ReimbursementRepository>();
// builder.Services.AddScoped<ITaxDeclarationRepository, TaxDeclarationRepository>();

// =============================================
// PAYROLL SYSTEM - SERVICES (NEW)
// =============================================
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<ISalaryStructureService, SalaryStructureService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ISalarySlipService, SalarySlipService>();
// Add more as needed:
// builder.Services.AddScoped<IAdvanceService, AdvanceService>();
// builder.Services.AddScoped<IReimbursementService, ReimbursementService>();
// builder.Services.AddScoped<ITaxDeclarationService, TaxDeclarationService>();
// builder.Services.AddScoped<ISalarySlipService, SalarySlipService>();
// builder.Services.AddScoped<ISSRSReportService, SSRSReportService>();

// =============================================
// EMAIL SETTINGS
// =============================================
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);

// =============================================
// CACHING
// =============================================
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, CacheService>();

// =============================================
// STUDENT MANAGEMENT (EXISTING)
// =============================================
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IStudentIdGenerator, StudentIdGenerator>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped<ISalaryStructureRepository, SalaryStructureRepository>();
builder.Services.AddScoped<ISsrsReportService, SsrsReportService>();
builder.Services.AddHostedService<PayrollEmailBackgroundService>();
builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();



// =============================================
// AUDIT LOGGING (EXISTING)
// =============================================
builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// =============================================
// HTTP CLIENT
// =============================================
builder.Services.AddHttpClient<IPaymentGatewayService, PaymentGatewayService>();

// =============================================
// JWT AUTHENTICATION
// =============================================
var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = "role"
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

// =============================================
// AUTHORIZATION POLICIES (UPDATED WITH PAYROLL)
// =============================================
builder.Services.AddAuthorization(options =>
{
    // Existing policies
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("HROnly", policy => policy.RequireRole("Admin", "HR"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Admin", "HR", "Manager"));
    options.AddPolicy("AllRoles", policy => policy.RequireRole("Admin", "HR", "Manager", "Employee"));

    // Employee permissions
    options.AddPolicy("CanViewEmployees", policy => policy.RequireClaim("Permission", "Employee.View"));
    options.AddPolicy("Employee.Create", policy => policy.RequireClaim("Permission", "Employee.Create"));
    options.AddPolicy("CanUpdateEmployee", policy => policy.RequireClaim("Permission", "Employee.Update"));
    options.AddPolicy("CanDeleteEmployee", policy => policy.RequireClaim("Permission", "Employee.Delete"));

    // Student permissions
    options.AddPolicy("CanViewStudents", policy => policy.RequireClaim("Permission", "Student.View"));
    options.AddPolicy("CanCreateStudent", policy => policy.RequireClaim("Permission", "Student.Create"));
    options.AddPolicy("CanUpdateStudent", policy => policy.RequireClaim("Permission", "Student.Update"));
    options.AddPolicy("CanDeleteStudent", policy => policy.RequireClaim("Permission", "Student.Delete"));

    // Leave permissions
    options.AddPolicy("CanApplyLeave", policy => policy.RequireClaim("Permission", "Leave.Apply"));
    options.AddPolicy("CanApproveLeave", policy => policy.RequireClaim("Permission", "Leave.Approve"));
    options.AddPolicy("CanViewLeave", policy => policy.RequireClaim("Permission", "Leave.View"));

    // ===== PAYROLL PERMISSIONS (NEW) =====
    // Payroll Cycle
    options.AddPolicy("Payroll.ViewCycle", policy => policy.RequireClaim("Permission", "Payroll.ViewCycle"));
    options.AddPolicy("Payroll.CreateCycle", policy => policy.RequireClaim("Permission", "Payroll.CreateCycle"));
    options.AddPolicy("Payroll.ProcessPayroll", policy => policy.RequireClaim("Permission", "Payroll.ProcessPayroll"));
    options.AddPolicy("Payroll.ApprovePayroll", policy => policy.RequireClaim("Permission", "Payroll.ApprovePayroll"));
    options.AddPolicy("Payroll.LockPayroll", policy => policy.RequireClaim("Permission", "Payroll.LockPayroll"));

    // Salary Structure
    options.AddPolicy("Salary.ViewStructure", policy => policy.RequireClaim("Permission", "Salary.ViewStructure"));
    options.AddPolicy("Salary.AssignStructure", policy => policy.RequireClaim("Permission", "Salary.AssignStructure"));
    options.AddPolicy("Salary.UpdateStructure", policy => policy.RequireClaim("Permission", "Salary.UpdateStructure"));
    options.AddPolicy("Salary.ManageComponents", policy => policy.RequireClaim("Permission", "Salary.ManageComponents"));

    // Loan Management
    options.AddPolicy("Loan.Apply", policy => policy.RequireClaim("Permission", "Loan.Apply"));
    options.AddPolicy("Loan.View", policy => policy.RequireClaim("Permission", "Loan.View"));
    options.AddPolicy("Loan.Approve", policy => policy.RequireClaim("Permission", "Loan.Approve"));
    options.AddPolicy("Loan.Disburse", policy => policy.RequireClaim("Permission", "Loan.Disburse"));

    // Salary Slip
    options.AddPolicy("SalarySlip.View", policy => policy.RequireClaim("Permission", "SalarySlip.View"));
    options.AddPolicy("SalarySlip.Generate", policy => policy.RequireClaim("Permission", "SalarySlip.Generate"));
    options.AddPolicy("SalarySlip.Download", policy => policy.RequireClaim("Permission", "SalarySlip.Download"));

    // Reports
    options.AddPolicy("Payroll.ViewReports", policy => policy.RequireClaim("Permission", "Payroll.ViewReports"));
});

// =============================================
// CORS
// =============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.WithOrigins("https://localhost:44354")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// =============================================
// SWAGGER
// =============================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Employee Management & Payroll API",
        Version = "v1",
        Description = "Complete Employee Management System with Payroll, Loans, and SSRS Integration"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// =============================================
// LOGGING
// =============================================
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

// =============================================
// RESPONSE COMPRESSION
// =============================================
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.MimeTypes = new[]
    {
        "application/json",
        "text/html",
        "text/css",
        "application/javascript",
        "text/plain"
    };
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

// =============================================
// OUTPUT CACHE
// =============================================
builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);
    options.AddPolicy("LeaveTypes", policy => policy.Expire(TimeSpan.FromMinutes(30)));
    options.AddPolicy("Holidays", policy => policy.Expire(TimeSpan.FromHours(1)));
    options.AddPolicy("ShortCache", policy => policy.Expire(TimeSpan.FromSeconds(10)));

    // Payroll caching policies
    options.AddPolicy("SalaryComponents", policy => policy.Expire(TimeSpan.FromHours(2)));
    options.AddPolicy("LoanTypes", policy => policy.Expire(TimeSpan.FromHours(1)));
});

// =============================================
// BUILD APP
// =============================================
var app = builder.Build();

app.UseResponseCompression();

// =============================================
// DEVELOPMENT CONFIG
// =============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management & Payroll API v1");
        c.RoutePrefix = "swagger";
    });
}

// =============================================
// MIDDLEWARE PIPELINE
// =============================================
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseOutputCache();
app.UseCors("AllowMVC");

app.UseAuthentication();
app.UseAuthorization();

// =============================================
// CREATE UPLOAD FOLDERS
// =============================================
var webRootPath = app.Environment.WebRootPath;

if (string.IsNullOrEmpty(webRootPath))
{
    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
}

var uploadPaths = new[]
{
    Path.Combine(webRootPath, "uploads", "profiles"),
    Path.Combine(webRootPath, "uploads", "students"),
    Path.Combine(webRootPath, "uploads", "attendance"),
    Path.Combine(webRootPath, "uploads", "leave-attachments"),
    Path.Combine(webRootPath, "uploads", "temp"),
    // Payroll related folders
    Path.Combine(webRootPath, "uploads", "payslips"),
    Path.Combine(webRootPath, "uploads", "loan-documents"),
    Path.Combine(webRootPath, "uploads", "reimbursement-bills"),
    Path.Combine(webRootPath, "uploads", "tax-proofs")
};

foreach (var path in uploadPaths)
{
    if (!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
        Console.WriteLine($"Created directory: {path}");
    }
}

// =============================================
// MAP CONTROLLERS
// =============================================
app.MapControllers();

Console.WriteLine("🚀 Application started successfully!");
Console.WriteLine($"📁 Web Root Path: {webRootPath}");
Console.WriteLine("💰 Payroll System Initialized!");

app.Run();
