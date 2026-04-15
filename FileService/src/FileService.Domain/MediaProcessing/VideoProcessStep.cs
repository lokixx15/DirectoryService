using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain.MediaProcessing;

public class VideoProcessStep
{
    // ef core
    private VideoProcessStep() { }

    public Guid Id { get; private set; }

    public Guid ProcessId { get; private set; }

    public int Order { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public VideoProcessStatus Status { get; private set; }

    public double Progress { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public string? ErrorMessage { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private VideoProcessStep(
        Guid id,
        Guid processId,
        int order,
        string name)
    {
        Id = id;
        ProcessId = processId;
        Order = order;
        Name = name;
        Status = VideoProcessStatus.PENDING;
        Progress = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public static Result<VideoProcessStep, Error> Create(
        Guid id,
        Guid processId,
        int order,
        string name)
    {
        if (order <= 0)
            return GeneralErrors.ValueIsNotValid("Step's order cannot be less than 0", "Order");

        if (string.IsNullOrWhiteSpace(name))
            return GeneralErrors.ValueIsNullOrWhitespace("Name");

        return new VideoProcessStep(id, processId, order, name);
    }

    internal UnitResult<Error> Start()
    {
        if (Status != VideoProcessStatus.PENDING)
            return Error.Validation(
                "video.process.status.is.invalid",
                "Invalid video process status to start video processing. Status Must be PENDING",
                nameof(VideoProcessStatus));

        Status = VideoProcessStatus.RUNNING;
        ErrorMessage = null;
        UpdatedAt = DateTime.UtcNow;
        StartedAt = UpdatedAt;

        return UnitResult.Success<Error>();
    }

    internal UnitResult<Error> SetProgress(double percent)
    {
        if (Status != VideoProcessStatus.RUNNING)
            return Error.Validation(
                "video.process.status.is.invalid",
                "Invalid video process status to set progress. Status Must be RUNNING",
                nameof(VideoProcessStatus));

        if (percent < 0 || percent > 100)
            return GeneralErrors.ValueIsNotValid("Video process step progress must be from 0 to 100", "Progress");

        Progress = percent;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    internal UnitResult<Error> Complete()
    {
        if (Status != VideoProcessStatus.RUNNING)
            return Error.Validation(
                "video.process.status.is.invalid",
                "Invalid video process status to complete video processing. Status Must be RUNNING",
                nameof(VideoProcessStatus));

        Status = VideoProcessStatus.SUCCEEDED;
        Progress = 100;
        UpdatedAt = DateTime.UtcNow;
        CompletedAt = UpdatedAt;

        return UnitResult.Success<Error>();
    }

    internal UnitResult<Error> Fail(string error)
    {
        if (Status != VideoProcessStatus.RUNNING)
            return Error.Validation(
                "video.process.status.is.invalid",
                "Invalid video process status to fail video processing. Status must be RUNNING",
                nameof(VideoProcessStatus));

        Status = VideoProcessStatus.FAILED;
        ErrorMessage = error;
        UpdatedAt = DateTime.UtcNow;
        CompletedAt = UpdatedAt;

        return UnitResult.Success<Error>();
    }

    internal void Reset()
    {
        Status = VideoProcessStatus.PENDING;
        UpdatedAt = DateTime.UtcNow;
        ErrorMessage = null;
        StartedAt = null;
        CompletedAt = null;
    }
}