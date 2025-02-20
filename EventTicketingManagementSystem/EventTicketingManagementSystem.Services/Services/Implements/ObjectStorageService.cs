using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using Amazon.S3.Model;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EventTicketingManagementSystem.Services.Services.Implements
{
    [ExcludeFromCodeCoverage]
    public class ObjectStorageService : IObjectStorageService
    {
        private readonly AmazonS3Client s3Client;
        private readonly ILogger<ObjectStorageService> logger;

        public ObjectStorageService(IConfiguration configuration, ILogger<ObjectStorageService> logger)
        {
            this.logger = logger;

            string? accessKey = configuration["SPACES_KEY"];
            string? secretKey = configuration["SPACES_SECRET"];
            string? serviceUrl = configuration["SPACES_SERVICE_URL"];

            if (AreInvalidStrings(accessKey, secretKey, serviceUrl))
            {
                throw new Exception("SPACES_KEY, SPACES_SECRET, or SPACES_SERVICE_URL configuration not set");
            }

            AmazonS3Config config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true,
            };

            var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
            s3Client = new AmazonS3Client(credentials, config);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string bucketName)
        {
            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName,
                    InputStream = fileStream,
                    ContentType = fileName.GetContentType(),
                    CannedACL = S3CannedACL.PublicRead // Make file public
                };

                PutObjectResponse response = await s3Client.PutObjectAsync(putRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Construct the file URL
                    string fileUrl = $"{s3Client.Config.ServiceURL}{bucketName}/{fileName}";
                    return fileUrl;
                }
                else
                {
                    throw new Exception($"File upload failed with status code: {response.HttpStatusCode}");
                }
            }
            catch (AmazonS3Exception ex)
            {
                logger.LogError(ex, $"Error encountered on server. Message:'{ex.Message}' when writing an object");
                throw new Exception($"Error encountered on server. Message:'{ex.Message}' when writing an object", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unknown error encountered on server. Message:'{ex.Message}' when writing an object");
                throw new Exception($"Unknown error encountered on server. Message:'{ex.Message}' when writing an object", ex);
            }
        }

        public async Task<Stream> GetFileAsync(string fileName, string bucketName)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName
                };

                using GetObjectResponse response = await s3Client.GetObjectAsync(request);
                MemoryStream memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (AmazonS3Exception ex)
            {
                logger.LogError(ex, $"Error encountered on server. Message:'{ex.Message}' when reading an object");
                throw new Exception($"Error encountered on server. Message:'{ex.Message}' when reading an object", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unknown error encountered on server. Message:'{ex.Message}' when reading an object");
                throw new Exception($"Unknown error encountered on server. Message:'{ex.Message}' when reading an object", ex);
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName, string bucketName)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName
                };

                DeleteObjectResponse response = await s3Client.DeleteObjectAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return true;
                }
                else
                {
                    logger.LogError($"File deletion failed with status code: {response.HttpStatusCode}");
                    return false;
                }
            }
            catch (AmazonS3Exception ex)
            {
                logger.LogError(ex, $"Error encountered on server. Message:'{ex.Message}' when deleting an object");
                throw new Exception($"Error encountered on server. Message:'{ex.Message}' when deleting an object", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unknown error encountered on server. Message:'{ex.Message}' when deleting an object");
                throw new Exception($"Unknown error encountered on server. Message:'{ex.Message}' when deleting an object", ex);
            }
        }

        #region Private Methods
        private bool AreInvalidStrings(params string?[] strings)
        {
            foreach (string? str in strings)
            {
                if (string.IsNullOrEmpty(str))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
