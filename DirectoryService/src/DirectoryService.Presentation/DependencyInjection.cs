using DirectoryService.Application;
using DirectoryService.Infrastructure;

namespace DirectoryService.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services) =>
        services
            .AddApplication()
            .AddInfrastructure()
            .AddWeb();

    private static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen();

        return services;
    }
}
