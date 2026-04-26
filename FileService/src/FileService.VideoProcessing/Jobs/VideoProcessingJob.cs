using Microsoft.Extensions.Logging;
using Quartz;

namespace FileService.VideoProcessing.Jobs;

[DisallowConcurrentExecution]
public class VideoProcessingJob : IJob
{
    public static readonly JobKey VideoAssetIdKey = new("VideoAssetId");

    private readonly IVideoProcessingService _videoProcessingService;
    private readonly ILogger<VideoProcessingJob> _logger;

    public VideoProcessingJob(
        IVideoProcessingService videoProcessingService,
        ILogger<VideoProcessingJob> logger)
    {
        _videoProcessingService = videoProcessingService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.MergedJobDataMap;
        Guid videoAssetId = dataMap.GetGuid(VideoAssetIdKey.Name);

        _logger.LogInformation("Start video processing for video asset with id {VideoAssetId}",
            videoAssetId);

        var result = await _videoProcessingService.ProcessVideoAsync(videoAssetId,
            context.CancellationToken);

        if (result.IsFailure)
        {
            _logger.LogError("Video processing failed for for video asset with id {VideoAssetId}",
                videoAssetId);

            throw new JobExecutionException(refireImmediately: false);
        }
    }
}