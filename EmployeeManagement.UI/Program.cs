//using EmployeeManagement.UI.Services;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.Facebook;
//using Microsoft.AspNetCore.Authentication.Google;
//using Polly;
//using Polly.Extensions.Http;
//using System.Net;

//var builder = WebApplication.CreateBuilder(args);

//// ✅ Add MVC
//builder.Services.AddControllersWithViews();


//// ✅ Session Configuration
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromSeconds(144);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//    options.Cookie.SameSite = SameSiteMode.Lax;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
//});


//// ✅ Required for TokenService
//builder.Services.AddHttpContextAccessor();


//builder.Services.AddHttpClient("ApiClient", client =>
//{
//    client.BaseAddress = new Uri("http://localhost:26024/");
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//});

//builder.Services.AddScoped<ITokenService, TokenService>();

//// ✅ Register PaymentService (ONLY ONCE — IMPORTANT)
//builder.Services.AddHttpClient<IPaymentService, PaymentService>(client =>
//{
//    client.BaseAddress = new Uri("http://localhost:26024/");
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//    client.Timeout = TimeSpan.FromSeconds(144);
//})
//.ConfigurePrimaryHttpMessageHandler(() =>
//{
//    return new HttpClientHandler
//    {
//        ServerCertificateCustomValidationCallback =
//            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//    };
//})
//.AddPolicyHandler(GetRetryPolicy());


//// ✅ Optional Named Client (if needed elsewhere)
//builder.Services.AddHttpClient("API", client =>
//{
//    client.BaseAddress = new Uri("http://localhost:26024/");
//})
//.ConfigurePrimaryHttpMessageHandler(() =>
//{
//    return new HttpClientHandler
//    {
//        ServerCertificateCustomValidationCallback =
//            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//    };
//})
//.AddPolicyHandler(GetRetryPolicy());


//// ✅ Authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//.AddCookie(options =>
//{
//    options.LoginPath = "/Account/Login";
//    options.LogoutPath = "/Account/Logout";
//    options.AccessDeniedPath = "/Account/AccessDenied";
//    options.SlidingExpiration = true;
//    options.ExpireTimeSpan = TimeSpan.FromDays(14);
//})
//.AddGoogle(options =>
//{
//    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//    options.SaveTokens = true;
//})
//.AddFacebook(options =>
//{
//    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
//    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
//    options.SaveTokens = true;
//});


//var app = builder.Build();


//// ✅ Middleware pipeline
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}
//else
//{
//    app.UseDeveloperExceptionPage();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseSession();      // MUST be before Authentication
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();


//// ✅ Polly Retry Policy
//static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
//{
//    return HttpPolicyExtensions
//        .HandleTransientHttpError()
//        .WaitAndRetryAsync(3, retry =>
//            TimeSpan.FromSeconds(Math.Pow(2, retry)));
//}
 
using EmployeeManagement.UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Polly;
using Polly.Extensions.Http;


var builder = WebApplication.CreateBuilder(args);

// =============================================
// ADD MVC SERVICES
// =============================================
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// =============================================
// SESSION CONFIGURATION
// =============================================
builder.Services.AddDistributedMemoryCache(); // Required for session

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(144);
    options.Cookie.Name = ".EmployeeManagement.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use Always in production with HTTPS
});

// =============================================
// HTTP CONTEXT ACCESSOR
// =============================================
builder.Services.AddHttpContextAccessor();

// =============================================
// GET API BASE URL FROM CONFIGURATION
// =============================================
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:26024/";

// =============================================
// HTTP CLIENT FACTORY CONFIGURATION
// =============================================

// 1️⃣ Default API Client (for general API calls)
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(144);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        builder.Environment.IsDevelopment()
            ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            : null // In production, validate certificates properly
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

// 2️⃣ Typed HttpClient for PaymentService
builder.Services.AddHttpClient<IPaymentService, PaymentService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(144);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        builder.Environment.IsDevelopment()
            ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            : null
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

// 3️⃣ Named client for API (if needed elsewhere)
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(144);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        builder.Environment.IsDevelopment()
            ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            : null
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

// =============================================
// REGISTER SERVICES
// =============================================
builder.Services.AddScoped<EmployeeManagement.UI.Services.ITokenService, EmployeeManagement.UI.Services.TokenService>();
builder.Services.AddScoped<IApiService, ApiService>();
//builder.Services.AddScoped<IAuthService, AuthService>();

//builder.Services.AddScoped<IEmployeeService, EmployeeService>();
//builder.Services.AddScoped<IDepartmentService, DepartmentService>();
// Add other services as needed

// =============================================
// AUTHENTICATION CONFIGURATION
// =============================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ReturnUrlParameter = "returnUrl";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.Cookie.Name = ".EmployeeManagement.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use Always in production

    // Handle authentication events
    options.Events = new CookieAuthenticationEvents
    {
        OnValidatePrincipal = async context =>
        {
            // Check if token is still valid from session
            var accessToken = context.HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    };
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
        ?? throw new InvalidOperationException("Google ClientId not configured");
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
        ?? throw new InvalidOperationException("Google ClientSecret not configured");
    options.SaveTokens = true;
    options.CallbackPath = "/signin-google";

    // Add required scopes
    options.Scope.Add("profile");
    options.Scope.Add("email");
})
.AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"]
        ?? throw new InvalidOperationException("Facebook AppId not configured");
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]
        ?? throw new InvalidOperationException("Facebook AppSecret not configured");
    options.SaveTokens = true;
    options.CallbackPath = "/signin-facebook";

    // Add required fields
    options.Fields.Add("name");
    options.Fields.Add("email");
    options.Fields.Add("picture");
});

// =============================================
// AUTHORIZATION
// =============================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("HROnly", policy => policy.RequireRole("Admin", "HR"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Admin", "HR", "Manager"));
});

// =============================================
// ANTI-FORGERY
// =============================================
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = ".EmployeeManagement.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// =============================================
// LOGGING
// =============================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (!builder.Environment.IsDevelopment())
{
    builder.Logging.AddEventLog();
    // Add file logging or Serilog here if needed
}

// =============================================
// BUILD APP
// =============================================
var app = builder.Build();

// =============================================
// CONFIGURE MIDDLEWARE PIPELINE
// =============================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();

    // Add strict transport security header
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        await next();
    });
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ⚠️ CORRECT ORDER: Session BEFORE Authentication
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// =============================================
// CONFIGURE ROUTES
// =============================================
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// =============================================
// RUN APPLICATION
// =============================================
app.Run();

// =============================================
// POLLY POLICIES
// =============================================

/// <summary>
/// Retry policy for transient HTTP errors (408, 5xx, network failures)
/// Retries up to 3 times with exponential backoff
/// </summary>
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                // Log retry attempts
                Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s delay due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            });
}

/// <summary>
/// Circuit breaker policy to prevent cascading failures
/// Opens circuit after 5 consecutive failures
/// Stays open for 144 seconds before attempting recovery
/// </summary>
static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(144),
            onBreak: (outcome, duration) =>
            {
                Console.WriteLine($"Circuit breaker opened for {duration.TotalSeconds}s due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            },
            onReset: () =>
            {
                Console.WriteLine("Circuit breaker reset");
            },
            onHalfOpen: () =>
            {
                Console.WriteLine("Circuit breaker half-open, testing...");
            });
}