using FileService.Contracts.Dtos;

namespace FileService.Contracts.Requests;

public record CompleteMultipartUploadRequest(
    Guid MediaAssetId,
    string UploadId,
    List<PartETagDto> PartETags);