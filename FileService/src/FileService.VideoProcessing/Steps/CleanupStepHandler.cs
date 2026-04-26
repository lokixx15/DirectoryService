using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.FileStorage;
using FileService.Domain.MediaProcessing;
using FileService.VideoProcessing.Pipeline;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.Steps;

public sealed class CleanupStepHandler : IProcessingStepHandler
{
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<CleanupStepHandler> _logger;

    public CleanupStepHandler(
        IS3Provider s3Provider,
        ILogger<CleanupStepHandler> logger)
    {
        _s3Provider = s3Provider;
        _logger = logger;
    }

    public StepType StepType => StepType.CLEANUP;

    public async Task<Result<ProcessingContext, Error>> ExecuteAsync(
        ProcessingContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting upload hls to S3 for VideoAsset {VideoAssetId}",
            context.VideoAsset.Id);

        if (string.IsNullOrWhiteSpace(context.WorkingDirectory))
        {
            _logger.LogWarning("Working directory is not set");
            return await Task.FromResult(context);
        }

        var deleteResult = await _s3Provider.DeleteFileAsync(context.VideoAsset.RawKey, cancellationToken);
        if (deleteResult.IsFailure)
        {
            _logger.LogWarning("Failed to DELETE raw file from storage for VideoAsset with id {VideoAssetId}. Error: {Error}",
                context.VideoAsset.Id, deleteResult.Error);
        }
        else
        {
            _logger.LogDebug("Raw file deleted from storage for VideoAsset with id {VideoAssetId}",
                context.VideoAsset.Id);
        }

        try
        {
            if (Directory.Exists(context.WorkingDirectory))
            {
                Directory.Delete(context.WorkingDirectory, true);
                _logger.LogDebug("Working directory deleted {WorkingDirectory}", context.WorkingDirectory);

                context.CleanUp();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete working directory {WorkingDirectory}", context.WorkingDirectory);
        }

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