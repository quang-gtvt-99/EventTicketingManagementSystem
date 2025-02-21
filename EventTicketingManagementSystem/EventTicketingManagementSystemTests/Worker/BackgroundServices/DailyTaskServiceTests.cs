using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingManagementSystem.Worker.BackgroundServices;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;

namespace EventTicketingManagementSystemTests.Worker.BackgroundServices
{
    public class DailyTaskServiceTests
    {
        private readonly Mock<ILogger<DailyTaskService>> _mockLogger;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IServiceScope> _mockServiceScope;
        private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;

        public DailyTaskServiceTests()
        {
            _mockLogger = new Mock<ILogger<DailyTaskService>>();
            _mockUserService = new Mock<IUserService>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockServiceScope = new Mock<IServiceScope>();
            _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            // Setup service scope properly
            var scopeServiceProvider = new Mock<IServiceProvider>();
            scopeServiceProvider.Setup(x => x.GetService(typeof(IUserService)))
                .Returns(_mockUserService.Object);

            _mockServiceScope.Setup(x => x.ServiceProvider)
                .Returns(scopeServiceProvider.Object);
            _mockServiceScopeFactory.Setup(x => x.CreateScope())
                .Returns(_mockServiceScope.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(_mockServiceScopeFactory.Object);

            // Setup mock user service
            _mockUserService.Setup(u => u.GetAllUsersAsync())
                .ReturnsAsync(new List<User>());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallGetAllUsersAsync()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var dailyTaskService = new DailyTaskService(_mockLogger.Object, _mockServiceProvider.Object);

            try
            {
                // Act
                await dailyTaskService.StartAsync(cancellationTokenSource.Token);

                // Assert
                _mockUserService.Verify(x => x.GetAllUsersAsync(), Times.AtLeast(1));
            }
            finally
            {
                // Cleanup
                await dailyTaskService.StopAsync(cancellationTokenSource.Token);
                cancellationTokenSource.Dispose();
            }
        }

        [Fact]
        public async Task StopAsync_ShouldStopExecution()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var dailyTaskService = new DailyTaskService(_mockLogger.Object, _mockServiceProvider.Object);

            // Act
            await dailyTaskService.StartAsync(cancellationTokenSource.Token);
            var executionCount = _mockUserService.Invocations.Count;

            await dailyTaskService.StopAsync(cancellationTokenSource.Token);

            // Assert
            Assert.Equal(executionCount, _mockUserService.Invocations.Count);

            // Cleanup
            cancellationTokenSource.Dispose();
        }
    }
}