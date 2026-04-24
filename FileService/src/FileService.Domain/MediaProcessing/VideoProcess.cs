using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain.MediaProcessing;

public class VideoProcess
{
    // ef core
    private VideoProcess() { }

    private List<VideoProcessStep> _steps = [];

    public Guid Id { get; private set; }

    public StorageKey RawKey { get; private set; } = null!;

    public StorageKey HlsKey { get; private set; } = null!;

    public VideoProcessStatus Status { get; private set; }

    public int? CurrentStepOrder { get; private set; }

    public StepType? CurrentStepType { get; private set; }

    public double CurrentStepProgress { get; private set; }

    public double TotalProgress { get; private set; }

    public Metadata Metadata { get; private set; } = null!;

    public string? ErrorMessage { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<VideoProcessStep> Steps => _steps;

    public bool IsCompleted =>
        Status == VideoProcessStatus.SUCCEEDED
        || Status == VideoProcessStatus.FAILED
        || Status == VideoProcessStatus.CANCELED;

    private VideoProcess(
        Guid id,
        StorageKey rawKey,
        StorageKey hlsKey,
        IEnumerable<VideoProcessStep> steps)
    {
        Id = id;
        RawKey = rawKey;
        HlsKey = hlsKey;
        _steps = steps.ToList();
        Status = VideoProcessStatus.PENDING;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public static Result<VideoProcess, Error> Create(
        Guid id,
        StorageKey rawKey,
        StorageKey hlsKey,
        IEnumerable<VideoProcessStep> steps)
    {
        var stepsList = steps.ToList();

        if (!stepsList.Any())
            return GeneralErrors.CollectionIsNullOrEmpty("Video process steps");

        if (stepsList.Count != stepsList.Select(s => s.Order).Distinct().Count())
            return GeneralErrors.CollectionContainsDuplicates("Order");

        if (stepsList.Any(s => s.Order <= 0))
            return GeneralErrors.ValueIsNotValid("Step's order cannot be less than 0", "Order");

        return new VideoProcess(id, rawKey, hlsKey, stepsList);
    }

    public UnitResult<Error> PrepareForExecution()
    {
        if (Status == VideoProcessStatus.CANCELED)
            return Error.Validation(
                    "video.processing.canceled",
                    $"Video processing is canceled - cannot prepare for execution",
                    nameof(VideoProcessStatus));

        if (Status == VideoProcessStatus.FAILED || Status == VideoProcessStatus.RUNNING)
        {
            Status = VideoProcessStatus.PENDING;
            CurrentStepOrder = null;
            CurrentStepType = null;
            CurrentStepProgress = 0;
            ErrorMessage = null;

            foreach (var step in _steps)
                step.Reset();
        }

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> StartStep(int order, StepType stepType)
    {
        var step = _steps.FirstOrDefault(s => s.Order == order && s.StepType == stepType);
        if (step is null)
            return Error.NotFound(
                "value.does.not.exist",
                $"Process step with order {order} and stepType {stepType} doesn't exist in steps");

        var startStepResult = step.Start();
        if (startStepResult.IsFailure)
            return startStepResult.Error;

        CurrentStepOrder = order;
        CurrentStepType = stepType;
        Status = VideoProcessStatus.RUNNING;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> CompleteStep(int order)
    {
        if (Status != VideoProcessStatus.RUNNING)
            return Error.Validation(
                "video.process.status.is.invalid",
                "Invalid video process status to complete video processing. Status Must be RUNNING",
                nameof(VideoProcessStatus));

        if (CurrentStepOrder != order)
            return GeneralErrors.ValueIsNotValid("Order must match with current step's order");

        var step = _steps.FirstOrDefault(s => s.Order == order);
        if (step is null)
            return Error.NotFound("value.does.not.exist",
                $"Process step with order {order} doesn't exist in steps");

        var completeStepResult = step.Complete();
        if (completeStepResult.IsFailure)
            return completeStepResult.Error;

        UpdatedAt = DateTime.UtcNow;
        CalculateTotalProgress();

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> ReportStepProgress(double percent)
    {
        if (Status != VideoProcessStatus.RUNNING)
            return Error.Validation(
                "video.process.status.is.invalid",
                "Invalid video process status to report step progress. Status Must be RUNNING",
                nameof(VideoProcessStatus));

        var step = _steps.FirstOrDefault(s => s.Order == CurrentStepOrder);
        if (step is null)
            return Error.NotFound(
                "value.does.not.exist",
                $"Process step with order {CurrentStepOrder} doesn't exist in steps");

        var setProgressResult = step.SetProgress(percent);
        if (setProgressResult.IsFailure)
            return setProgressResult.Error;

        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    public void SetMetadata(Metadata metadata)
    {
        Metadata = metadata;
        UpdatedAt = DateTime.UtcNow;
    }

    public UnitResult<Error> FinishProcessing()
    {
        if (!_steps.All(s => s.Status == VideoProcessStatus.SUCCEEDED))
            return Error.Validation(
                "video.process.not.completed",
                "Cannot finish processing because not all steps succeeded",
                nameof(VideoProcessStatus));

        UpdatedAt = DateTime.UtcNow;
        Status = VideoProcessStatus.SUCCEEDED;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Fail(string message)
    {
        if (Status != VideoProcessStatus.RUNNING)
            return Error.Validation(
                "video.process.status.is.invalid",
                "Invalid video process status to fail video processing. Status Must be RUNNING",
                nameof(VideoProcessStatus));

        var step = _steps.FirstOrDefault(s => s.Order == CurrentStepOrder);
        if (step is null)
            return Error.NotFound("value.does.not.exist",
                $"Process step with order {CurrentStepOrder} doesn't exist in steps");

        var failStepResult = step.Fail(message);
        if (failStepResult.IsFailure)
            return failStepResult.Error;

        UpdatedAt = DateTime.UtcNow;
        Status = VideoProcessStatus.FAILED;
        ErrorMessage = message;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Cancel(string message)
    {
        if (Status != VideoProcessStatus.RUNNING)
            return Error.Validation(
                "video.process.status.is.invalid",
                "Invalid video process status to cancel video processing. Status Must be RUNNING",
                nameof(VideoProcessStatus));

        UpdatedAt = DateTime.UtcNow;
        Status = VideoProcessStatus.CANCELED;

        return UnitResult.Success<Error>();
    }

    private void CalculateTotalProgress() =>
        TotalProgress = _steps.Count == 0
            ? 0
            : (double)_steps.Count(s => s.Status == VideoProcessStatus.SUCCEEDED) / _steps.Count * 100;
}