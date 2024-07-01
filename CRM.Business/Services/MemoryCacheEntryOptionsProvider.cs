using Microsoft.Extensions.Caching.Memory;

namespace CRM.Business.Services;

public static class MemoryCacheEntryOptionsProvider
{
    public static MemoryCacheEntryOptions GetMemoryCacheEntryOptions() => _memoryCacheEntryOptions;
    
    private static readonly MemoryCacheEntryOptions _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(60))
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
        .SetPriority(CacheItemPriority.Normal)
        .SetSize(1024);
}