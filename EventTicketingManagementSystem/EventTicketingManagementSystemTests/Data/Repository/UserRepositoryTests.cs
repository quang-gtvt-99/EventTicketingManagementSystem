using EventTicketingManagementSystem.Data.Data.Repository.Implement;
using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventTicketingManagementSystemTests.Data.Repository
{
    public class UserRepositoryTests : BaseRepositoryTests
    {
        private readonly UserRepository _userRepository;
        private readonly Mock<IRoleRepository> _mockRoleRepository;

        public UserRepositoryTests()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();
            _userRepository = new UserRepository(Context, _mockRoleRepository.Object);
            SeedData();
        }

        private void SeedData()
        {
            var users = new List<User>
            {
                new User { Id = 1, Email = "user1@example.com", FullName = "User One", PasswordHash = "hash1", PhoneNumber = "1234567890" },
                new User { Id = 2, Email = "user2@example.com", FullName = "User Two", PasswordHash = "hash2", PhoneNumber = "0987654321" }
            };

            Context.Users.AddRange(users);
            Context.SaveChanges();
        }

        [Fact]
        public async Task FindByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            // Arrange
            var email = "user1@example.com";

            // Act
            var user = await _userRepository.FindByEmailAsync(email);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(email, user.Email);
        }

        [Fact]
        public async Task FindByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";

            // Act
            var user = await _userRepository.FindByEmailAsync(email);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task UserEmailExisted_ShouldReturnTrue_WhenEmailExists()
        {
            // Arrange
            var email = "user1@example.com";

            // Act
            var exists = await _userRepository.UserEmailExisted(email);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task UserEmailExisted_ShouldReturnFalse_WhenEmailDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";

            // Act
            var exists = await _userRepository.UserEmailExisted(email);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task GetUserRolesAsync_ShouldReturnRoles_WhenUserHasRoles()
        {
            // Arrange
            var userId = 1;
            var roleIds = new List<int> { 1, 2 };
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            };

            Context.UserRoles.AddRange(new List<UserRole>
            {
                new UserRole { UserId = userId, RoleId = 1 },
                new UserRole { UserId = userId, RoleId = 2 }
            });
            Context.SaveChanges();

            _mockRoleRepository.Setup(r => r.GetRoleByIds(It.IsAny<List<int>>()))
                .ReturnsAsync(roles.Select(role => new RoleDto { Id = role.Id, Name = role.Name }).ToList());
            // Act
            var userRoles = await _userRepository.GetUserRolesAsync(userId);

            // Assert
            Assert.Equal(2, userRoles.Count);
            Assert.Contains("Admin", userRoles);
            Assert.Contains("User", userRoles);
        }

        [Fact]
        public async Task AssignRoleAsync_ShouldAssignRoleToUser()
        {
            // Arrange
            var userId = 1;
            var roleName = "Admin";
            var role = new Role { Id = 1, Name = roleName };

            _mockRoleRepository.Setup(r => r.GetRoleByName(roleName)).ReturnsAsync(new RoleDto { Id = role.Id, Name = role.Name });

            // Act
            await _userRepository.AssignRoleAsync(userId, roleName);
            await Context.SaveChangesAsync();

            // Assert
            var userRole = await Context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);
            Assert.NotNull(userRole);
            Assert.Equal(userId, userRole.UserId);
            Assert.Equal(role.Id, userRole.RoleId);
        }
    }
}
