using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.FileStorage;
using SharedService.SharedKernel;

namespace FileService.Infrastructure.S3;

public class ChunkSizeCalculator : IChunkSizeCalculator
{
    private readonly S3Options _options;

    public ChunkSizeCalculator(S3Options options)
    {
        _options = options;
    }

    public Result<(long ChunkSize, int TotalChunks), Error> Calculate(long fileSize)
    {
        if (fileSize == 0)
            return GeneralErrors.ValueIsNotValid("File size cannot be zero", "File size");

        int recommendedChunkSize = _options.RecommendedChunkSizeBytes;
        int maxChunks = _options.MaxChuncks;

        if (fileSize <= recommendedChunkSize)
            return (fileSize, 1);

        int totalChunks = (int)Math.Ceiling((double)(fileSize + recommendedChunkSize + 1) / recommendedChunkSize);

        long chunkSize;

        if (totalChunks > maxChunks)
        {
            totalChunks = maxChunks;
            chunkSize = (long)Math.Ceiling((double)fileSize / totalChunks);
        }
        else
        {
            chunkSize = recommendedChunkSize;
        }

        return (chunkSize, totalChunks);
    }
}