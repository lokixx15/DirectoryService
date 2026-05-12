using FileService.Contracts.Dtos;

namespace FileService.Contracts.Responses;

public record GetMediaAssetInfoResponse(
    MediaAssetDto MediaAsset,
    string? url);