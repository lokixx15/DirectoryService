using CSharpFunctionalExtensions;
using FileService.Domain.MediaProcessing;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.FfmpegProcess;

public interface IFfmpegProcessRunner
{
    Task<Result<Metadata, Error>> ExtractMetadataAsync(
        string inputFileUrl,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> GenerateHlsAsync(
        string inputFileUrl,
        string outputDirectory,
        CancellationToken cancellationToken);
}