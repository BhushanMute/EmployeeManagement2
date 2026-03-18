using EmployeeManagement.API;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.Services;
using EmployeeManagement.API.services;

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
// EMPLOYEE MANAGEMENT SERVICES
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


builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);
builder.Services.AddMemoryCache();

// =============================================
// STUDENT MANAGEMENT
// =============================================
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IStudentIdGenerator, StudentIdGenerator>();
builder.Services.AddSingleton<ICacheService, CacheService>();
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
// AUTHORIZATION POLICIES
// =============================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("HROnly", policy => policy.RequireRole("Admin", "HR"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Admin", "HR", "Manager"));
    options.AddPolicy("AllRoles", policy => policy.RequireRole("Admin", "HR", "Manager", "Employee"));

    options.AddPolicy("CanViewEmployees", policy => policy.RequireClaim("Permission", "Employee.View"));
    options.AddPolicy("Employee.Create", policy => policy.RequireClaim("Permission", "Employee.Create"));
    options.AddPolicy("CanUpdateEmployee", policy => policy.RequireClaim("Permission", "Employee.Update"));
    options.AddPolicy("CanDeleteEmployee", policy => policy.RequireClaim("Permission", "Employee.Delete"));

    options.AddPolicy("CanViewStudents", policy => policy.RequireClaim("Permission", "Student.View"));
    options.AddPolicy("CanCreateStudent", policy => policy.RequireClaim("Permission", "Student.Create"));
    options.AddPolicy("CanUpdateStudent", policy => policy.RequireClaim("Permission", "Student.Update"));
    options.AddPolicy("CanDeleteStudent", policy => policy.RequireClaim("Permission", "Student.Delete"));

    options.AddPolicy("CanApplyLeave", policy => policy.RequireClaim("Permission", "Leave.Apply"));
    options.AddPolicy("CanApproveLeave", policy => policy.RequireClaim("Permission", "Leave.Approve"));
    options.AddPolicy("CanViewLeave", policy => policy.RequireClaim("Permission", "Leave.View"));
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
        Title = "Employee & Student Management API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
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
builder.Services.AddOutputCache(options =>
{
    // Default: cache for 30 seconds
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);

    // Custom policies
    options.AddPolicy("LeaveTypes", policy => policy.Expire(TimeSpan.FromMinutes(30)));
    options.AddPolicy("Holidays", policy => policy.Expire(TimeSpan.FromHours(1)));
    options.AddPolicy("ShortCache", policy => policy.Expire(TimeSpan.FromSeconds(10)));
});
builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API v1");
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

// =============================================
// MAP CONTROLLERS
// =============================================
app.MapControllers();

Console.WriteLine("🚀 Application started successfully!");
Console.WriteLine($"📁 Web Root Path: {webRootPath}");

app.Run();