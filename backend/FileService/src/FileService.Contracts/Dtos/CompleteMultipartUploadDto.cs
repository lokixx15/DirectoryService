namespace FileService.Contracts.Dtos;

public record CompleteMultipartUploadDto(
    string Location,
    string BucketName,
    string Key);