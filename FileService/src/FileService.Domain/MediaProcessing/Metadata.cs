using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain.MediaProcessing;

public sealed record Metadata
{
    // ef core
    private Metadata() { }

    public MediaData MediaData { get; } = null!;

    public TimeSpan? Duration { get; }

    public int? Width { get; }

    public int? Height { get; }

    public string? Codec { get; }

    private Metadata(
        MediaData mediaData,
        TimeSpan? duration,
        int? width,
        int? height,
        string? codec)
    {
        MediaData = mediaData;
        Duration = duration;
        Width = width;
        Height = height;
        Codec = codec;
    }

    public static Result<Metadata, Error> Create(
        MediaData mediaData,
        TimeSpan? duration = null,
        int? width = null,
        int? height = null,
        string? codec = null)
    {
        if (mediaData is null)
            return GeneralErrors.ValueIsNotValid("Media data cannot be null", nameof(mediaData));

        if (duration < TimeSpan.Zero)
            return GeneralErrors.ValueIsNotValid("Duration cannot be negative", nameof(duration));

        if (width <= 0)
            return GeneralErrors.ValueIsNotValid("Width must be greater than 0", nameof(width));

        if (height <= 0)
            return GeneralErrors.ValueIsNotValid("Height must be greater than 0", nameof(height));

        return new Metadata(mediaData, duration, width, height, codec);
    }
}