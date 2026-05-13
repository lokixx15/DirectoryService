using FileService.Domain;
using FileService.Domain.MediaProcessing;

namespace UnitTests;

public class VideoProcessTests
{
    [Fact]
    public void Create_WithCorrectData_ShouldSuccess()
    {
        var steps = new[] { CreateStep(1, StepType.INITIALIZE), CreateStep(2, StepType.EXTRACT_METADATA) };
        var result = VideoProcess.Create(Guid.NewGuid(), CreateKey(), CreateKey(), steps);

        Assert.True(result.IsSuccess);
        Assert.Equal(VideoProcessStatus.PENDING, result.Value.Status);
        Assert.Equal(2, result.Value.Steps.Count);
    }

    [Fact]
    public void Create_WithEmptySteps_ShouldFail()
    {
        var result = VideoProcess.Create(Guid.NewGuid(), CreateKey(), CreateKey(), []);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_WithDuplicateOrder_ShouldFail()
    {
        var steps = new[] { CreateStep(1, StepType.INITIALIZE), CreateStep(1, StepType.EXTRACT_METADATA) };
        var result = VideoProcess.Create(Guid.NewGuid(), CreateKey(), CreateKey(), steps);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void HappyPath_PrepareStartCompleteFinish_AllSteps()
    {
        var steps = new[] { CreateStep(1, StepType.INITIALIZE), CreateStep(2, StepType.EXTRACT_METADATA) };
        var process = VideoProcess.Create(Guid.NewGuid(), CreateKey(), CreateKey(), steps).Value;

        process.PrepareForExecution();
        process.StartStep(1, StepType.INITIALIZE);
        process.ReportStepProgress(50);
        process.CompleteStep(1);
        process.StartStep(2, StepType.EXTRACT_METADATA);
        process.ReportStepProgress(100);
        process.CompleteStep(2);
        var finishResult = process.FinishProcessing();

        Assert.True(finishResult.IsSuccess);
        Assert.Equal(VideoProcessStatus.SUCCEEDED, process.Status);
        Assert.Equal(100, process.TotalProgress);
    }

    [Fact]
    public void PrepareForExecution_AfterFailed_AllowsRestart()
    {
        var steps = new[] { CreateStep(1, StepType.INITIALIZE) };
        var process = VideoProcess.Create(Guid.NewGuid(), CreateKey(), CreateKey(), steps).Value;

        process.PrepareForExecution();
        process.StartStep(1, StepType.INITIALIZE);
        process.Fail("fail");
        process.PrepareForExecution();

        Assert.Equal(VideoProcessStatus.PENDING, process.Status);
        Assert.All(process.Steps, s => Assert.Equal(VideoProcessStatus.PENDING, s.Status));
    }

    [Fact]
    public void PrepareForExecution_WhenCanceled_ShouldFail()
    {
        var steps = new[] { CreateStep(1, StepType.INITIALIZE) };
        var process = VideoProcess.Create(Guid.NewGuid(), CreateKey(), CreateKey(), steps).Value;

        process.PrepareForExecution();
        process.StartStep(1, StepType.INITIALIZE);
        process.Cancel("cancel");
        var result = process.PrepareForExecution();

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Fail_ShouldSetFailedStatusAndError()
    {
        var steps = new[] { CreateStep(1, StepType.INITIALIZE) };
        var process = VideoProcess.Create(Guid.NewGuid(), CreateKey(), CreateKey(), steps).Value;

        process.PrepareForExecution();
        process.StartStep(1, StepType.INITIALIZE);
        var result = process.Fail("fail reason");

        Assert.True(result.IsSuccess);
        Assert.Equal(VideoProcessStatus.FAILED, process.Status);
        Assert.Equal("fail reason", process.ErrorMessage);
    }

    [Fact]
    public void TotalProgress_ShouldUpdateOnStepCompletion()
    {
        var steps = new[] { CreateStep(1, StepType.INITIALIZE), CreateStep(2, StepType.EXTRACT_METADATA) };
        var process = VideoProcess.Create(Guid.NewGuid(), CreateKey(), CreateKey(), steps).Value;

        process.PrepareForExecution();

        process.StartStep(1, StepType.INITIALIZE);
        process.CompleteStep(1);
        Assert.Equal(50, process.TotalProgress);

        process.StartStep(2, StepType.EXTRACT_METADATA);
        process.CompleteStep(2);
        Assert.Equal(100, process.TotalProgress);
    }

    private static VideoProcessStep CreateStep(int order, StepType stepType)
    {
        return VideoProcessStep.Create(Guid.NewGuid(), Guid.NewGuid(), order, stepType).Value;
    }

    private static StorageKey CreateKey(string bucket = "bucket")
    {
        return StorageKey.Create(bucket, null, Guid.NewGuid().ToString()).Value;
    }
}