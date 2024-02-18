using diceclub_api_netcore.Domain;
using diceclub_api_netcore.Domain.Enums;
using diceclub_api_netcore.Domain.Interfaces.Repositories;
using StackExchange.Redis;
using System.Text.Json;

namespace diceclub_api_netcore.Infrastructure.Repositories
{
    public class CacheRedisRepository : ICacheRedisRepository
    {
        private readonly IDatabase cache;

        public CacheRedisRepository(IConnectionMultiplexer connection)
        {
            this.cache = connection.GetDatabase();
        }

        public async Task HashSetAsync<T>(string key, string hashKey, T hashValue, CacheType cacheType, int cacheExpirationMinutes)
        {
            var redisKey = GetRedisKey(key, cacheType);
            var redisExpiration = new TimeSpan(0, (int)cacheExpirationMinutes, 0);
            var newHash = new HashEntry(hashKey, JsonSerializer.Serialize(hashValue));

            await cache.HashSetAsync(redisKey, [newHash]);
            await cache.KeyExpireAsync(redisKey, redisExpiration);
        }

        public async Task<T?> HashGetAsync<T>(string key, string hashKey, CacheType cacheType)
        {
            var redisKey = GetRedisKey(key, cacheType);
            var hashValue = await cache.HashGetAsync(redisKey, hashKey);

            if(hashValue.HasValue)
            {
                var value = JsonSerializer.Deserialize<T>(hashValue!);

                return value;
            }

            return default;
        }

        private string GetRedisKey(string key, CacheType type)
        {
            if (!string.IsNullOrEmpty(key))
            {
                return $"{Parameters.Redis.Name}:{type}:{key}";
            }
            return $"{Parameters.Redis.Name}:{type}";
        }
    }
}
