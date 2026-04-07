using Microsoft.AspNetCore.Http;

namespace FileService.Contracts.Requests;

public record UploadFileRequest(
    IFormFile FormFile,
    string AssetType,
    Guid EntityId,
    string Context);
