using FileService.Core.Features;
using FileService.Domain;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace FileService.Core;

public static class DependencyInjectionCore
{
    public static IServiceCollection AddCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionCore).Assembly);

        services.AddScoped<IMediaAssetFactory, MediaAssetFactory>();

        services.AddScoped<UploadFileHandler>();
        services.AddScoped<DownloadFileHandler>();
        services.AddScoped<DeleteFileHandler>();
        services.AddScoped<GetDownloadUrlHandler>();
        services.AddScoped<AbortMultipartUploadHandler>();
        services.AddScoped<CompleteMultipartUploadHandler>();
        services.AddScoped<GetChunkUploadUrlHandler>();
        services.AddScoped<GetMediaAssetInfoHandler>();
        services.AddScoped<GetMediaAssetsInfoHandler>();
        services.AddScoped<StartMultipartUploadHandler>();
        services.AddScoped<CheckVideoExistenceHandler>();

        services.AddQuartzHostedService(configuration);

        return services;
    }

    private static IServiceCollection AddQuartzHostedService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
            options.AwaitApplicationStarted = true;
        });

        return services;
    }
}