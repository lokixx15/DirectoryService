using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.Jobs;

public interface IVideoProcessingScheduler
{
    Task<UnitResult<Error>> ScheduleProcessingAsync(Guid videoAssetId, CancellationToken cancellationToken);
}