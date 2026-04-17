using FileService.Contracts.Dtos;

namespace FileService.Contracts.Responses;

public record StartMultipartUploadResponse(
    Guid MediaAssetId,
    string UploadId,
    IReadOnlyList<ChunkUploadUrlDto> ChunkUrls,
    int ChunkSize);