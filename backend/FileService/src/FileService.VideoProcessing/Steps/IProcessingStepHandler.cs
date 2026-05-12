using CSharpFunctionalExtensions;
using FileService.Domain.MediaProcessing;
using FileService.VideoProcessing.Pipeline;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.Steps;

public interface IProcessingStepHandler
{
    StepType StepType { get; }

    Task<Result<ProcessingContext, Error>> ExecuteAsync(ProcessingContext context, CancellationToken cancellationToken);
}