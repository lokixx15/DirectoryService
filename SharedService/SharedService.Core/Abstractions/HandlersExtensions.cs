using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharedService.Core.Abstractions;

public static class HandlersExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly[] assemblies)
    {
        services.Scan(scan => scan.FromAssemblies(assemblies)
        .AddClasses(classes => classes
        .AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
        .AsSelfWithInterfaces()
        .WithScopedLifetime());

        services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes
            .AssignableToAny(typeof(IQueryHandler<,>), typeof(IQueryHandler<>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        return services;
    }
}