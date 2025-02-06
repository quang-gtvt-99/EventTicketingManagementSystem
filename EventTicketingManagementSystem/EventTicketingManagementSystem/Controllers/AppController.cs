using EventTicketingManagementSystem.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {
        private readonly IAppDbService _appDataService;

        public AppController(IAppDbService appDataService)
        {
            _appDataService = appDataService;
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _appDataService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _appDataService.GetAllRolesAsync();
            return Ok(roles);
        }
    }
}
