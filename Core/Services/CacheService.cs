using Domain.Contracts;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services;
public class CacheService(ICacheRepository cacheRepositorhy) : ICacheService
{
    public async Task<string?> GetCacheValueAsync(string key)
    {
       var value = await cacheRepositorhy.GetAsync(key);

        return value == null ? null : value;
    }

    public async Task SetCacheValueAsync(string key, string value, TimeSpan duration)
    {
         await cacheRepositorhy.SetAsync(key, value, duration);
    }
}
