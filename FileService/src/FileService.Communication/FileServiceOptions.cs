namespace FileService.Communication;

public record FileServiceOptions
{
    public string Url { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; } = 5;
}