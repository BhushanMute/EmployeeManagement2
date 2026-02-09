using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.UI.Controllers
{
    public class BaseController : Controller
    {
        protected bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("token") != null;
        }

        protected IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Account");
        }
    }
}
