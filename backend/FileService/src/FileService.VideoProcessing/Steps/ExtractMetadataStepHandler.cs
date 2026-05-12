using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.FileStorage;
using FileService.Domain.MediaProcessing;
using FileService.VideoProcessing.FfmpegProcess;
using FileService.VideoProcessing.Pipeline;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.Steps;

public class ExtractMetadataStepHandler : IProcessingStepHandler
{
    private readonly IFfmpegProcessRunner _ffmpegProcessRunner;
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<ExtractMetadataStepHandler> _logger;

    public ExtractMetadataStepHandler(
        IFfmpegProcessRunner ffmpegProcessRunner,
        IS3Provider s3Provider,
        ILogger<ExtractMetadataStepHandler> logger)
    {
        _ffmpegProcessRunner = ffmpegProcessRunner;
        _s3Provider = s3Provider;
        _logger = logger;
    }

    public StepType StepType => StepType.EXTRACT_METADATA;

    public async Task<Result<ProcessingContext, Error>> ExecuteAsync(
        ProcessingContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Extracting metadata for video asset with id {VideoAssetId}",
            context.VideoAsset.Id);

        var generateMediaAssetUrlResult = await _s3Provider
            .GenerateDownloadUrlAsync(context.VideoAsset.RawKey);
        if (generateMediaAssetUrlResult.IsFailure)
            return generateMediaAssetUrlResult.Error;

        context.SetMediaAssetUrl(generateMediaAssetUrlResult.Value);

        var extractMetadataResult = await _ffmpegProcessRunner.ExtractMetadataAsync(
            generateMediaAssetUrlResult.Value,
            cancellationToken);
        if (extractMetadataResult.IsFailure)
            return extractMetadataResult.Error;

        context.VideoProcess.SetMetadata(extractMetadataResult.Value);

        var setProgressResult = context.VideoProcess.ReportStepProgress(100);
        if (setProgressResult.IsFailure)
        {
            _logger.LogError("Failed to set progress for VideoAsset with id {VideoAssetId} in {Step}",
                context.VideoAsset.Id, StepType.ToString());
            return setProgressResult.Error;
        }

        return context;
    }
}