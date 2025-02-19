using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventTicketingManagementSystem.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles = $"{RoleConsts.User}, {RoleConsts.Admin}")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IVNPayService _vnPayService;
        public UserController(IUserService userService, IVNPayService vnPayService)
        {
            _userService = userService;
            _vnPayService = vnPayService;
        }

        [HttpGet]
        [Route("profile")]
        public async Task<IActionResult> Profile()
        {
            var result = await _userService.GetUserProfileAsync();
            return Ok(result);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest request)
        {
            var response = await _userService.UpdateUserProfileAsync(request);
            return Ok(response);
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
                var bookingResult = await _userService.CreateBookingAsync(bookingRequestDto, loggedInUserId);

                return Ok(new
                {
                    success = true,
                    message = "Booking successful.",
                    bookingId = bookingResult.Id,
                    totalAmount = bookingResult.TotalAmount
                });
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
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            var booking = new Booking
            {
                Id = request.BookingId,
                TotalAmount = request.Amount,
                Status = CommConstants.CST_PAY_STATUS_PENDING,
                BookingDate = DateTime.Now
            };

            var paymentUrl = await _vnPayService.CreatePaymentUrl(booking, request.Locale, request.BankCode, request.OrderType);
            return Ok(new { PaymentUrl = paymentUrl });
        }
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var query = HttpContext.Request.Query;
            var result = await _vnPayService.ProcessVnPayReturn(query);
            if (result.ResponseCode == "00" && result.TransactionStatus == "00")
            {
                if (userIdClaim != null)
                {
                    int loggedInUserId = int.Parse(userIdClaim.Value);
                    _userService.SendEmailToId(result,loggedInUserId);
                }
            }
            return Ok(result);
        }
    }
}
