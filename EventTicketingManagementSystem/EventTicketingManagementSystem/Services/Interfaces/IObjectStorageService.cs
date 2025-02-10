
namespace EventTicketingManagementSystem.Services.Interfaces
{
    public interface IObjectStorageService
    {
        Task<bool> DeleteFileAsync(string fileName, string bucketName);
        Task<Stream> GetFileAsync(string fileName, string bucketName);
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string bucketName);
    }
}
