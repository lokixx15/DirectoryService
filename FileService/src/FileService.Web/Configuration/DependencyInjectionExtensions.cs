using FileService.Infrastructure.Postgres;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.Logging;

namespace FileService.Web.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddPostgresInfrastructure(configuration)
            .AddSerilogLogging(configuration)
            .AddWeb();

    private static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddOpenApi();

        return services;
    }
}