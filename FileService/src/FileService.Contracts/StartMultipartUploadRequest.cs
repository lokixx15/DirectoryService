namespace FileService.Core.Features;

public record StartMultipartUploadRequest(
    string FileName,
    string ContentType,
    long Size,
    string AssetType,
    string Context,
    Guid ContextId);
