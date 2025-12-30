using DirectoryService.Application.Locations;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILocationsService, LocationsService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
