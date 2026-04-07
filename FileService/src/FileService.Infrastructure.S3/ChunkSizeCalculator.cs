using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.FileStorage;
using Microsoft.Extensions.Options;
using SharedService.SharedKernel;

namespace FileService.Infrastructure.S3;

public class ChunkSizeCalculator : IChunkSizeCalculator
{
    private readonly S3Options _options;

    public ChunkSizeCalculator(IOptions<S3Options> options)
    {
        _options = options.Value;
    }

    public Result<(int ChunkSize, int TotalChunks), Error> Calculate(long fileSize)
    {
        if (_options.RecommendedChunkSizeBytes <= 0)
            return GeneralErrors.ValueIsNotValid("Recommended chunk size cannot be equal or less than 0 bytes", "Recommended chunk size");

        if (_options.MaxChunks <= 0)
            return GeneralErrors.ValueIsNotValid("Max chunks cannot be equal or less than 0", "Max chunks");

        if (fileSize <= 0)
            return GeneralErrors.ValueIsNotValid("File size cannot be equal or less than 0", "File size");

        if (fileSize <= _options.RecommendedChunkSizeBytes)
            return ((int)fileSize, 1);

        int totalChunks = Math.Min((int)Math.Ceiling((double)fileSize / _options.RecommendedChunkSizeBytes), _options.MaxChunks);

        long chunkSize = (fileSize + totalChunks - 1) / totalChunks;

        return ((int)chunkSize, totalChunks);
    }
}