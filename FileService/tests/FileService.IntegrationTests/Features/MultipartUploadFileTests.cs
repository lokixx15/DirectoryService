using System.Net.Http.Json;
using Amazon.S3;
using FileService.Contracts.Dtos;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Domain;
using FileService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.IntegrationTests.Features;

public class MultipartUploadFileTests : FileServiceTestsBase
{
    private readonly IntegrationTestsWebFactory _factory;

    public MultipartUploadFileTests(
        IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task MultipartUpload_FullCycle_PersistsAsset()
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

        var partETags = await UploadChunksAsync(fileInfo, startMultipartUploadResponse.Value, cancellationToken);

        await CompleteMultipartUpload(startMultipartUploadResponse.Value, partETags, cancellationToken);

        await ExecuteInDb(async dbContext =>
        {
            var mediaAsset = await dbContext.MediaAssets.FirstOrDefaultAsync(
                mA => mA.Id == startMultipartUploadResponse.Value.MediaAssetId
                && mA.MediaStatus == MediaStatus.UPLOADED,
                cancellationToken);

            Assert.NotNull(mediaAsset);

            var amazonS3Client = _factory.Services.GetRequiredService<IAmazonS3>();

            var getObjectResponse = await amazonS3Client.GetObjectAsync(
                mediaAsset.RawKey.Bucket,
                mediaAsset.RawKey.Key,
                cancellationToken);

            Assert.Equal(getObjectResponse.Key, mediaAsset.RawKey.Key);
            Assert.Equal(getObjectResponse.ContentLength, fileInfo.Length);
        });
    }

    private async Task<IReadOnlyList<PartETagDto>> UploadChunksAsync(
        FileInfo fileInfo,
        StartMultipartUploadResponse startMultipartUploadResponse,
        CancellationToken cancellationToken)
    {
        await using var stream = fileInfo.OpenRead();

        var partETags = new List<PartETagDto>();

        foreach (var chunckUploadUrl in startMultipartUploadResponse.ChunkUrls.OrderBy(c => c.PartNumber))
        {
            var chunck = new byte[startMultipartUploadResponse.ChunkSize];
            var bytesRead = await stream.ReadAsync(chunck.AsMemory(0, startMultipartUploadResponse.ChunkSize), cancellationToken);
            if (bytesRead == 0)
                break;

            var content = new ByteArrayContent(chunck.AsMemory(0, bytesRead).ToArray());

            var response = await HttpClient.PutAsync(chunckUploadUrl.UploadUrl, content, cancellationToken);

            var eTag = response.Headers.ETag?.Tag.Trim('"');

            partETags.Add(new PartETagDto(chunckUploadUrl.PartNumber, eTag!));
        }

        return partETags;
    }

    private async Task CompleteMultipartUpload(
        StartMultipartUploadResponse startMultipartUploadResponse,
        IReadOnlyList<PartETagDto> partETags,
        CancellationToken cancellationToken)
    {
        var completeMultipartUploadRequest = new CompleteMultipartUploadRequest(
            startMultipartUploadResponse.MediaAssetId,
            startMultipartUploadResponse.UploadId,
            partETags.ToList());

        var competeMultipartUploadResponse = await AppHttpClient.PostAsJsonAsync(
            "/api/files/multipart/complete",
            completeMultipartUploadRequest,
            cancellationToken);

        var competeMultipartUploadResult = await competeMultipartUploadResponse
            .HandleResponseAsync<CompleteMultipartUploadResponse>(cancellationToken);

        Assert.True(competeMultipartUploadResult.IsSuccess);
    }
}