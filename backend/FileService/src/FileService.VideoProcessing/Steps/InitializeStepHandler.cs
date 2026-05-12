using CSharpFunctionalExtensions;
using FileService.Domain.MediaProcessing;
using FileService.VideoProcessing.Pipeline;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.Steps;

public class InitializeStepHandler : IProcessingStepHandler
{
    private readonly ILogger<InitializeStepHandler> _logger;

    public StepType StepType => StepType.INITIALIZE;

    public InitializeStepHandler(ILogger<InitializeStepHandler> logger)
    {
        _logger = logger;
    }

    public Task<Result<ProcessingContext, Error>> ExecuteAsync(
        ProcessingContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing processing for media ID: {MediaId}",
            context.VideoAsset.Id);

        var createDirecroryResult = context.CreateWorkingDirectory();
        if (createDirecroryResult.IsFailure)
        {
            _logger.LogError("Failed to create working directory for media ID: {MediaId}. Error: {Error}",
                context.VideoAsset.Id, createDirecroryResult.Error);

            return Task.FromResult(Result.Failure<ProcessingContext, Error>(createDirecroryResult.Error));
        }

        var setProgressResult = context.VideoProcess.ReportStepProgress(100);
        if (setProgressResult.IsFailure)
        {
            _logger.LogError("Failed to set progress for VideoAsset with id {VideoAssetId} in {Step}",
                context.VideoAsset.Id, StepType.ToString());
            return Task.FromResult(Result.Failure<ProcessingContext, Error>(setProgressResult.Error));
        }

        return Task.FromResult(Result.Success<ProcessingContext, Error>(context));
    }
}