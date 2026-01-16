using DirectoryService.Application;
using DirectoryService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DirectoryService.Presentation.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSerilogLogging(configuration)
            .AddApplication()
            .AddInfrastructure()
            .AddWeb();

    private static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();

        services.Configure<ApiBehaviorOptions>(options =>
        options.SuppressModelStateInvalidFilter = true);

        services.AddOpenApi();

        return services;
    }

    private static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(configuration)
        .ReadFrom.Services(services)
        .Enrich.WithProperty("ServiceName", "DirectoryService"));

        return services;
    }
}