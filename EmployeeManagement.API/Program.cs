using EmployeeManagement.API;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
 
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
 
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

// Null check for jwtSettings
if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("JWT Settings not configured properly in appsettings.json");
}

// =============================================
// REGISTER CORE SERVICES
// =============================================
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<DbHelper>();

// =============================================
// REGISTER EXISTING EMPLOYEE MANAGEMENT SERVICES
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

// =============================================
// ✅ REGISTER STUDENT ATTENDANCE SERVICES (NEW)
// =============================================

// Student Management
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// Attendance Management
//builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();

// Excel & File Services
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// ID Generator Service
builder.Services.AddScoped<IStudentIdGenerator, StudentIdGenerator>();

// Face Recognition Service (Optional - Uncomment when ready)
/*
var faceRecognitionProvider = builder.Configuration["FaceRecognition:Provider"];
if (faceRecognitionProvider == "Azure")
{
    builder.Services.AddScoped<IFaceRecognitionService, AzureFaceRecognitionService>();
}
else if (faceRecognitionProvider == "Python")
{
    builder.Services.AddScoped<IFaceRecognitionService, PythonFaceRecognitionService>();
}
else
{
    builder.Services.AddScoped<IFaceRecognitionService, OpenCVFaceRecognitionService>();
}
*/

// =============================================
// HTTP CLIENT
// =============================================
builder.Services.AddHttpClient<IPaymentGatewayService, PaymentGatewayService>();
builder.Services.AddHttpClient(); // For general HTTP client usage

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
    options.RequireHttpsMetadata = false; // Set to true in production
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
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Unauthorized access attempt to: {Path}", context.Request.Path);
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var userId = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            logger.LogWarning("Forbidden access by User: {UserId} to: {Path}", userId, context.Request.Path);
            return Task.CompletedTask;
        }
    };
});

// =============================================
// AUTHORIZATION POLICIES
// =============================================
builder.Services.AddAuthorization(options =>
{
    // ========== ROLE-BASED POLICIES ==========
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("HROnly", policy => policy.RequireRole("Admin", "HR"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Admin", "HR", "Manager"));
    options.AddPolicy("AllRoles", policy => policy.RequireRole("Admin", "HR", "Manager", "Employee"));

    // ========== EMPLOYEE PERMISSION POLICIES ==========
    options.AddPolicy("CanViewEmployees", policy => policy.RequireClaim("Permission", "Employee.View"));
    options.AddPolicy("CanViewEmployeeDetails", policy => policy.RequireClaim("Permission", "Employee.ViewDetails"));
    options.AddPolicy("Employee.Create", policy => policy.RequireClaim("Permission", "Employee.Create"));
    options.AddPolicy("Employee.Register", policy => policy.RequireClaim("Permission", "Employee.Register"));
    options.AddPolicy("CanUpdateEmployee", policy => policy.RequireClaim("Permission", "Employee.Update"));
    options.AddPolicy("CanDeleteEmployee", policy => policy.RequireClaim("Permission", "Employee.Delete"));
    options.AddPolicy("CanExportEmployee", policy => policy.RequireClaim("Permission", "Employee.Export"));
    options.AddPolicy("CanImportEmployee", policy => policy.RequireClaim("Permission", "Employee.Import"));

    // ========== USER MANAGEMENT POLICIES ==========
    options.AddPolicy("CanViewUsers", policy => policy.RequireClaim("Permission", "User.View"));
    options.AddPolicy("CanCreateUser", policy => policy.RequireClaim("Permission", "User.Create"));
    options.AddPolicy("CanUpdateUser", policy => policy.RequireClaim("Permission", "User.Update"));
    options.AddPolicy("CanDeleteUser", policy => policy.RequireClaim("Permission", "User.Delete"));
    options.AddPolicy("CanManageUserRoles", policy => policy.RequireClaim("Permission", "User.ManageRoles"));

    // ========== ROLE MANAGEMENT POLICIES ==========
    options.AddPolicy("CanViewRoles", policy => policy.RequireClaim("Permission", "Role.View"));
    options.AddPolicy("CanManageRoles", policy => policy.RequireClaim("Permission", "Role.ManagePermissions"));

    // ========== PAYMENT POLICIES ==========
    options.AddPolicy("CanViewPayments", policy => policy.RequireClaim("Permission", "Payment.View"));
    options.AddPolicy("CanProcessPayments", policy => policy.RequireClaim("Permission", "Payment.Process"));

    // ========== REPORT POLICIES ==========
    options.AddPolicy("CanViewReports", policy => policy.RequireClaim("Permission", "Report.View"));
    options.AddPolicy("CanExportReports", policy => policy.RequireClaim("Permission", "Report.Export"));

    // ========== DEPARTMENT POLICIES ==========
    options.AddPolicy("CanViewDepartments", policy => policy.RequireClaim("Permission", "Department.View"));
    options.AddPolicy("CanManageDepartments", policy => policy.RequireClaim("Permission", "Department.Manage"));

    // ========== ✅ STUDENT MANAGEMENT POLICIES (NEW) ==========
    options.AddPolicy("CanViewStudents", policy => policy.RequireClaim("Permission", "Student.View"));
    options.AddPolicy("CanCreateStudent", policy => policy.RequireClaim("Permission", "Student.Create"));
    options.AddPolicy("CanUpdateStudent", policy => policy.RequireClaim("Permission", "Student.Update"));
    options.AddPolicy("CanDeleteStudent", policy => policy.RequireClaim("Permission", "Student.Delete"));
    options.AddPolicy("CanImportStudents", policy => policy.RequireClaim("Permission", "Student.Import"));
    options.AddPolicy("CanExportStudents", policy => policy.RequireClaim("Permission", "Student.Export"));

    // ========== ✅ ATTENDANCE POLICIES (NEW) ==========
    options.AddPolicy("CanViewAttendance", policy => policy.RequireClaim("Permission", "Attendance.View"));
    options.AddPolicy("CanMarkAttendance", policy => policy.RequireClaim("Permission", "Attendance.Mark"));
    options.AddPolicy("CanExportAttendance", policy => policy.RequireClaim("Permission", "Attendance.Export"));
});

// =============================================
// CORS CONFIGURATION
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
// SWAGGER CONFIGURATION
// =============================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Employee & Student Management API",
        Version = "v1",
        Description = "API with Role-Based Authentication & Student Attendance System",
        Contact = new OpenApiContact
        {
            Name = "Admin",
            Email = "admin@company.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
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

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// =============================================
// HTTP CONTEXT ACCESSOR & LOGGING
// =============================================
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

// =============================================
// BUILD APP
// =============================================
var app = builder.Build();

// =============================================
// CONFIGURE MIDDLEWARE PIPELINE
// =============================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API v1");
        c.RoutePrefix = "swagger";
    });
}

// Global Exception Handling
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// ✅ Static Files (for uploaded photos)
app.UseStaticFiles();

// CORS - Must be before Authentication
app.UseCors("AllowMVC");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ Create upload directories if they don't exist
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
    Path.Combine(webRootPath, "uploads", "temp")
};

foreach (var path in uploadPaths)
{
    if (!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
        Console.WriteLine($"Created directory: {path}");
    }
}

// Map Controllers
app.MapControllers();

Console.WriteLine("🚀 Application started successfully!");
Console.WriteLine($"📁 Web Root Path: {webRootPath}");
Console.WriteLine($"🔗 Swagger UI: {(app.Environment.IsDevelopment() ? "https://localhost:7192/swagger" : "Disabled")}");

app.Run();