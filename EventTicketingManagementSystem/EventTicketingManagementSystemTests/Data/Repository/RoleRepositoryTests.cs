using EventTicketingManagementSystem.Data.Data.Repository.Implement;
using EventTicketingMananagementSystem.Core.Models;
using EventTicketingMananagementSystem.Core.Dtos;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventTicketingManagementSystemTests.Data.Repository
{
    public class RoleRepositoryTests : BaseRepositoryTests
    {
        private readonly RoleRepository _roleRepository;
        private readonly Mock<ILogger<RoleRepository>> _mockLogger;

        public RoleRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<RoleRepository>>();
            _roleRepository = new RoleRepository(Context, _mockLogger.Object);
            SeedData();
        }

        private void SeedData()
        {
            var roles = new List<Role>
            {
                new Role 
                { 
                    Id = 1, 
                    Name = "Admin", 
                    Description = "Administrator role" 
                },
                new Role 
                { 
                    Id = 2, 
                    Name = "User", 
                    Description = "Regular user role" 
                }
            };

            Context.Roles.AddRange(roles);
            Context.SaveChanges();
        }

        [Fact]
        public async Task GetRoleByName_ShouldReturnRole_WhenRoleExists()
        {
            // Arrange
            var roleName = "Admin";

            // Act
            var role = await _roleRepository.GetRoleByName(roleName);

            // Assert
            Assert.NotNull(role);
            Assert.Equal(roleName, role.Name);
            Assert.Equal("Administrator role", role.Description);
        }

        [Fact]
        public async Task GetRoleByName_ShouldReturnNull_WhenRoleDoesNotExist()
        {
            // Arrange
            var nonExistentRoleName = "NonExistentRole";

            // Act
            var role = await _roleRepository.GetRoleByName(nonExistentRoleName);

            // Assert
            Assert.Null(role);
        }

        [Fact]
        public async Task GetRoleById_ShouldReturnRole_WhenRoleExists()
        {
            // Arrange
            var roleId = 1;

            // Act
            var role = await _roleRepository.GetRoleById(roleId);

            // Assert
            Assert.NotNull(role);
            Assert.Equal(roleId, role.Id);
            Assert.Equal("Admin", role.Name);
        }

        [Fact]
        public async Task GetRoleById_ShouldReturnNull_WhenRoleDoesNotExist()
        {
            // Arrange
            var nonExistentRoleId = 999;

            // Act
            var role = await _roleRepository.GetRoleById(nonExistentRoleId);

            // Assert
            Assert.Null(role);
        }

        [Fact]
        public async Task GetRoleByIds_ShouldReturnRoles_WhenRolesExist()
        {
            // Arrange
            var roleIds = new List<int> { 1, 2 };

            // Act
            var roles = await _roleRepository.GetRoleByIds(roleIds);

            // Assert
            Assert.NotNull(roles);
            Assert.Equal(2, roles.Count);
            Assert.Contains(roles, r => r.Id == 1 && r.Name == "Admin");
            Assert.Contains(roles, r => r.Id == 2 && r.Name == "User");
        }

        [Fact]
        public async Task GetRoleByIds_ShouldReturnEmptyList_WhenRolesDoNotExist()
        {
            // Arrange
            var nonExistentRoleIds = new List<int> { 998, 999 };

            // Act
            var roles = await _roleRepository.GetRoleByIds(nonExistentRoleIds);

            // Assert
            Assert.NotNull(roles);
            Assert.Empty(roles);
        }

        [Fact]
        public async Task GetRoleByIds_ShouldReturnPartialList_WhenSomeRolesExist()
        {
            // Arrange
            var roleIds = new List<int> { 1, 999 };

            // Act
            var roles = await _roleRepository.GetRoleByIds(roleIds);

            // Assert
            Assert.NotNull(roles);
            Assert.Single(roles);
            Assert.Contains(roles, r => r.Id == 1 && r.Name == "Admin");
        }
    }
}