using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagement.UI.Filters
{
    public class AuthorizeSessionAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;
        //fgdg
        public AuthorizeSessionAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (_roles.Length > 0)
            {
                var userRoles = context.HttpContext.Session.GetString("Roles")?.Split(',') ?? Array.Empty<string>();

                if (!_roles.Any(r => userRoles.Contains(r)))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }

    public class AuthorizePermissionSessionAttribute : ActionFilterAttribute
    {
        private readonly string[] _permissions;

        public AuthorizePermissionSessionAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (_permissions.Length > 0)
            {
                var userPermissions = context.HttpContext.Session.GetString("Permissions")?.Split(',') ?? Array.Empty<string>();

                if (!_permissions.Any(p => userPermissions.Contains(p)))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }

}
