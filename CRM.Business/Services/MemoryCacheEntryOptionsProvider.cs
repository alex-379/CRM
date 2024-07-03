using Microsoft.Extensions.Caching.Memory;

namespace CRM.Business.Services;

public static class MemoryCacheEntryOptionsProvider
{
    public static MemoryCacheEntryOptions GetMemoryCacheEntryOptionsForToken() => _memoryCacheEntryOptionsForToken;
    
    private static readonly MemoryCacheEntryOptions _memoryCacheEntryOptionsForToken = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(60))
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
        .SetPriority(CacheItemPriority.Normal)
        .SetSize(1024);
}