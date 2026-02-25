using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Caching;
using FluentValidation;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        var assembly = typeof(DependencyInjection).Assembly;
        services.AddValidatorsFromAssembly(assembly);

        services.Scan(scan => scan.FromAssemblies(assembly)
        .AddClasses(classes => classes
        .AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
        .AsSelfWithInterfaces()
        .WithScopedLifetime());

        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes
            .AssignableToAny(typeof(IQueryHandler<,>), typeof(IQueryHandler<>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

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