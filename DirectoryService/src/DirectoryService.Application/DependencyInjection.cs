using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
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
            
        return services;
    }
}