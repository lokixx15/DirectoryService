using FileService.Contracts;

namespace FileService.Core.Features;

public record StartMultipartUploadResponse(
    Guid MediaAssetId,
    string UploadId,
    IReadOnlyList<ChunkUploadUrl> ChunkUrls,
    long ChunkSize);
