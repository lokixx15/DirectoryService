namespace FileService.VideoProcessing.ProcessExecutor;

public record ProcessResult(
    int ExitCode,
    string StandardOutput,
    string StandardError);