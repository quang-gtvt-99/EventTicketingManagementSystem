using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventTicketingManagementSystem.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles = "User")]
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
        [HttpPost]
        [Route("booking")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto bookingRequestDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User not authenticated.");
                }

                int loggedInUserId = int.Parse(userIdClaim.Value);
                await _userService.CreateBookingAsync(bookingRequestDto, loggedInUserId);

                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("payment/{paymentId}")]
        public async Task<IActionResult> UpdatePaymentStatus(int paymentId, [FromBody] UpdatePaymentDto requestDto)
        {
            try
            {
                var updatedPayment = await _userService.UpdatePaymentStatusAsync(paymentId, requestDto);
                return Ok(updatedPayment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("delete-expired/{paymentId}")]
        public async Task<IActionResult> DeleteExpiredBooking(int paymentId)
        {
            try
            {
                bool deleted = await _userService.DeleteExpiredBookingAsync(paymentId);
                if (deleted)
                {
                    return Ok(new { message = "Deleted successfully." });
                }
                return BadRequest(new { message = "Delete failure." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("create-ticket/{bookingId}")]
        public async Task<IActionResult> CreateTickets(int bookingId)
        {
            try
            {
                var tickets = await _userService.CreateTicketsAsync(bookingId);
                return Ok(tickets);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
