namespace FileService.VideoProcessing;

public sealed record VideoProcessingOptions
{
    public const string SECTION_NAME = "VideoProcessing";

    public string FfmpegPath { get; init; } = "\"C:\\ffmpeg\\bin\\ffmpeg.exe\"";

    public string FfprobePath { get; init; } = "\"C:\\ffmpeg\\bin\\ffprobe.exe\"";

    public bool UseHardwareAcceleration { get; init; }

    public string VideoEncoder { get; init; } = "libx264";

    public string VideoPreset { get; init; } = "medium";

    public int UploadDegreeOfParallelism { get; init; } = 10;
}