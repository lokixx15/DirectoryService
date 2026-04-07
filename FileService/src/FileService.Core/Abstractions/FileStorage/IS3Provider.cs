using CSharpFunctionalExtensions;
using FileService.Contracts.Dtos;
using FileService.Core.Models;
using FileService.Domain;
using SharedService.SharedKernel;

namespace FileService.Core.Abstractions.FileStorage;

public interface IS3Provider
{
    Task<UnitResult<Error>> UploadFileAsync(
        StorageKey storageKey,
        Stream stream,
        MediaData mediaData,
        CancellationToken cancellationToken);

    Task<Result<string, Error>> DownloadFileAsync(
        StorageKey storageKey,
        string tempPath,
        CancellationToken cancellationToken);

    Task<Result<string, Error>> DeleteFileAsync(
        StorageKey storageKey,
        CancellationToken cancellationToken);

    Task<Result<string, Error>> GenerateUploadUrlAsync(
        StorageKey storageKey,
        MediaData mediaData,
        CancellationToken cancellationToken);

    Task<Result<string, Error>> GenerateDownloadUrlAsync(
        StorageKey storageKey);

    Task<Result<IReadOnlyList<MediaUrl>, Error>> GenerateDownloadUrlsAsync(
        IEnumerable<StorageKey> storageKeys,
        CancellationToken cancellationToken);

    Task<Result<string, Error>> StartMultipartUploadAsync(
        StorageKey storageKey,
        MediaData mediaData,
        CancellationToken cancellationToken);

    Task<Result<ChunkUploadUrlDto, Error>> GenerateChunkUploadUrlAsync(
        StorageKey storageKey,
        string uploadId,
        int partNumber);

    Task<Result<IReadOnlyList<ChunkUploadUrlDto>, Error>> GenerateAllChunkUploadUrlsAsync(
        StorageKey storageKey,
        string uploadId,
        int totalChunks,
        CancellationToken cancellationToken);

    Task<Result<CompleteMultipartUploadDto, Error>> CompleteMultipartUploadAsync(
        StorageKey storageKey,
        string uploadId,
        List<PartETagDto> partETags,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> AbortMultipartUploadAsync(
        StorageKey storageKey,
        string uploadId,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<MultipartUploadDto>, Error>> ListMultipartUploadAsync(
        string bucketName,
        CancellationToken cancellationToken);
}