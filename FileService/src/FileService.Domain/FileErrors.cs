using SharedService.SharedKernel;

namespace FileService.Domain;

public static class FileErrors
{
    public static Error NotFound() => Error.NotFound("element.not.found", $"Bucket or file was not found");

    public static Error Forbidden() => Error.Failure("forbidden", "Access is denied");

    public static Error Conflict() => Error.Conflict("invalid.object.state", "File is an invalid state");

    public static Error InternalError() => Error.Failure("internal.server.error", "Internal server error has occured");

    public static Error ProcessFailed() => Error.Failure("process.failed", "File processing failed");

    public static Error InvalidFfprobeOutput(string details) => Error.Failure("invalid.ffprobe.output", $"Invalid ffprobe output: {details}");

    public static Error HlsProcessingFailed() => Error.Failure("hls.processing.failed", "Failed to process HLS for the video asset");

    public static Error HlsProcessingFailed(string details) => Error.Failure("hls.processing.failed", $"Failed to process HLS for the video asset: {details}");
}