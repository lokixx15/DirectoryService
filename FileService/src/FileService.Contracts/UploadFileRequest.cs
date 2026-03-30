using Microsoft.AspNetCore.Http;

namespace FileService.Core.Features;

public record UploadFileRequest(
    IFormFile FormFile,
    string AssetType,
    Guid EntityId,
    string Context);
