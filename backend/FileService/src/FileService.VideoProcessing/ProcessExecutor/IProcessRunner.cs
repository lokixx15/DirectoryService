using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.ProcessExecutor;

public interface IProcessRunner
{
    Task<Result<ProcessResult, Error>> RunAsync(
        ProcessCommand command,
        Action<string>? onOutput = null,
        CancellationToken cancellationToken = default);
}
