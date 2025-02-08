using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

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

            // TODO  System.Text.Json.JsonException: A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 64.
            //await cache.SetStringAsync(key, JsonSerializer.Serialize(value), options ?? Default, cancellationToken);

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