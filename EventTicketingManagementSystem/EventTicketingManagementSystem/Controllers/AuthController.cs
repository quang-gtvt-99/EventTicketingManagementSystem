using Microsoft.AspNetCore.Mvc;

namespace EventTicketingManagementSystem.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
