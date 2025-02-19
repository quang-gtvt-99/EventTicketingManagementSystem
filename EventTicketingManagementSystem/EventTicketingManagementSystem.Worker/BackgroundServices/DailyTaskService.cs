using EventTicketingManagementSystem.Services.Services.Interfaces;

namespace EventTicketingManagementSystem.Worker.BackgroundServices
{
    public class DailyTaskService : BackgroundService
    {
        private readonly ILogger<DailyTaskService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DailyTaskService(ILogger<DailyTaskService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRunTime = DateTime.Today.AddDays(1); // 0h next day
                var delay = nextRunTime - now;

                _logger.LogInformation($"DailyTaskService is sleeping for {delay.TotalMilliseconds} milliseconds.");

                try
                {
                    await Task.Delay(delay, stoppingToken);

                    _logger.LogInformation("DailyTaskService is running.");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        // TODO: Change events status
                        await userService.GetAllUsersAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while executing the daily task.");
                }
            }
        }
    }
}
