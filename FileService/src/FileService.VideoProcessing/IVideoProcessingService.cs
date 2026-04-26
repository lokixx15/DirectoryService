using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing;

public interface IVideoProcessingService
{
    Task<UnitResult<Error>> ProcessVideoAsync(Guid videoAssetId, CancellationToken cancellationToken);
}