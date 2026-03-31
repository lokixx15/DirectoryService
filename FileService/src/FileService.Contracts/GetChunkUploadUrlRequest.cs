namespace FileService.Core.Features;

public record GetChunkUploadUrlRequest(
    Guid MediaAssetId,
    string UploadId,
    int PartNumber);
