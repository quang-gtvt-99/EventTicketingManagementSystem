using EventTicketingManagementSystem.Request;
using EventTicketingManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingManagementSystem.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJwtAuth _jwtAuth;

        public AuthController(IUserService userService, IJwtAuth jwtAuth)
        {
            _userService = userService;
            _jwtAuth = jwtAuth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _userService.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            var authResult = _jwtAuth.Authentication(model.Email, model.Password);

            if (authResult == null)
            {
                return Unauthorized(); 
            }

            return Ok(authResult); 
        }
    }
}
