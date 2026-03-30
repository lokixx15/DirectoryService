using Microsoft.AspNetCore.Http;

namespace FileService.Contracts;

public record UploadFileRequest(
    IFormFile FormFile,
    string AssetType,
    Guid EntityId,
    string Context);
