//using EmployeeManagement.API.Common;
//using EmployeeManagement.API.Models;
//using EmployeeManagement.API.Repositories;
//using EmployeeManagement.API.services;
//using EmployeeManagement.API.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();

//// Get JWT Settings
//var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

//// Register Services
//builder.Services.AddScoped<DbHelper>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
//builder.Services.AddScoped<ITokenGenerationService, TokenGenerationService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
//builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
//builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
//builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
//builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
//builder.Services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
//builder.Services.AddScoped<IDummyUpiPaymentService, DummyUpiPaymentService>();

//// HttpClient
//builder.Services.AddHttpClient<IPaymentGatewayService, PaymentGatewayService>();

//// JWT Authentication
//var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings.Issuer,
//        ValidAudience = jwtSettings.Audience,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ClockSkew = TimeSpan.Zero
//    };
//});

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new() { Title = "Employee API", Version = "v1" });

//    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Description = "Enter: Bearer {your JWT token}"
//    });

//    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//    {
//        {
//            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//            {
//                Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                {
//                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });
//});

//builder.Services.AddHttpContextAccessor();
//builder.Services.AddLogging();

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using EmployeeManagement.API;
using EmployeeManagement.API.Common;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.Repositories;
using EmployeeManagement.API.services;
using EmployeeManagement.API.Services;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
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
// REGISTER SERVICES - Your Existing Services
// =============================================
//builder.Services.AddScoped<IDbConnectionFactory>(sp =>
//{
//    var configuration = sp.GetRequiredService<IConfiguration>();
//    var connStr = configuration.GetConnectionString("Default");
//    return new DbConnectionFactory(connStr);
//});
//builder.Services.AddScoped<IDbConnectionFactory>(serviceProvider =>
//{
//    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

//    var connectionString = configuration.GetConnectionString("DefaultConnection");

//    if (string.IsNullOrEmpty(connectionString))
//    {
//        throw new Exception("Connection string 'Default' not found.");
//    }

//    return new DbConnectionFactory(connectionString);
//});
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<DbHelper>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>(); // ADD THIS
builder.Services.AddScoped<ITokenService,  TokenService>();

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITokenGenerationService, TokenGenerationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
builder.Services.AddScoped<IDummyUpiPaymentService, DummyUpiPaymentService>();
//builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
 // =============================================
// REGISTER NEW SERVICES - Role-Based Auth
// =============================================
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuditService, AuditService>();
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
        ClockSkew = TimeSpan.Zero, // No tolerance for token expiration,
        RoleClaimType = "role"
    };

    // ✅ ADD JWT EVENTS
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // Add header if token is expired
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            // Log unauthorized access attempts
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Unauthorized access attempt to: {Path}", context.Request.Path);
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            // Log forbidden access attempts
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
    options.AddPolicy("Employee.Register",
       policy => policy.RequireClaim("Permission", "Employee.Register"));
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("HROnly", policy =>
        policy.RequireRole("Admin", "HR"));

    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireRole("Admin", "HR", "Manager"));

    options.AddPolicy("AllRoles", policy =>
        policy.RequireRole("Admin", "HR", "Manager", "Employee"));

    // ========== PERMISSION-BASED POLICIES ==========
    // Employee Permissions
    options.AddPolicy("CanViewEmployees", policy =>
        policy.RequireClaim("Permission", "Employee.View"));

    options.AddPolicy("CanViewEmployeeDetails", policy =>
        policy.RequireClaim("Permission", "Employee.ViewDetails"));

    options.AddPolicy("Employee.Create", policy =>
        policy.RequireClaim("Permission", "Employee.Create"));

    options.AddPolicy("CanUpdateEmployee", policy =>
        policy.RequireClaim("Permission", "Employee.Update"));

    options.AddPolicy("CanDeleteEmployee", policy =>
        policy.RequireClaim("Permission", "Employee.Delete"));

    options.AddPolicy("CanExportEmployee", policy =>
        policy.RequireClaim("Permission", "Employee.Export"));

    options.AddPolicy("CanImportEmployee", policy =>
        policy.RequireClaim("Permission", "Employee.Import"));

    // User Management Permissions
    options.AddPolicy("CanViewUsers", policy =>
        policy.RequireClaim("Permission", "User.View"));

    options.AddPolicy("CanCreateUser", policy =>
        policy.RequireClaim("Permission", "User.Create"));

    options.AddPolicy("CanUpdateUser", policy =>
        policy.RequireClaim("Permission", "User.Update"));

    options.AddPolicy("CanDeleteUser", policy =>
        policy.RequireClaim("Permission", "User.Delete"));

    options.AddPolicy("CanManageUserRoles", policy =>
        policy.RequireClaim("Permission", "User.ManageRoles"));

    // Role Management Permissions
    options.AddPolicy("CanViewRoles", policy =>
        policy.RequireClaim("Permission", "Role.View"));

    options.AddPolicy("CanManageRoles", policy =>
        policy.RequireClaim("Permission", "Role.ManagePermissions"));

    // Payment Permissions
    options.AddPolicy("CanViewPayments", policy =>
        policy.RequireClaim("Permission", "Payment.View"));

    options.AddPolicy("CanProcessPayments", policy =>
        policy.RequireClaim("Permission", "Payment.Process"));

    // Report Permissions
    options.AddPolicy("CanViewReports", policy =>
        policy.RequireClaim("Permission", "Report.View"));

    options.AddPolicy("CanExportReports", policy =>
        policy.RequireClaim("Permission", "Report.Export"));

    // Department Permissions
    options.AddPolicy("CanViewDepartments", policy =>
        policy.RequireClaim("Permission", "Department.View"));

    options.AddPolicy("CanManageDepartments", policy =>
        policy.RequireClaim("Permission", "Department.Manage"));
});

// =============================================
// CORS CONFIGURATION
// =============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.WithOrigins(
            "https://localhost:5001",  // MVC HTTPS
            "http://localhost:5000",   // MVC HTTP
            "https://localhost:7001",  // Additional ports if needed
            "http://localhost:7000"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
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
        Title = "Employee Management API",
        Version = "v1",
        Description = "API with Role-Based Authentication",
        Contact = new OpenApiContact
        {
            Name = "Admin",
            Email = "admin@company.com"
        }
    });

    // JWT Security Definition
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

    // Include XML comments if available
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
    // Add file logging if using Serilog
});

// =============================================
// BUILD APP
// =============================================
var app = builder.Build();

// =============================================
// CONFIGURE MIDDLEWARE PIPELINE
// =============================================

// Development only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API v1");
        c.RoutePrefix = "swagger";
    });
}

// Global Exception Handling Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// CORS - Must be before Authentication
app.UseCors("AllowMVC"); // Use "AllowAll" for development

// Authentication & Authorization - Order matters!
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();