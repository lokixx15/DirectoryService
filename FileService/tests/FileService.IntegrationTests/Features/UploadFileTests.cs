using System.Net.Http.Headers;
using CSharpFunctionalExtensions;
using FileService.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Http;
using SharedService.SharedKernel;

namespace FileService.IntegrationTests.Features;

public class UploadFileTests : FileServiceTestsBase
{
    public UploadFileTests(IntegrationTestsWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task UploadVideoFile_Should_Success()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var uploadFileResult = await UploadFileAsync(
            "test-file.mp4",
            "video/mp4",
            "video",
            "lesson",
            cancellationToken);

        Assert.True(uploadFileResult.IsSuccess);
    }

    [Fact]
    public async Task UploadImageFile_Should_Success()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var uploadFileResult = await UploadFileAsync(
            "test-image.png",
            "image/png",
            "preview",
            "lesson",
            cancellationToken);

        Assert.True(uploadFileResult.IsSuccess);
    }

    [Fact]
    public async Task UploadImageFile_With_NonexistentAssetType_Should_Fail()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var uploadFileResult = await UploadFileAsync(
            "test-image.png",
            "image/png",
            "picture",
            "lesson",
            cancellationToken);

        Assert.False(uploadFileResult.IsSuccess);
    }

    private async Task<UnitResult<Error>> UploadFileAsync(
        string fileName,
        string contentType,
        string assetType,
        string context,
        CancellationToken cancellationToken)
    {
        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", fileName));

        await using var stream = fileInfo.OpenRead();

        var formFile = new FormFile(stream, 0, stream.Length, "file", fileInfo.Name)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

        var multipartContent = new MultipartFormDataContent();

        var fileContent = new StreamContent(formFile.OpenReadStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        multipartContent.Add(fileContent, "FormFile", fileInfo.Name);

        multipartContent.Add(new StringContent(assetType), "AssetType");
        multipartContent.Add(new StringContent(Guid.NewGuid().ToString()), "EntityId");
        multipartContent.Add(new StringContent(context), "Context");

        var uploadFileResponse = await AppHttpClient.PostAsync("api/files", multipartContent, cancellationToken);

        return await uploadFileResponse.HandleResponseAsync(cancellationToken);
    }
}