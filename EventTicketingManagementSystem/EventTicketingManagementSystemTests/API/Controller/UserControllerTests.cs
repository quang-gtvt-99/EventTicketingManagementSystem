using EventTicketingManagementSystem.API.Controllers;
using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.API.Response;
using EventTicketingManagementSystem.Response;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Hubs;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EventTicketingManagementSystemTests.API.Controller
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IVNPayService> _mockVnPayService;
        private readonly Mock<IHubContext<NotificationHub>> _mockHubContext;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockVnPayService = new Mock<IVNPayService>();
            _mockHubContext = new Mock<IHubContext<NotificationHub>>();
            _userController = new UserController(_mockUserService.Object, _mockVnPayService.Object, _mockHubContext.Object);

            // Setup default user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, RoleConsts.User)
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task Profile_ShouldReturnOk_WhenProfileExists()
        {
            // Arrange
            var expectedProfile = new UserInfoDto
            {
                Email = "test@test.com",
                FullName = "Test User"
            };
            _mockUserService.Setup(x => x.GetUserProfileAsync()).ReturnsAsync(expectedProfile);

            // Act
            var result = await _userController.Profile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProfile = Assert.IsType<UserInfoDto>(okResult.Value);
            Assert.Equal(expectedProfile.Email, returnedProfile.Email);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldReturnOk_WhenUpdateSucceeds()
        {
            // Arrange
            var request = new UpdateUserProfileRequest
            {
                Email = "updated@test.com",
                FullName = "Updated User"
            };
            var response = new CommonMessageResponse { IsSuccess = true, Message = "Success" };
            _mockUserService.Setup(x => x.UpdateUserProfileAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _userController.UpdateUserProfile(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<CommonMessageResponse>(okResult.Value);
            Assert.True(returnedResponse.IsSuccess);
        }

        [Fact]
        public async Task CreateBooking_ShouldReturnOk_WhenBookingSucceeds()
        {
            // Arrange
            var bookingDto = new CreateBookingDto();
            var booking = new Booking { Id = 1, TotalAmount = 100 };
            _mockUserService.Setup(x => x.CreateBookingAsync(bookingDto, 1)).ReturnsAsync(booking);

            // Act
            var result = await _userController.CreateBooking(bookingDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdatePaymentStatus_ShouldReturnOk_WhenUpdateSucceeds()
        {
            // Arrange
            var paymentId = 1;
            var requestDto = new UpdatePaymentDto();
            var payment = new Payment { Id = paymentId };
            _mockUserService.Setup(x => x.UpdatePaymentStatusAsync(paymentId, requestDto))
                .ReturnsAsync(payment);

            // Act
            var result = await _userController.UpdatePaymentStatus(paymentId, requestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<Payment>(okResult.Value);
        }

        [Fact]
        public async Task DeleteExpiredBooking_ShouldReturnOk_WhenDeleteSucceeds()
        {
            // Arrange
            var paymentId = 1;
            _mockUserService.Setup(x => x.DeleteExpiredBookingAsync(paymentId))
                .ReturnsAsync(true);

            // Act
            var result = await _userController.DeleteExpiredBooking(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateTickets_ShouldReturnOk_WhenCreationSucceeds()
        {
            // Arrange
            var bookingId = 1;
            var tickets = new List<Ticket> { new Ticket { Id = 1 } };
            _mockUserService.Setup(x => x.CreateTicketsAsync(bookingId))
                .ReturnsAsync(tickets);

            // Act
            var result = await _userController.CreateTickets(bookingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTickets = Assert.IsType<List<Ticket>>(okResult.Value);
            Assert.Single(returnedTickets);
        }

        [Fact]
        public async Task CreatePayment_ShouldReturnOk_WhenPaymentUrlCreated()
        {
            // Arrange
            var request = new PaymentRequest
            {
                BookingId = 1,
                Amount = 100
            };
            var paymentUrl = "http://payment.url";
            _mockVnPayService.Setup(x => x.CreatePaymentUrl(
                It.IsAny<Booking>(),
                request.Locale,
                request.BankCode,
                request.OrderType
            )).ReturnsAsync(paymentUrl);

            // Act
            var result = await _userController.CreatePayment(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task VnPayReturn_ShouldReturnOk_AndSendEmail_WhenPaymentSucceeds()
        {
            // Arrange
            var paymentResponse = new PaymentResponse
            {
                ResponseCode = "00",
                TransactionStatus = "00"
            };
            _mockVnPayService.Setup(x => x.ProcessVnPayReturn(It.IsAny<IQueryCollection>()))
                .ReturnsAsync(paymentResponse);

            // Act
            var result = await _userController.VnPayReturn();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PaymentResponse>(okResult.Value);
            _mockUserService.Verify(x => x.SendEmailToId(paymentResponse, 1), Times.Once);
        }

        [Fact]
        public async Task ProcessPayment_ShouldHandleSuccessfulPayment()
        {
            // Arrange
            var request = new PaymentResponse
            {
                ResponseCode = "00",
                TransactionStatus = "00",
                BookingId = 1
            };

            // Act
            var result = await _userController.ProcessPayment(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockUserService.Verify(x => x.ProcessSuccessfulTicketAndPaymentAsync(
                request.BookingId, request), Times.Once);
        }

        [Fact]
        public async Task ProcessPayment_ShouldHandleFailedPayment()
        {
            // Arrange
            var request = new PaymentResponse
            {
                ResponseCode = "01",
                TransactionStatus = "01",
                BookingId = 1
            };

            // Act
            var result = await _userController.ProcessPayment(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockUserService.Verify(x => x.ProcessFailBookingAndSeatsAsync(request.BookingId), Times.Once);
        }
    }
}