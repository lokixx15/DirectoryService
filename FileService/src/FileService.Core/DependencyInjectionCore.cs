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

        return services;
    }
}