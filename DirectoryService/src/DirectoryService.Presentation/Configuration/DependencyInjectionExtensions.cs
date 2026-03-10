using DirectoryService.Application;
using DirectoryService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.Logging;

namespace DirectoryService.Presentation.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSerilogLogging(configuration)
            .AddApplication(configuration)
            .AddInfrastructure(configuration)
            .AddWeb();

    private static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();

        services.Configure<ApiBehaviorOptions>(options =>
        options.SuppressModelStateInvalidFilter = true);

        services.AddOpenApi();

        return services;
    }
}