using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Quartz;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.Jobs;

public class VideoProcessingScheduler : IVideoProcessingScheduler
{
    private const string JOB_GROUP = "video-processing";

    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ILogger<VideoProcessingScheduler> _logger;

    public VideoProcessingScheduler(
        ISchedulerFactory schedulerFactory,
        ILogger<VideoProcessingScheduler> logger)
    {
        _schedulerFactory = schedulerFactory;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> ScheduleProcessingAsync(Guid videoAssetId, CancellationToken cancellationToken)
    {
        try
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            await scheduler.ScheduleJob(CreateJob(videoAssetId), CreateTrigger(videoAssetId), cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule video processing for {VideoAssetId}", videoAssetId);
            return Error.Failure("video_processing.schedule_failed", $"Failed to schedule video processing job for asset {videoAssetId}");
        }
    }

    private IJobDetail CreateJob(Guid mediaAssetId)
    {
        return JobBuilder.Create<VideoProcessingJob>()
            .WithIdentity($"video-processing-{mediaAssetId}", JOB_GROUP)
            .UsingJobData(VideoProcessingJob.VideoAssetIdKey.Name, mediaAssetId.ToString())
            .StoreDurably(true)
            .Build();
    }

    private ITrigger CreateTrigger(Guid mediaAssetId)
    {
        return TriggerBuilder.Create()
            .WithIdentity($"video-processing-{mediaAssetId}", JOB_GROUP)
            .StartNow()
            .Build();
    }
}