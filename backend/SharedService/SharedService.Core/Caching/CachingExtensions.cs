using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedService.Core.Caching;

public static class CachingExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(setup =>
        {
            setup.Configuration = configuration.GetConnectionString("Redis");
        });

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions()
            {
                LocalCacheExpiration = configuration.GetValue<TimeSpan>("HybridCache:LocalCacheExpiration"),
                Expiration = configuration.GetValue<TimeSpan>("HybridCache:Expiration")
            };
        });

        return services;
    }
}