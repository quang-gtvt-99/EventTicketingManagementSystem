using EventTicketingManagementSystem.API.Response;
using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Services.Services.Implements;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EventTicketingManagementSystemTests.Service
{
    public class JwtAuthTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly JwtAuth _jwtAuth;
        private readonly string _secretKey = "your_test_secret_key_with_sufficient_length_for_hmacsha256";

        public JwtAuthTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["JwtKey"]).Returns(_secretKey);
            _jwtAuth = new JwtAuth(_mockUserRepository.Object, _mockCacheService.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Authentication_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var email = "nonexistent@test.com";
            var password = "password123";
            _mockUserRepository.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User?)null);

            // Act
            var result = await _jwtAuth.Authentication(email, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authentication_ShouldReturnNull_WhenPasswordInvalid()
        {
            // Arrange
            var email = "test@test.com";
            var password = "wrongpassword";
            var user = new User
            {
                Id = 1,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                FullName = "Test User"
            };

            _mockUserRepository.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockCacheService.Setup(x => x.IsCacheKeyExistAsync(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _jwtAuth.Authentication(email, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authentication_ShouldSucceed_WithValidPassword()
        {
            // Arrange
            var email = "test@test.com";
            var password = "correctpassword";
            var user = new User
            {
                Id = 1,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FullName = "Test User"
            };

            var roles = new List<string> { RoleConsts.User };

            _mockUserRepository.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.GetUserRolesAsync(user.Id)).ReturnsAsync(roles);
            _mockCacheService.Setup(x => x.IsCacheKeyExistAsync(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _jwtAuth.Authentication(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal(roles, result.Roles);
            Assert.NotNull(result.Token);
            _mockUserRepository.Verify(x => x.Update(It.Is<User>(u => 
                u.Id == user.Id && 
                u.LastLoginAt != null)), Times.Once);
        }

        [Fact]
        public async Task Authentication_ShouldSucceed_WithValidOTP()
        {
            // Arrange
            var email = "test@test.com";
            var otp = "123456";
            var hashedOtp = BCrypt.Net.BCrypt.HashPassword(otp);
            var user = new User
            {
                Id = 1,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("somepassword"),
                FullName = "Test User"
            };

            _mockUserRepository.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockCacheService.Setup(x => x.IsCacheKeyExistAsync($"{CacheKeyConsts.OneTimePassword}:{email}"))
                .ReturnsAsync(true);
            _mockCacheService.Setup(x => x.GetAsync<string>($"{CacheKeyConsts.OneTimePassword}:{email}"))
                .ReturnsAsync(hashedOtp);

            // Act
            var result = await _jwtAuth.Authentication(email, otp);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenJwtKeyNotConfigured()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["JwtKey"]).Returns((string?)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => new JwtAuth(
                _mockUserRepository.Object,
                _mockCacheService.Object,
                mockConfig.Object
            ));
            Assert.Equal("Cannot get JwtKey", exception.Message);
        }
    }
}