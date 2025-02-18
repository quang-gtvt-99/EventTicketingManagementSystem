using EventTicketingManagementSystem.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingManagementSystem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISendMailService _sendMailService;

        public AppController(IUserService userService, ISendMailService sendMailService)
        {
            _userService = userService;
            _sendMailService = sendMailService;
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


    }
}
