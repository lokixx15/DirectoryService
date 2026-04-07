using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain;

public sealed record FileName
{
    // ef core
    private FileName() { }

    public string Value { get; } = string.Empty;

    public string Name { get; } = string.Empty;

    public string Extension { get; } = string.Empty;

    private FileName(
        string name,
        string extension)
    {
        Name = name;
        Extension = extension;
        Value = name + "." + extension;
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