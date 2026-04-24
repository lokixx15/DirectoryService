using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain.MediaProcessing;

public sealed record Metadata
{
    // ef core
    private Metadata() { }

    public TimeSpan? Duration { get; }

    public int? Width { get; }

    public int? Height { get; }

    public string? Codec { get; }

    private Metadata(
        TimeSpan? duration,
        int? width,
        int? height,
        string? codec)
    {
        Duration = duration;
        Width = width;
        Height = height;
        Codec = codec;
    }

    public static Result<Metadata, Error> Create(
        TimeSpan? duration = null,
        int? width = null,
        int? height = null,
        string? codec = null)
    {
        if (duration < TimeSpan.Zero)
            return GeneralErrors.ValueIsNotValid("Duration cannot be negative", nameof(duration));

        if (width <= 0)
            return GeneralErrors.ValueIsNotValid("Width must be greater than 0", nameof(width));

        if (height <= 0)
            return GeneralErrors.ValueIsNotValid("Height must be greater than 0", nameof(height));

        return new Metadata(duration, width, height, codec);
    }
}