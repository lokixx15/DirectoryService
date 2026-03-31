using FileService.Contracts;

namespace FileService.Core.Features;

public record CompleteMultipartUploadRequest(
    Guid MediaAssetId,
    string UploadId,
    List<PartETagDto> PartETags);
