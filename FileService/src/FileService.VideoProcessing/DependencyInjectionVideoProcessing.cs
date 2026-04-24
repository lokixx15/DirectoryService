using FileService.VideoProcessing.FfmpegProcess;
using FileService.VideoProcessing.Pipeline;
using FileService.VideoProcessing.ProcessExecutor;
using FileService.VideoProcessing.Steps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.VideoProcessing;

public static class DependencyInjectionVideoProcessing
{
    public static IServiceCollection AddVideoProcessing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IFfmpegProcessRunner, FfmpegProcessRunner>();

        services.AddScoped<IProcessRunner, ProcessRunner>();

        services.AddScoped<IProcessingStepHandler, InitializeStepHandler>();
        services.AddScoped<IProcessingStepHandler, ExtractMetadataStepHandler>();
        services.AddScoped<IProcessingStepHandler, GenerateHlsStepHandler>();
        services.AddScoped<IProcessingStepHandler, UploadHlsStepHandler>();
        services.AddScoped<IProcessingStepHandler, CleanupStepHandler>();

        services.AddScoped<IProcessingPipeline, ProcessingPipeline>();

        services.Configure<VideoProcessingOptions>(configuration.GetSection(
            VideoProcessingOptions.SECTION_NAME));

        services.AddScoped<IVideoProcessingService, VideoProcessingService>();

        return services;
    }
}