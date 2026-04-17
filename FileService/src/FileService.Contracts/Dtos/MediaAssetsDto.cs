namespace FileService.Contracts.Dtos;

public record MediaAssetsDto(
    Guid Id,
    string Status,
    string? DownloadUrl);