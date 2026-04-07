namespace FileService.Contracts.Dtos;

public record MediaAssetDto(
    Guid Id,
    string Status,
    string AssetType,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    FileInfoDto FileInfo);
