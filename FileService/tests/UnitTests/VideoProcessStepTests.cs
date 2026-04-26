using FileService.Domain;
using FileService.Domain.MediaProcessing;

namespace UnitTests;

public class VideoProcessStepTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var process = CreateProcessWithStep();
        var step = process.Steps[0];

        Assert.Equal(VideoProcessStatus.PENDING, step.Status);
        Assert.Equal(0, step.Progress);
    }

    [Fact]
    public void Start_FromPending_ShouldSetRunning()
    {
        var process = CreateProcessWithStep();
        var step = process.Steps[0];

        process.PrepareForExecution();
        var result = process.StartStep(step.Order, step.StepType);

        Assert.True(result.IsSuccess);
        Assert.Equal(VideoProcessStatus.RUNNING, step.Status);
        Assert.NotNull(step.StartedAt);
    }

    [Fact]
    public void SetProgress_ClampsAndUpdatesProgress()
    {
        var process = CreateProcessWithStep();
        var step = process.Steps[0];

        process.PrepareForExecution();
        process.StartStep(step.Order, step.StepType);

        var result = process.ReportStepProgress(55.5);
        Assert.True(result.IsSuccess);
        Assert.Equal(55.5, step.Progress);
    }

    [Fact]
    public void Complete_FromRunning_ShouldSetSucceeded()
    {
        var process = CreateProcessWithStep();
        var step = process.Steps[0];

        process.PrepareForExecution();
        process.StartStep(step.Order, step.StepType);
        var result = process.CompleteStep(step.Order);

        Assert.True(result.IsSuccess);
        Assert.Equal(VideoProcessStatus.SUCCEEDED, step.Status);
        Assert.Equal(100, step.Progress);
        Assert.NotNull(step.CompletedAt);
    }

    [Fact]
    public void Fail_FromRunning_ShouldSetFailedAndError()
    {
        var process = CreateProcessWithStep();
        var step = process.Steps[0];

        process.PrepareForExecution();
        process.StartStep(step.Order, step.StepType);
        var errorMessage = "fail reason";
        var result = process.Fail(errorMessage);

        Assert.True(result.IsSuccess);
        Assert.Equal(VideoProcessStatus.FAILED, step.Status);
        Assert.Equal(errorMessage, step.ErrorMessage);
        Assert.NotNull(step.CompletedAt);
    }

    [Fact]
    public void Reset_ShouldReturnToPending()
    {
        var process = CreateProcessWithStep();
        var step = process.Steps[0];

        process.PrepareForExecution();
        process.StartStep(step.Order, step.StepType);
        process.CompleteStep(step.Order);
        process.PrepareForExecution();

        Assert.Equal(VideoProcessStatus.PENDING, step.Status);
        Assert.Null(step.ErrorMessage);
        Assert.Null(step.StartedAt);
        Assert.Null(step.CompletedAt);
    }

    [Fact]
    public void Start_FromSucceeded_ShouldFail()
    {
        var process = CreateProcessWithStep();
        var step = process.Steps[0];

        process.PrepareForExecution();
        process.StartStep(step.Order, step.StepType);
        process.CompleteStep(step.Order);
        var result = process.StartStep(step.Order, step.StepType);

        Assert.True(result.IsFailure);
        Assert.Equal(VideoProcessStatus.SUCCEEDED, step.Status);
    }

    private static VideoProcess CreateProcessWithStep(int order = 1, StepType stepType = StepType.INITIALIZE)
    {
        var step = VideoProcessStep.Create(Guid.NewGuid(), Guid.NewGuid(), order, stepType).Value;
        var process = VideoProcess.Create(
            Guid.NewGuid(),
            StorageKey.Create("bucket", null, "raw").Value,
            StorageKey.Create("bucket", null, "hls").Value,
            [step])
        .Value;
        return process;
    }
}