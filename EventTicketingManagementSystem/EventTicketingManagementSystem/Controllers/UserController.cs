﻿using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.Services.Services.Implements;
using EventTicketingManagementSystem.Response;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Hubs;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Org.BouncyCastle.Asn1.Ocsp;

namespace EventTicketingManagementSystem.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles = $"{RoleConsts.User}, {RoleConsts.Admin}")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IVNPayService _vnPayService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public UserController(IUserService userService, IVNPayService vnPayService, IHubContext<NotificationHub> hubContext)
        {
            _userService = userService;
            _vnPayService = vnPayService;
            _hubContext = hubContext;
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
                    await _userService.SendEmailToId(result,loggedInUserId);
                }
            }
            return Ok(result);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfoDto>> GetUserById(int id)
        {
            var userItem = await _userService.GetUserByIdAsync(id);
            if (userItem == null) return NotFound();
            return Ok(userItem);
        }

        // GET: api/users/filter?search=music&category=Concert&status=Active&pageNumber=1&pageSize=10
        [HttpGet("filter")]
        public async Task<IActionResult> GetUsersByFilter([FromQuery] string? search)
        {
            var users = await _userService.GetFilteredPagedUsersAsync(search);

            return Ok(users);
        }


        //// POST: api/users
        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Admin}")]
        public async Task<ActionResult<int>> CreateUser([FromBody] AddUpdateUserRequest userItem)
        {
            var newUserID = await _userService.CreateUser(userItem);
            return Ok(newUserID);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = $"{RoleConsts.Admin}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] AddUpdateUserRequest userItem)
        {
            if (id != userItem.ID) return BadRequest();

            var updated = await _userService.UpdateUser(userItem);
            if (!updated) return NotFound();


            if (userItem.IsActive == false)
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null || user?.Email == null) return NotFound();
                var userEmail = user.Email;
                await _hubContext.Clients.Group(userEmail).SendAsync("ReceiveMessage", "Tài khoản của bạn đã bị khóa, Vui lòng liên hệ với admin để được hỗ trợ.", userItem.Email);
            }

            return Ok(true);
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{RoleConsts.Admin}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUser(id);

            if (!deleted) return NotFound();

            return Ok(true);
        }
        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentResponse request)
        {
            try
            {
                if (request.ResponseCode != "00" || request.TransactionStatus != "00")
                {
                    await _userService.ProcessFailBookingAndSeatsAsync(request.BookingId);
                    return Ok(new { message = "Payment fail. Delete booking and updated seats processed successfully" });
                }
                else
                {
                    await _userService.ProcessSuccessfulTicketAndPaymentAsync(request.BookingId, request);
                    return Ok(new { Message = "Payment Successfully. Tickets created and payment processed successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
