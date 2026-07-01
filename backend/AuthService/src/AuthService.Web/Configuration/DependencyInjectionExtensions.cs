using AuthService.Infrastructure.Postgres;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Presentation.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddWeb();

        return services;
    }

    private static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();

        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddOpenApi();

        return services;
    }
}
