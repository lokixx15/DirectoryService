using CSharpFunctionalExtensions;
using FileService.Core;
using FileService.Core.Abstractions.Database;
using FileService.Domain.MediaProcessing;
using FileService.VideoProcessing.Steps;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.Pipeline;

public class ProcessingPipeline : IProcessingPipeline
{
    private readonly IVideoProcessRepository _videoProcessRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IEnumerable<IProcessingStepHandler> _stepHandlers;
    private readonly ILogger<ProcessingPipeline> _logger;

    public ProcessingPipeline(
        IVideoProcessRepository videoProcessRepository,
        IMediaRepository mediaRepository,
        ITransactionManager transactionManager,
        IEnumerable<IProcessingStepHandler> stepHandlers,
        ILogger<ProcessingPipeline> logger)
    {
        _videoProcessRepository = videoProcessRepository;
        _mediaRepository = mediaRepository;
        _transactionManager = transactionManager;
        _stepHandlers = stepHandlers;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> ProcessAllStepsAsync(
        Guid videoAssetId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting processing pipeline for video asset id: {VideoAssetId}", videoAssetId);

        var createContextResult = await CreateProcessingContext(videoAssetId, cancellationToken);
        if (createContextResult.IsFailure)
        {
            _logger.LogError("Failed to create processing context. Video asset id: {VideoAssetId}. Error: {Error}",
                videoAssetId, createContextResult.Error);

            return createContextResult.Error;
        }

        var context = createContextResult.Value;

        var executeAllStepsResult = await ExecuteAllSteps(context, cancellationToken);
        if (executeAllStepsResult.IsFailure)
        {
            _logger.LogError("Failed to execute all steps. Video asset id: {VideoAssetId}. Error: {Error}",
                videoAssetId, executeAllStepsResult.Error);

            return await FinalizeWithErrorAsync(context, executeAllStepsResult.Error, cancellationToken);
        }

        return await FinalizeAsync(context, cancellationToken);
    }

    private async Task<Result<ProcessingContext, Error>> CreateProcessingContext(
        Guid videoAssetId,
        CancellationToken cancellationToken)
    {
        var mediaAssetResult = await _mediaRepository.GetVideoByAsync(
            m => m.Id == videoAssetId,
            cancellationToken);
        if (mediaAssetResult.IsFailure)
        {
            _logger.LogError("Failed to obtain media asset from the database. Video asset id: {VideoAssetId}. Error: {Error}",
                videoAssetId, mediaAssetResult.Error);
            return mediaAssetResult.Error;
        }

        var videoProcessResult = await _videoProcessRepository.GetByAsync(
            vp => vp.Id == videoAssetId,
            cancellationToken);

        VideoProcess videoProcess;

        if (videoProcessResult.IsFailure)
        {
            var initializeStepsResult = VideoProcess.InitializeSteps(videoAssetId);
            if (initializeStepsResult.IsFailure)
            {
                _logger.LogError("Failed to initialize video process steps for video process with id {Id}",
                    videoAssetId);

                return initializeStepsResult.Error;
            }

            var newVideoProcess = VideoProcess.Create(
                videoAssetId,
                mediaAssetResult.Value.RawKey,
                mediaAssetResult.Value.HslRootKey,
                initializeStepsResult.Value);
            if (newVideoProcess.IsFailure)
            {
                _logger.LogError("Failed to create video process. Video asset id: {VideoAssetId}. Error: {Error}",
                    videoAssetId, newVideoProcess.Error);
                return newVideoProcess.Error;
            }

            videoProcess = newVideoProcess.Value;

            var addResult = await _videoProcessRepository.AddAsync(videoProcess, cancellationToken);
            if (addResult.IsFailure)
            {
                _logger.LogError("Failed to add video process to the database. Video asset id: {VideoAssetId}. Error: {Error}",
                    videoAssetId, addResult.Error);
                return addResult.Error;
            }
        }
        else
        {
            _logger.LogInformation("Video process with id {VideoProcessId} was obtained from the database", videoAssetId);

            videoProcess = videoProcessResult.Value;
            var setHlsResult = videoProcess.SetHlsKey(mediaAssetResult.Value.HslRootKey);
            if (setHlsResult.IsFailure)
            {
                _logger.LogError("Failed to set hls key for video process with id {VideoProcessId}", videoAssetId);
                return setHlsResult.Error;
            }
        }

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Failed to save changes to the database. Video asset id: {VideoAssetId}. Error: {Error}",
                videoAssetId, saveChangesResult.Error);
            return saveChangesResult.Error;
        }

        var context = new ProcessingContext
        {
            VideoProcess = videoProcess,
            VideoAsset = mediaAssetResult.Value
        };

        return context;
    }

    private async Task<UnitResult<Error>> ExecuteAllSteps(
        ProcessingContext context,
        CancellationToken cancellationToken)
    {
        var videoProcess = context.VideoProcess;

        var prepareForExecutionResult = videoProcess.PrepareForExecution();
        if (prepareForExecutionResult.IsFailure)
        {
            _logger.LogError("Failed to prepare video process for execution. Video asset id: {VideoAssetId}. Error: {Error}",
                videoProcess.Id, prepareForExecutionResult.Error);
            return prepareForExecutionResult.Error;
        }

        foreach (var step in context.VideoProcess.Steps.OrderBy(s => s.Order))
        {
            var startStepResult = context.VideoProcess.StartStep(step.Order, step.StepType);
            if (startStepResult.IsFailure)
            {
                _logger.LogError("Failed to start step with order {StepOrder} and type {StepType}. Video asset id: {VideoAssetId}. Error: {Error}",
                    step.Order, step.StepType, context.VideoAsset.Id, startStepResult.Error);
                return startStepResult.Error;
            }

            var saveAfterStartResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveAfterStartResult.IsFailure)
            {
                _logger.LogError("Failed to save changes after starting step with order {StepOrder} and type {StepType}. Video asset id: {VideoAssetId}. Error: {Error}",
                    step.Order, step.StepType, context.VideoAsset.Id, saveAfterStartResult.Error);
                return saveAfterStartResult.Error;
            }

            var stepHandler = _stepHandlers.FirstOrDefault(sH => sH.StepType == step.StepType);
            if (stepHandler is null)
            {
                _logger.LogError("No step handler was found for step type {StepType}. Video asset id: {VideoAssetId}",
                    step.StepType, context.VideoAsset.Id);
                return Error.Failure("processing.pipeline.error", $"No step handler was found for step type {step.StepType}.");
            }

            var executeStepResult = await ExecuteStepAsync(stepHandler, context, cancellationToken);
            if (executeStepResult.IsFailure)
            {
                _logger.LogError("Failed to execute step with order {StepOrder} and type {StepType}. Video asset id: {VideoAssetId}. Error: {Error}",
                    step.Order, step.StepType, context.VideoAsset.Id, executeStepResult.Error);
                return executeStepResult.Error;
            }

            var saveAfterExecuteStepResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveAfterExecuteStepResult.IsFailure)
            {
                _logger.LogError("Failed to save changes after executing step with order {StepOrder} and type {StepType}. Video asset id: {VideoAssetId}. Error: {Error}",
                    step.Order, step.StepType, context.VideoAsset.Id, saveAfterExecuteStepResult.Error);
                return saveAfterExecuteStepResult.Error;
            }

            var completeStepResult = context.VideoProcess.CompleteStep(step.Order);
            if (completeStepResult.IsFailure)
            {
                _logger.LogError("Failed to complete step with order {StepOrder} and type {StepType}. Video asset id: {VideoAssetId}. Error: {Error}",
                    step.Order, step.StepType, context.VideoAsset.Id, completeStepResult.Error);
                return completeStepResult.Error;
            }

            var saveAfterCompleteResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveAfterCompleteResult.IsFailure)
            {
                _logger.LogError("Failed to save changes after completing step with order {StepOrder} and type {StepType}. Video asset id: {VideoAssetId}. Error: {Error}",
                    step.Order, step.StepType, context.VideoAsset.Id, saveAfterCompleteResult.Error);
                return saveAfterCompleteResult.Error;
            }
        }

        return UnitResult.Success<Error>();
    }

    private async Task<Result<ProcessingContext, Error>> ExecuteStepAsync(
        IProcessingStepHandler stepHandler,
        ProcessingContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            return await stepHandler.ExecuteAsync(context, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while executing step {StepType}. Video asset id: {VideoAssetId}",
                stepHandler.StepType, context.VideoAsset.Id);
            return Error.Failure("processing.pipeline.error", "An exception occurred while executing the step.");
        }
    }

    private async Task<UnitResult<Error>> FinalizeWithErrorAsync(
        ProcessingContext context,
        Error error,
        CancellationToken cancellationToken)
    {
        context.VideoProcess.Fail(error.Message);
        context.VideoAsset.MarkFailed(DateTime.UtcNow);

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Failed to save changes after finalize video processing with error");
            return saveChangesResult.Error;
        }

        return UnitResult.Success<Error>();
    }

    private async Task<UnitResult<Error>> FinalizeAsync(
        ProcessingContext context,
        CancellationToken cancellationToken)
    {
        context.VideoProcess.FinishProcessing();
        context.VideoAsset.CompleteProcessing(DateTime.UtcNow);

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Failed to save changes after finalize video processing");
            return saveChangesResult.Error;
        }

        return UnitResult.Success<Error>();
    }
}