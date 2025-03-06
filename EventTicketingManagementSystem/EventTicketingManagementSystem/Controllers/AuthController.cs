using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EventTicketingManagementSystem.API.Controllers
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
        // [EnableRateLimiting(RateLimitConst.FixedRateLimit)]
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
        // [EnableRateLimiting(RateLimitConst.FixedRateLimit)]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var authResult = await _jwtAuth.Authentication(model.Email, model.Password);

            if (authResult == null)
            {
                return Unauthorized();
            }

            return Ok(authResult);
        }

        [HttpPost("forgot-password")]
        // [EnableRateLimiting(RateLimitConst.FixedRateLimit)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var response = await _userService.ResetPasswordAsync(request);

            return Ok(response);
        }
    }
}
