using diceclub_api_netcore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.Interfaces.Repositories
{
    public interface ICacheRedisRepository
    {
        Task HashSetAsync<T>(string key, string hashKey, T hashValue, CacheType cacheType, int cacheExpiraitonMinutes);
        Task<T?> HashGetAsync<T>(string key, string hashKey, CacheType cacheType);
    }
}
