using Microsoft.AspNetCore.Mvc;

namespace EventTicketingManagementSystem.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
