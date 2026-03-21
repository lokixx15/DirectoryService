using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Domain;

public sealed record MediaData
{
    public FileName FileName { get; }

    public ContentType ContentType { get; }

    public long Size { get; }

    public int ExpectedChuncksCount { get; }

    private MediaData(
        FileName fileName,
        ContentType contentType,
        long size,
        int expectedChuncksCount)
    {
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        ExpectedChuncksCount = expectedChuncksCount;
    }

    public static Result<MediaData, Error> Create(
        FileName fileName,
        ContentType contentType,
        long size,
        int expectedChuncksCount)
    {
        if (size <= 0)
            return GeneralErrors.ValueIsNotValid("Size cannot be less than 0", nameof(size));

        if (expectedChuncksCount <= 0)
            return GeneralErrors.ValueIsNotValid("Expected chuncks count cannot be less than 0", nameof(size));

        return new MediaData(fileName, contentType, size, expectedChuncksCount);
    }
}