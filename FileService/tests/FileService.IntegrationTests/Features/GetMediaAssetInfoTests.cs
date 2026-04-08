using FileService.Contracts.Responses;
using FileService.IntegrationTests.Infrastructure;

namespace FileService.IntegrationTests.Features;

public class GetMediaAssetInfoTests : FileServiceTestsBase
{
    public GetMediaAssetInfoTests(IntegrationTestsWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetMediaAssetInfo_Should_Success()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var imageId = await UploadTestImageAsync(cancellationToken);

        var response = await AppHttpClient.GetAsync($"/api/files/{imageId}/info", cancellationToken);

        var result = await response.HandleResponseAsync<GetMediaAssetInfoResponse>(cancellationToken);

        var mediaAsset = result.Value.mediaAsset;
        var fileInfo = mediaAsset.FileInfo;

        Assert.Equal(imageId, mediaAsset.Id);
        Assert.NotNull(mediaAsset.Status);
        Assert.NotNull(mediaAsset.AssetType);
        Assert.True(mediaAsset.CreatedAt <= DateTime.UtcNow);
        Assert.True(mediaAsset.UpdatedAt <= DateTime.UtcNow);
        Assert.NotNull(fileInfo.FileName);
        Assert.NotNull(fileInfo.ContentType);
        Assert.True(fileInfo.size > 0);
    }

    [Fact]
    public async Task GetMediaAssetInfo_WithNonexistanceId_Should_Fail()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var response = await AppHttpClient.GetAsync($"/api/files/{Guid.NewGuid()}/info", cancellationToken);

        var result = await response.HandleResponseAsync<GetMediaAssetInfoResponse>(cancellationToken);

        Assert.False(result.IsSuccess);
    }
}