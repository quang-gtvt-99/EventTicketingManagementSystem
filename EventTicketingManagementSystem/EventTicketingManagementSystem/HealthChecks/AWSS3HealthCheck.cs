using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using EventTicketingMananagementSystem.Core.Utilities;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EventTicketingManagementSystem.HealthChecks
{
    [ExcludeFromCodeCoverage]
    public class AWSS3HealthCheck : IHealthCheck
    {
        private readonly AmazonS3Client _s3Client;

        public AWSS3HealthCheck(IConfiguration configuration)
        {
            string accessKey = Utils.GetConfigurationValue(configuration, "SPACES_KEY");
            string secretKey = Utils.GetConfigurationValue(configuration, "SPACES_SECRET");
            string serviceUrl = Utils.GetConfigurationValue(configuration, "SPACES_SERVICE_URL");

            var config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true,
            };

            var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
            _s3Client = new AmazonS3Client(credentials, config);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _s3Client.ListBucketsAsync(cancellationToken);
                return HealthCheckResult.Healthy("AWS S3 connection is healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("AWS S3 connection is unhealthy", ex);
            }
        }
    }
}