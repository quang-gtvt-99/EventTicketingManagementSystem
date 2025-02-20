using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingManagementSystem.Worker.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;

namespace EventTicketingManagementSystemTests.Worker.BackgroundServices
{
    public class BookingTaskServiceTests
    {
        private readonly Mock<ILogger<BookingTaskService>> _mockLogger;
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IServiceScope> _mockServiceScope;
        private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;

        public BookingTaskServiceTests()
        {
            _mockLogger = new Mock<ILogger<BookingTaskService>>();
            _mockBookingService = new Mock<IBookingService>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockServiceScope = new Mock<IServiceScope>();
            _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            // Setup service scope
            _mockServiceScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
            _mockServiceScopeFactory.Setup(x => x.CreateScope()).Returns(_mockServiceScope.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(_mockServiceScopeFactory.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IBookingService)))
                .Returns(_mockBookingService.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallRemovePendingExpiredBookings()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var bookingTaskService = new BookingTaskService(_mockLogger.Object, _mockServiceProvider.Object);

            // Act
            // Start the service and cancel after a short delay to allow one execution
            var executionTask = bookingTaskService.StartAsync(cancellationTokenSource.Token);
            await Task.Delay(100); // Wait briefly to allow execution
            await bookingTaskService.StopAsync(cancellationTokenSource.Token);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Remove Pending Expired Bookings Task Is Running!")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.AtLeast(1)
            );

            _mockBookingService.Verify(x => x.RemovePendingExpiredBookings(), Times.AtLeast(1));
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleExceptions()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var expectedException = new Exception("Test exception");
            _mockBookingService.Setup(x => x.RemovePendingExpiredBookings())
                .ThrowsAsync(expectedException);

            var bookingTaskService = new BookingTaskService(_mockLogger.Object, _mockServiceProvider.Object);

            // Act
            var executionTask = bookingTaskService.StartAsync(cancellationTokenSource.Token);
            await Task.Delay(100); // Wait briefly to allow execution
            await bookingTaskService.StopAsync(cancellationTokenSource.Token);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    expectedException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.AtLeast(1)
            );
        }

        [Fact]
        public async Task StopAsync_ShouldStopExecution()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var bookingTaskService = new BookingTaskService(_mockLogger.Object, _mockServiceProvider.Object);

            // Act
            await bookingTaskService.StartAsync(cancellationTokenSource.Token);
            await Task.Delay(100); // Allow some execution time
            await bookingTaskService.StopAsync(cancellationTokenSource.Token);
            await Task.Delay(100); // Wait to ensure service has stopped

            // Get the execution count before stopping
            var executionCount = _mockBookingService.Invocations.Count;

            await Task.Delay(500); // Wait more time after stopping

            // Assert
            // Verify that no more executions occurred after stopping
            Assert.Equal(executionCount, _mockBookingService.Invocations.Count);
        }
    }
}