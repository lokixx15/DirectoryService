using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgresInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(_ => new FileServiceDbContext(
            configuration.GetConnectionString("FileServiceDb")!));

        return services;
    }
}
