using FileService.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace FileService.IntegrationTests.Features;

public class DownloadFileTests : FileServiceTestsBase
{
    public DownloadFileTests(IntegrationTestsWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task DownloadFile_Should_Success()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var imageId = await UploadTestImageAsync(cancellationToken);

        var response = await AppHttpClient.GetAsync($"/api/files/{imageId}", cancellationToken);

        var result = await response.HandleResponseAsync<string>(cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.True(File.Exists(result.Value));
    }

    [Fact]
    public async Task DownloadFile_WithNonexistanceId_Should_Fail()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var imageId = await UploadTestImageAsync(cancellationToken);

        var response = await AppHttpClient.GetAsync($"/api/files/{Guid.NewGuid()}", cancellationToken);

        var result = await response.HandleResponseAsync<string>(cancellationToken);

        Assert.False(result.IsSuccess);
    }
}