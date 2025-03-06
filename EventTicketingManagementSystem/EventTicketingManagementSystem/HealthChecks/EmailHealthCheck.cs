using System.Diagnostics.CodeAnalysis;
using EventTicketingMananagementSystem.Core.Utilities;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EventTicketingManagementSystem.HealthChecks
{
    [ExcludeFromCodeCoverage]
    public class EmailHealthCheck : IHealthCheck
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public EmailHealthCheck(IConfiguration configuration)
        {
            _smtpServer = Utils.GetConfigurationValue(configuration, "EmailSettings_SmtpServer");
            _port = int.Parse(Utils.GetConfigurationValue(configuration, "EmailSettings_Port"));
            _username = Utils.GetConfigurationValue(configuration, "EmailSettings_Username");
            _password = Utils.GetConfigurationValue(configuration, "EmailSettings_Password");
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _port, true, cancellationToken);
                    await client.AuthenticateAsync(_username, _password, cancellationToken);
                    await client.DisconnectAsync(true, cancellationToken);

                    return HealthCheckResult.Healthy("Email service is healthy");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Email service is unhealthy", ex);
            }
        }
    }
}