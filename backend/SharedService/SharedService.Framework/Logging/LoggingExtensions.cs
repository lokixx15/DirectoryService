using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace SharedService.Framework.Logging;

public static class LoggingExtensions
{
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(configuration)
        .ReadFrom.Services(services)
        .Enrich.WithProperty("ServiceName", "DirectoryService"));

        return services;
    }
}