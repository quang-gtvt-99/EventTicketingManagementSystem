using System.Diagnostics.CodeAnalysis;
using EventTicketingMananagementSystem.Core.Utilities;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace EventTicketingManagementSystem.HealthChecks
{
    [ExcludeFromCodeCoverage]
    public class RedisHealthCheck : IHealthCheck, IDisposable
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private const int LATENCY_THRESHOLD_MS = 1000;

        public RedisHealthCheck(IConfiguration configuration)
        {
            var endpoint = Utils.GetConfigurationValue(configuration, "RedisCache_Endpoint");
            var password = Utils.GetConfigurationValue(configuration, "RedisCache_Password");

            var config = new ConfigurationOptions
            {
                EndPoints = { endpoint },
                Password = password,
                Ssl = true,
                AbortOnConnectFail = false
            };

            _redis = ConnectionMultiplexer.Connect(config);
            _db = _redis.GetDatabase();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var ping = await _db.PingAsync();
                var latency = ping.TotalMilliseconds;

                var data = new Dictionary<string, object>
                {
                    { "Latency", $"{latency:0.00}ms" },
                    { "IsConnected", _redis.IsConnected },
                    { "Performance", latency < LATENCY_THRESHOLD_MS ? "Good" : "Degraded" }
                };

                var description = latency < LATENCY_THRESHOLD_MS
                    ? "Redis connection is healthy"
                    : $"Redis connection is degraded (latency: {latency:0.00}ms)";

                return latency < LATENCY_THRESHOLD_MS
                    ? HealthCheckResult.Healthy(description, data)
                    : HealthCheckResult.Degraded(description, null, data);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Redis connection failed", 
                    exception: ex,
                    data: new Dictionary<string, object>
                    {
                        { "IsConnected", false },
                        { "Error", "Connection error" }
                    });
            }
        }

        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}