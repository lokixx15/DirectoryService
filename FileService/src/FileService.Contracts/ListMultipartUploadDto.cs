namespace FileService.Contracts;

public record MultipartUploadDto(
    string Key,
    string UploadId);