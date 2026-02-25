using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace EmployeeManagement.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permission;

        public AuthorizePermissionAttribute(string permission)
        {
            _permission = permission;
        }

        //public void OnAuthorization(AuthorizationFilterContext context)
        //{
        //    var user = context.HttpContext.User;

        //    // Check if user is authenticated
        //    if (!user.Identity?.IsAuthenticated ?? true)
        //    {
        //        context.Result = new UnauthorizedResult();
        //        return;
        //    }

        //    // ✅ Check for permission claim
        //    var hasPermission = user.Claims
        //        .Where(c => c.Type == "Permission" || c.Type == "permission")
        //        .Any(c => c.Value.Equals(_permission, StringComparison.OrdinalIgnoreCase));

        //    if (hasPermission)
        //    {
        //        return; // Permission found, allow access
        //    }

        //    // ✅ Also check if user is Admin (Admin has all permissions)
        //    var isAdmin = user.IsInRole("Admin");
        //    if (isAdmin)
        //    {
        //        return; // Admin has all permissions
        //    }

        //    // Log forbidden access
        //    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var logger = context.HttpContext.RequestServices.GetService<ILogger<AuthorizePermissionAttribute>>();
        //    logger?.LogWarning("Forbidden access by User: {UserId} to: {Path}. Required permission: {Permission}",
        //        userId, context.HttpContext.Request.Path, _permission);

        //    context.Result = new ForbidResult();
        //}
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity!.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get roles
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role)
                                   .Select(c => c.Value)
                                   .ToList();

            // If role is Employee, grant all EmployeeController permissions
            if (roles.Contains("Employee"))
                return; // allow access

            // Otherwise, check if user has required permissions
            var userPermissions = user.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            if (!_permission.Any(p => userPermissions.Contains(p.ToString())))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}