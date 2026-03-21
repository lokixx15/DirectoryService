using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain;

public sealed record ContentType
{
    // ef core
    private ContentType() { }

    public string Value { get; } = string.Empty;

    public MediaType Category { get; }

    private ContentType(
        string value,
        MediaType category)
    {
        Value = value;
        Category = category;
    }

    public static Result<ContentType, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsNullOrWhitespace(value);

        value = value.Trim();

        var category = value.ToLowerInvariant() switch
        {
            _ when value.Contains("video", StringComparison.InvariantCultureIgnoreCase) => MediaType.VIDEO,
            _ when value.Contains("image", StringComparison.InvariantCultureIgnoreCase) => MediaType.IMAGE,
            _ when value.Contains("audio", StringComparison.InvariantCultureIgnoreCase) => MediaType.AUDIO,
            _ => MediaType.UNKNOWN
        };

        return new ContentType(value, category);
    }
}