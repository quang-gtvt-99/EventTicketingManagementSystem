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
using EventTicketingMananagementSystem.Core.Enums;

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
                $"Event Ticketing System - Đơn hàng mới: {paymentResponse.BookingId}",
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
                .Returns((string?)null);

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

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
    {
        new User { Id = 1, Email = "user1@test.com" },
        new User { Id = 2, Email = "user2@test.com" }
    };
            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedUsers);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            var users = result.ToList();
            Assert.Equal(expectedUsers.Count, users.Count);
            Assert.Equal(expectedUsers[0].Email, users[0].Email);
            Assert.Equal(expectedUsers[1].Email, users[1].Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserInfo_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                Email = "test@test.com",
                FullName = "Test User",
                PhoneNumber = "1234567890",
                Status = "Active"
            };
            var roles = new List<string> { RoleConsts.Admin };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.GetUserRolesAsync(userId)).ReturnsAsync(roles);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.FullName, result.FullName);
            Assert.Equal(user.PhoneNumber, result.PhoneNumber);
            Assert.True(result.IsActive);
            Assert.Equal(UserRoles.ADMIN, result.Role);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetFilteredPagedUsersAsync_ShouldReturnFilteredUsers()
        {
            // Arrange
            var search = "test";
            var users = new List<User>
    {
        new User { Id = 1, Email = "test1@test.com", FullName = "Test User 1", Status = "Active" },
        new User { Id = 2, Email = "test2@test.com", FullName = "Test User 2", Status = "Inactive" }
    };
            _mockUserRepository.Setup(x => x.GetFilteredPagedAsync(search)).ReturnsAsync(users);

            // Act
            var result = await _userService.GetFilteredPagedUsersAsync(search);

            // Assert
            var userList = result.ToList();
            Assert.Equal(2, userList.Count);
            Assert.True(userList[0].IsActive);
            Assert.False(userList[1].IsActive);
            Assert.Equal(users[0].Email, userList[0].Email);
            Assert.Equal(users[1].Email, userList[1].Email);
        }

        [Fact]
        public async Task CreateUser_ShouldCreateUserAndAssignRole()
        {
            // Arrange
            var userRequest = new AddUpdateUserRequest
            {
                Email = "new@test.com",
                FullName = "New User",
                PhoneNumber = "1234567890",
                Password = "password123",
                Role = UserRoles.ADMIN
            };

            var createdUser = new User { Id = 1, Email = userRequest.Email };
            _mockUserRepository.Setup(x => x.UserEmailExisted(userRequest.Email)).ReturnsAsync(false);
            _mockUserRepository.Setup(x => x.FindByEmailAsync(userRequest.Email)).ReturnsAsync(createdUser);

            // Act
            var result = await _userService.CreateUser(userRequest);

            // Assert
            Assert.Equal(createdUser.Id, result);
            _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
            _mockUserRepository.Verify(x => x.AssignRoleAsync(createdUser.Id, RoleConsts.Admin), Times.Once);
        }

        [Fact]
        public async Task CreateUser_ShouldThrowException_WhenEmailExists()
        {
            // Arrange
            var userRequest = new AddUpdateUserRequest { Email = "existing@test.com" };
            _mockUserRepository.Setup(x => x.UserEmailExisted(userRequest.Email)).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                async () => await _userService.CreateUser(userRequest)
            );
            Assert.Equal("Email already exists.", exception.Message);
        }

        [Fact]
        public async Task UpdateUser_ShouldUpdateUserAndRole_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var userRequest = new AddUpdateUserRequest
            {
                ID = userId,
                Email = "update@test.com",
                FullName = "Updated User",
                PhoneNumber = "0987654321",
                IsActive = true,
                Role = UserRoles.ADMIN
            };

            var existingUser = new User { Id = userId };
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.UpdateUser(userRequest);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(x => x.Update(It.Is<User>(u =>
                u.Email == userRequest.Email &&
                u.FullName == userRequest.FullName &&
                u.PhoneNumber == userRequest.PhoneNumber &&
                u.Status == "Active"
            )), Times.Once);
            _mockUserRepository.Verify(x => x.AssignRoleAsync(userId, RoleConsts.Admin), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var userRequest = new AddUpdateUserRequest { ID = 999 };
            _mockUserRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.UpdateUser(userRequest);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId };
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.DeleteUser(userId);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(x => x.Delete(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.DeleteUser(userId);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(x => x.Delete(It.IsAny<User>()), Times.Never);
        }
    }
}