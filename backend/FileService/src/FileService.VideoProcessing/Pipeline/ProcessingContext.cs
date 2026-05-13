using CSharpFunctionalExtensions;
using FileService.Domain.Assets;
using FileService.Domain.MediaProcessing;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.Pipeline;

public sealed record ProcessingContext
{
    private const string HLS_SUBDIRECTORY = "hls";

    public required VideoProcess VideoProcess { get; init; }

    public required VideoAsset VideoAsset { get; init; }

    public string? WorkingDirectory { get; private set; }

    public string? HlsOutputDirectory { get; private set; }

    public string? MediaAssetUrl { get; private set; }

    public UnitResult<Error> CreateWorkingDirectory()
    {
        try
        {
            WorkingDirectory = Directory.CreateTempSubdirectory("video-processing").FullName;

            HlsOutputDirectory = Path.Combine(WorkingDirectory, HLS_SUBDIRECTORY);
            Directory.CreateDirectory(HlsOutputDirectory);
        }
        catch (Exception)
        {
            return Error.Failure("working.directory.creation", "Failed to create working directory");
        }

        return UnitResult.Success<Error>();
    }

    public void SetMediaAssetUrl(string url)
    {
        MediaAssetUrl = url;
    }

    public void CleanUp()
    {
        WorkingDirectory = null;
        HlsOutputDirectory = null;
        MediaAssetUrl = null;
    }
}