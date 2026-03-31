namespace FileService.Core.Features;

public record AbortMultipartUploadRequest(
    Guid MediaAssetId,
    string UploadId);
