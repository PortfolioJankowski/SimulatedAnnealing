using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;

public static class CacheAside
{
    private static readonly DistributedCacheEntryOptions Default = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
    };
    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
    public static async Task<T?> GetOrCreateAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<CancellationToken, Task<T>> factory,
        DistributedCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var cachedValue = await cache.GetStringAsync(key, cancellationToken);
        
        T? value;
        if (!string.IsNullOrEmpty(cachedValue))
        {
            value = JsonConvert.DeserializeObject<T>(cachedValue);
            if (value is not null)
            {
                return value;
            }
        }
        var hasLock = await Semaphore.WaitAsync(500);

        if (!hasLock)
        {
            return default;
        }

        try
        {
            cachedValue = await cache.GetStringAsync(key, cancellationToken);

            value = await factory(cancellationToken);

            if (value is null)
            {
                return default;
            }

            var serialized = JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            await cache.SetStringAsync(key, serialized, options ?? Default, cancellationToken);
        }
        finally
        {
            Semaphore.Release();
        }
       
        return value;
    }
}