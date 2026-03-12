using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.Logging;

namespace DirectoryService.Presentation.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSerilogLogging(configuration)
            .AddWeb();

    private static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddOpenApi();

        return services;
    }
}