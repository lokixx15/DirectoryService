namespace FileService.Contracts.Dtos;

public record ChunkUploadUrlDto(
    int PartNumber,
    string UploadUrl);