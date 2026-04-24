using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.FileStorage;
using FileService.Domain;
using FileService.Domain.MediaProcessing;
using FileService.VideoProcessing.FfmpegProcess;
using FileService.VideoProcessing.Pipeline;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.Steps;

public class GenerateHlsStepHandler : IProcessingStepHandler
{
    private readonly IFfmpegProcessRunner _ffmpegProcessRunner;
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<GenerateHlsStepHandler> _logger;

    public GenerateHlsStepHandler(
        IFfmpegProcessRunner ffmpegProcessRunner,
        IS3Provider s3Provider,
        ILogger<GenerateHlsStepHandler> logger)
    {
        _ffmpegProcessRunner = ffmpegProcessRunner;
        _s3Provider = s3Provider;
        _logger = logger;
    }

    public StepType StepType => StepType.GENERATE_HLS;

    public async Task<Result<ProcessingContext, Error>> ExecuteAsync(
        ProcessingContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating HLS for video asset with id {VideoAssetId}",
            context.VideoAsset.Id);

        string inputFileUrl;
        if (!string.IsNullOrEmpty(context.MediaAssetUrl))
        {
            inputFileUrl = context.MediaAssetUrl;
        }
        else
        {
            var generateMediaAssetUrlResult = await _s3Provider
                .GenerateDownloadUrlAsync(context.VideoAsset.RawKey);
            if (generateMediaAssetUrlResult.IsFailure)
                return generateMediaAssetUrlResult.Error;

            inputFileUrl = generateMediaAssetUrlResult.Value;
        }

        if (context.HlsOutputDirectory == null)
            return FileErrors.HlsProcessingFailed();

        if (context.VideoProcess.Metadata == null)
            _logger.LogWarning("Process tracking will be unavailable for video asset with id {VideoAssetId} because metadata is missing",
                context.VideoAsset.Id);

        var generateHlsResult = await _ffmpegProcessRunner.GenerateHlsAsync(
            inputFileUrl,
            context.HlsOutputDirectory,
            cancellationToken);
        if (generateHlsResult.IsFailure)
            return generateHlsResult.Error;

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