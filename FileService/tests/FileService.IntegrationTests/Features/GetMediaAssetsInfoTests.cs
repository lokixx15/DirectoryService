using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.IntegrationTests.Infrastructure;
using System.Net.Http.Json;

namespace FileService.IntegrationTests.Features;

public class GetMediaAssetsInfoTests : FileServiceTestsBase
{
    public GetMediaAssetsInfoTests(IntegrationTestsWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetMediaAssetsInfo_Should_Success()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var imageId1 = await UploadTestImageAsync(cancellationToken);
        var imageId2 = await UploadTestImageAsync(cancellationToken);

        var request = new GetMediaAssetsInfoRequest([imageId1, imageId2]);

        var response = await AppHttpClient.PostAsJsonAsync($"/api/files/batch", request, cancellationToken);

        var result = await response.HandleResponseAsync<GetMediaAssetsInfoResponse>(cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.MediaAssets);
    }

    [Fact]
    public async Task GetMediaAssetsInfo_WithoutIds_Should_ReturnEmptyList()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var request = new GetMediaAssetsInfoRequest([]);

        var response = await AppHttpClient.PostAsJsonAsync($"/api/files/batch", request, cancellationToken);

        var result = await response.HandleResponseAsync<GetMediaAssetsInfoResponse>(cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.MediaAssets);
    }
}