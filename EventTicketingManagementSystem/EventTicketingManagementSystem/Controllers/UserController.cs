using EventTicketingManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingManagementSystem.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("profile")]
        public async Task<IActionResult> Profile()
        {
            var result = await _userService.GetUserProfileAsync();
            return Ok(result);
        }
    }
}
