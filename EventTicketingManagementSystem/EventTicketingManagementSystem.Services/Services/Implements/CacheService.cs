using EventTicketingManagementSystem.Services.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace EventTicketingManagementSystem.Services.Services.Implements
{
    public class CacheService : ICacheService
    {
        private readonly ConnectionMultiplexer redis;
        private readonly IDatabase db;
        private bool disposed = false;

        public CacheService(IConfiguration configuration)
        {
            var endpoint = configuration["RedisCache_Endpoint"];
            var password = configuration["RedisCache_Password"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(password))
            {
                throw new Exception("RedisCache_Endpoint or RedisCache_Password configuration not set");
            }

            redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { endpoint },
                Password = password,
                Ssl = true,
                AbortOnConnectFail = false
            });

            db = redis.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await db.StringGetAsync(key);
            if (value.HasValue)
            {
                return JsonSerializer.Deserialize<T>(value.ToString());
            }
            return default;
        }

        public async Task<bool> SetAsync<T>(string key, T value, int expirationSecond = 300)
        {
            var expiration = new TimeSpan(0, 0, expirationSecond);
            var serializedValue = JsonSerializer.Serialize(value);
            return await db.StringSetAsync(key, serializedValue, expiration);
        }

        public async Task ClearCacheAsync()
        {
            var endpoints = redis.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = redis.GetServer(endpoint);
                var keys = server.Keys();
                foreach (var key in keys)
                {
                    await db.KeyDeleteAsync(key);
                }
            }
        }

        public async Task<bool> InvalidCacheAsync(string key) => await db.KeyDeleteAsync(key);

        public async Task<bool> IsCacheKeyExistAsync(string key) => await db.KeyExistsAsync(key);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    redis.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
