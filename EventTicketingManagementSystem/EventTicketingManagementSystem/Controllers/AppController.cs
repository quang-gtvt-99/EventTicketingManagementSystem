using EventTicketingManagementSystem.Data.Repository;
using EventTicketingManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {
        private readonly IUserService _userService;

        public AppController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}
