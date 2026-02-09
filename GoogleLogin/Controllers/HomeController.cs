using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoogleLogin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        { 
            ViewData["IsAuthenticated"] = User.Identity?.IsAuthenticated ?? false;
            ViewData["Id"] = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewData["UserName"] = User.Identity?.Name;
            ViewData["Email"] = User.FindFirst(ClaimTypes.Email)?.Value;
            ViewData["Role"] = User.FindFirst(ClaimTypes.Role)?.Value;
             
            return View("Home");
        }
    }
}