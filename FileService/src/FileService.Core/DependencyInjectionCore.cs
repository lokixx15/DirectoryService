using FileService.Core.Features;
using FileService.Domain;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Core;

public static class DependencyInjectionCore
{
    public static IServiceCollection AddCore(this IServiceCollection services)
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

        return services;
    }
}