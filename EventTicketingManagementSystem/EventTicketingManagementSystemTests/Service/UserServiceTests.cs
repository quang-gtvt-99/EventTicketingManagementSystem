using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.API.Response;
using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Services.Services.Implements;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Models;
using EventTicketingMananagementSystem.Core.Dtos;
using Moq;
using System.Text;
using EventTicketingManagementSystem.Response;

namespace EventTicketingManagementSystemTests.Service
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IPaymentRepository> _mockPaymentRepository;
        private readonly Mock<ITicketRepository> _mockTicketRepository;
        private readonly Mock<ISendMailService> _mockSendMailService;
        private readonly Mock<ISeatRepository> _mockSeatRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockPaymentRepository = new Mock<IPaymentRepository>();
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockSendMailService = new Mock<ISendMailService>();
            _mockSeatRepository = new Mock<ISeatRepository>();
            _mockCacheService = new Mock<ICacheService>();

            _userService = new UserService(
                _mockUserRepository.Object,
                _mockBookingRepository.Object,
                _mockCurrentUserService.Object,
                _mockPaymentRepository.Object,
                _mockTicketRepository.Object,
                _mockSendMailService.Object,
                _mockCacheService.Object,
                _mockSeatRepository.Object
            );
        }

        [Fact]
        public async Task GetUserProfileAsync_ShouldReturnUserProfile_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            _mockCurrentUserService.Setup(x => x.Id).Returns(userId.ToString());

            var user = new User { Id = userId, Email = "test@test.com", FullName = "Test User", PhoneNumber = "1234567890" };
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

            var bookings = new List<BookingInfoDto>();
            _mockBookingRepository.Setup(x => x.GetBookingInfosByUserIdAsync(userId)).ReturnsAsync(bookings);

            // Act
            var result = await _userService.GetUserProfileAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.FullName, result.FullName);
            Assert.Equal(user.PhoneNumber, result.PhoneNumber);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenEmailDoesNotExist()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "new@test.com",
                Password = "password123",
                FullName = "New User",
                PhoneNumber = "1234567890"
            };

            _mockUserRepository.Setup(x => x.UserEmailExisted(request.Email)).ReturnsAsync(false);
            _mockUserRepository.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(new User { Id = 1, Email = request.Email });

            // Act
            var result = await _userService.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.FullName, result.FullName);
            _mockUserRepository.Verify(x => x.AssignRoleAsync(It.IsAny<int>(), RoleConsts.User), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldSendOTP_WhenEmailExists()
        {
            // Arrange
            var request = new ResetPasswordRequest { Email = "test@test.com" };
            _mockUserRepository.Setup(x => x.UserEmailExisted(request.Email)).ReturnsAsync(true);

            // Act
            var result = await _userService.ResetPasswordAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("OTP đã được gửi đến email của bạn!", result.Message);
            _mockSendMailService.Verify(x => x.SendEmailAsync(
                It.Is<string>(email => email == request.Email),
                It.IsAny<string>(),
                It.IsAny<string>(),
                true
            ), Times.Once);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ShouldUpdateProfile_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            _mockCurrentUserService.Setup(x => x.Id).Returns(userId.ToString());

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                FullName = "Test User",
                PhoneNumber = "1234567890"
            };

            var request = new UpdateUserProfileRequest
            {
                Email = "updated@test.com",
                FullName = "Updated User",
                PhoneNumber = "0987654321",
                AllowChangePassword = false
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.UpdateUserProfileAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Cập nhật hồ sơ người dùng thành công!", result.Message);
        }

        [Fact]
        public async Task ProcessSuccessfulTicketAndPaymentAsync_ShouldCreateTicketsAndPayment()
        {
            // Arrange
            var bookingId = 1;
            var paymentResponse = new PaymentResponse { BookingId = bookingId };

            // Act
            await _userService.ProcessSuccessfulTicketAndPaymentAsync(bookingId, paymentResponse);

            // Assert
            _mockTicketRepository.Verify(x => x.CreateTicketsAsync(bookingId), Times.Once);
            _mockPaymentRepository.Verify(x => x.CreatePaymentAsync(paymentResponse), Times.Once);
        }
        [Fact]
        public async Task UpdateUserProfileAsync_ShouldChangePassword_WhenAllowChangePasswordIsTrue()
        {
            // Arrange
            var userId = 1;
            _mockCurrentUserService.Setup(x => x.Id).Returns(userId.ToString());

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                FullName = "Test User",
                PhoneNumber = "1234567890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword")
            };

            var request = new UpdateUserProfileRequest
            {
                Email = "updated@test.com",
                FullName = "Updated User",
                PhoneNumber = "0987654321",
                AllowChangePassword = true,
                OldPassword = "oldpassword",
                NewPassword = "newpassword123",
                ConfirmedNewPassword = "newpassword123"
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockCacheService.Setup(x => x.IsCacheKeyExistAsync(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _userService.UpdateUserProfileAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Cập nhật hồ sơ người dùng thành công!", result.Message);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ShouldReturnError_WhenPasswordChangeValidationFails()
        {
            // Arrange
            var userId = 1;
            _mockCurrentUserService.Setup(x => x.Id).Returns(userId.ToString());

            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                FullName = "Test User",
                PhoneNumber = "1234567890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
            };

            var request = new UpdateUserProfileRequest
            {
                Email = "updated@test.com",
                FullName = "Updated User",
                PhoneNumber = "0987654321",
                AllowChangePassword = true,
                OldPassword = "wrongpassword", // Wrong password
                NewPassword = "newpassword123",
                ConfirmedNewPassword = "newpassword123"
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockCacheService.Setup(x => x.IsCacheKeyExistAsync(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _userService.UpdateUserProfileAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Mật khẩu cũ không đúng.", result.Message);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task ProcessFailBookingAndSeatsAsync_ShouldUpdateSeatsAndDeleteBooking()
        {
            // Arrange
            var bookingId = 1;

            // Act
            await _userService.ProcessFailBookingAndSeatsAsync(bookingId);

            // Assert
            _mockSeatRepository.Verify(x => x.UpdateSeatsByBookingIdAsync(
                bookingId,
                CommConstants.CST_SEAT_STATUS_DEFAULT
            ), Times.Once);

            _mockBookingRepository.Verify(x => x.DeleteBookingByIdAsync(bookingId), Times.Once);
        }

        [Fact]
        public async Task SendEmailToId_ShouldSendEmail_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var userEmail = "test@test.com";
            var paymentResponse = new PaymentResponse
            {
                BookingId = 1,
                VnPayTranId = 123456,
                PayDate = DateTime.Now,
                TransactionStatus = "00",
                Amount = 100000,
                BankCode = "VCB",
                Message = "Success"
            };

            _mockUserRepository.Setup(x => x.GetEmailByIdAsync(userId))
                .Returns(userEmail);

            // Act
            await _userService.SendEmailToId(paymentResponse, userId);

            // Assert
            _mockSendMailService.Verify(x => x.SendEmailAsync(
                userEmail,
                $"Finiko - Đơn hàng mới: {paymentResponse.BookingId}",
                It.Is<string>(body =>
                    body.Contains(paymentResponse.BookingId.ToString()) &&
                    body.Contains(paymentResponse.VnPayTranId.ToString()) &&
                    body.Contains(paymentResponse.BankCode)
                ),
                true
            ), Times.Once);
        }

        [Fact]
        public async Task SendEmailToId_ShouldThrowException_WhenUserEmailNotFound()
        {
            // Arrange
            var userId = 999;
            var paymentResponse = new PaymentResponse { BookingId = 1 };

            _mockUserRepository.Setup(x => x.GetEmailByIdAsync(userId))
                .Returns((string)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                async () => await _userService.SendEmailToId(paymentResponse, userId)
            );
            Assert.Equal($"Không tìm thấy email cho người dùng có ID: {userId}", exception.Message);

            _mockSendMailService.Verify(x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            ), Times.Never);
        }
    }
}