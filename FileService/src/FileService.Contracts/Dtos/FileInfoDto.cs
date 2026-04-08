namespace FileService.Contracts.Dtos;

public record FileInfoDto(
    string FileName,
    string ContentType,
    long size);
