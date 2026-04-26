using FileService.Core;
using FileService.Core.Abstractions.Database;
using FileService.Infrastructure.Postgres.Database;
using FileService.Infrastructure.Postgres.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Infrastructure.Postgres;

public static class DependencyInjectionPostgres
{
    public static IServiceCollection AddPostgresInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(_ => new FileServiceDbContext(
            configuration.GetConnectionString("FileServiceDb")!));

        services.AddScoped<IReadFileServiceDbContext, FileServiceDbContext>(
            _ => new FileServiceDbContext(configuration.GetConnectionString("FileServiceDb")!));

        services.AddScoped<ITransactionManager, TransactionManager>();

        services.AddScoped<IMediaRepository, MediaAssetRepository>();
        services.AddScoped<IVideoProcessRepository, VideoProcessRepository>();

        return services;
    }
}