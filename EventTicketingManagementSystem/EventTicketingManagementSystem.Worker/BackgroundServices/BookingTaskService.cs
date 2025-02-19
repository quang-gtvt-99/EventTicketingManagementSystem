using EventTicketingManagementSystem.Services.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventTicketingManagementSystem.Worker.BackgroundServices
{
    public class BookingTaskService : BackgroundService
    {
        private readonly ILogger<BookingTaskService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(60);

        public BookingTaskService(ILogger<BookingTaskService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("BookingTaskService is running.");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        // TODO: Change to revert booking when expired
                        await userService.GetAllUsersAsync();
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
