using System.Net.Http.Json;
using FileService.Contracts.Dtos;
using FileService.Contracts.Requests;
using FileService.IntegrationTests.Infrastructure;

namespace FileService.IntegrationTests.Features;

public class GetChunkUploadUrlTests : FileServiceTestsBase
{
    public GetChunkUploadUrlTests(IntegrationTestsWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetChunkUploadUrl_Should_Success()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", Constants.VIDEO_FILE_NAME));

        var startMultipartUploadResponse = await StartMultipartUploadAsync(
            fileInfo,
            "video/mp4",
            "video",
            "lesson",
            cancellationToken);

        Assert.True(startMultipartUploadResponse.IsSuccess);

        int partNumber = 2;
        var getChunkUploadRequest = new GetChunkUploadUrlRequest(
            startMultipartUploadResponse.Value.MediaAssetId,
            startMultipartUploadResponse.Value.UploadId,
            partNumber);

        var getChunkUploadUrlResponse = await AppHttpClient.PostAsJsonAsync(
            "/api/files/multipart/url",
            getChunkUploadRequest,
            cancellationToken);

        var getChunkUploadUrlResult = await getChunkUploadUrlResponse.HandleResponseAsync<ChunkUploadUrlDto>(cancellationToken);

        Assert.True(getChunkUploadUrlResult.IsSuccess);
        Assert.True(getChunkUploadUrlResult.Value.PartNumber == partNumber);
        Assert.NotEmpty(getChunkUploadUrlResult.Value.UploadUrl);
    }

    [Fact]
    public async Task GetOutOfRangeChunkUploadUrl_Should_Fail()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", Constants.VIDEO_FILE_NAME));

        var startMultipartUploadResponse = await StartMultipartUploadAsync(
            fileInfo,
            "video/mp4",
            "video",
            "lesson",
            cancellationToken);

        Assert.True(startMultipartUploadResponse.IsSuccess);

        int partNumber = 5;
        var getChunkUploadRequest = new GetChunkUploadUrlRequest(
            startMultipartUploadResponse.Value.MediaAssetId,
            startMultipartUploadResponse.Value.UploadId,
            partNumber);

        var getChunkUploadUrlResponse = await AppHttpClient.PostAsJsonAsync(
            "/api/files/multipart/url",
            getChunkUploadRequest,
            cancellationToken);

        var getChunkUploadUrlResult = await getChunkUploadUrlResponse.HandleResponseAsync<ChunkUploadUrlDto>(cancellationToken);

        Assert.False(getChunkUploadUrlResult.IsSuccess);
    }
}