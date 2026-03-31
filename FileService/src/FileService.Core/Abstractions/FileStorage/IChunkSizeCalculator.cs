using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Core.Abstractions.FileStorage;

public interface IChunkSizeCalculator
{
    Result<(long ChunkSize, int TotalChunks), Error> Calculate(long fileSize);
}