
namespace EventTicketingManagementSystem.Services.Interfaces
{
    public interface ICacheService
    {
        Task ClearCacheAsync();
        Task<T?> GetAsync<T>(string key);
        Task<bool> InvalidCacheAsync(string key);
        Task<bool> SetAsync<T>(string key, T value, int expirationSecond = 300);
    }
}
