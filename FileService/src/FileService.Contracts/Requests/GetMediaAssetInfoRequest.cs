namespace FileService.Contracts.Requests;

public record GetMediaAssetsInfoRequest(
    IReadOnlyList<Guid> MediaAssetIds);
