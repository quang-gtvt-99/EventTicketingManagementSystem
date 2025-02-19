using EventTicketingManagementSystem.Services.Services.Interfaces;

namespace EventTicketingManagementSystem.Worker.BackgroundServices
{
    public class BookingTaskService : BackgroundService
    {
        private readonly ILogger<BookingTaskService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(60 * 2);

        public BookingTaskService(ILogger<BookingTaskService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Remove Pending Expired Bookings Task Is Running!");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                        await bookingService.RemovePendingExpiredBookings();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while executing the background service.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
