using CSharpFunctionalExtensions;
using FileService.VideoProcessing.Pipeline;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing;

public class VideoProcessingService : IVideoProcessingService
{
    private readonly IProcessingPipeline _processingPipeline;
    private readonly ILogger<VideoProcessingService> _logger;

    public VideoProcessingService(
        IProcessingPipeline processingPipeline,
        ILogger<VideoProcessingService> logger)
    {
        _processingPipeline = processingPipeline;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> ProcessVideoAsync(
        Guid videoAssetId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting video processing for video asset with id {VideoAssetId}", videoAssetId);

        return await _processingPipeline.ProcessAllStepsAsync(videoAssetId, cancellationToken);
    }
}