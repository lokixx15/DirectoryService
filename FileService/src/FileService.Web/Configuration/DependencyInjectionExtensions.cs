using FileService.Core;
using FileService.Infrastructure.Postgres;
using FileService.Infrastructure.S3;
using SharedService.Framework.Endpoints;
using SharedService.Framework.Logging;

namespace FileService.Web.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddPostgresInfrastructure(configuration)
            .AddS3Infrastructure(configuration)
            .AddCore()
            .AddSerilogLogging(configuration)
            .AddWeb();

    private static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddEndpoints(typeof(DependencyInjectionCore).Assembly);

        return services;
    }
}