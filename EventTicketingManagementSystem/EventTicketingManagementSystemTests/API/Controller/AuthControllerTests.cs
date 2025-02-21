using EventTicketingManagementSystem.API.Controllers;
using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.API.Response;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventTicketingManagementSystemTests.API.Controller
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IJwtAuth> _mockJwtAuth;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockJwtAuth = new Mock<IJwtAuth>();
            _authController = new AuthController(_mockUserService.Object, _mockJwtAuth.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenRegistrationSucceeds()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "password123",
                FullName = "Test User",
                PhoneNumber = "1234567890"
            };

            var expectedResponse = new RegisterResponse
            {
                UserId = 1,
                Email = request.Email,
                FullName = request.FullName
            };

            _mockUserService.Setup(x => x.RegisterAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _authController.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RegisterResponse>(okResult.Value);
            Assert.Equal(expectedResponse.Email, response.Email);
            Assert.Equal(expectedResponse.FullName, response.FullName);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var expectedError = "Email already exists.";
            _mockUserService.Setup(x => x.RegisterAsync(request))
                .ThrowsAsync(new Exception(expectedError));

            // Act
            var result = await _authController.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic? errorResponse = badRequestResult?.Value;
            Assert.NotNull(errorResponse);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var authResponse = new AuthResult
            {
                Token = "jwt_token",
                Email = request.Email
            };

            _mockJwtAuth.Setup(x => x.Authentication(request.Email, request.Password))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _authController.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResult>(okResult.Value);
            Assert.Equal(authResponse.Token, response.Token);
            Assert.Equal(authResponse.Email, response.Email);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            _mockJwtAuth.Setup(x => x.Authentication(request.Email, request.Password))
                .ReturnsAsync((AuthResult?)null);

            // Act
            var result = await _authController.Login(request);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnOk_WhenEmailExists()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Email = "test@example.com"
            };

            var expectedResponse = new CommonMessageResponse
            {
                IsSuccess = true,
                Message = "OTP đã được gửi đến email của bạn!"
            };

            _mockUserService.Setup(x => x.ResetPasswordAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _authController.ResetPassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CommonMessageResponse>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(expectedResponse.Message, response.Message);
        }
    }
}