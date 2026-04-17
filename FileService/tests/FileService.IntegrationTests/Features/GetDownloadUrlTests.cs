using FileService.IntegrationTests.Infrastructure;

namespace FileService.IntegrationTests.Features;

public class GetDownloadUrlTests : FileServiceTestsBase
{
    public GetDownloadUrlTests(IntegrationTestsWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetChunkUploadUrl_Should_Success()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var imageId = await UploadTestImageAsync(cancellationToken);

        var response = await AppHttpClient.GetAsync($"/api/files/{imageId}/url", cancellationToken);

        var result = await response.HandleResponseAsync<string>(cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value);
    }

    [Fact]
    public async Task GetChunkUploadUrl_WithNonexistanceId_Should_Fail()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var response = await AppHttpClient.GetAsync($"/api/files/{Guid.NewGuid()}/url", cancellationToken);

        var result = await response.HandleResponseAsync<string>(cancellationToken);

        Assert.False(result.IsSuccess);
    }
}