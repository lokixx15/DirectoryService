using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain;

public sealed record FileName
{
    public string Value { get; }

    public string Extension { get; }

    private FileName(
        string value,
        string extension)
    {
        Value = value;
        Extension = extension;
    }

    public static Result<FileName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsNullOrWhitespace(value);

        value = value.Trim();

        var extension = Path.GetExtension(value);

        if (string.IsNullOrEmpty(extension))
            return GeneralErrors.ValueIsNotValid("File must have valid extension", value);

        extension = extension.Substring(1).ToLowerInvariant();

        return new FileName(value, extension);
    }
}