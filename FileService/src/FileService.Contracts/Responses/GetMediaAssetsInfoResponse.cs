using FileService.Contracts.Dtos;

namespace FileService.Contracts.Responses;

public record GetMediaAssetsInfoResponse(
    IReadOnlyList<MediaAssetsDto> MediaAssets);
