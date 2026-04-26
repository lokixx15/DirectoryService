using System.Net.Http.Json;
using Amazon.S3;
using FileService.Contracts.Requests;
using FileService.Domain;
using FileService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.IntegrationTests.Features;

public class AbortMultipartUploadTests : FileServiceTestsBase
{
    public AbortMultipartUploadTests(IntegrationTestsWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task AbortMultipartUpload_Should_Succeed()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", "test-file.mp4"));

        var startMultipartUploadResponse = await StartMultipartUploadAsync(
            fileInfo,
            "video/mp4",
            "video",
            "lesson",
            cancellationToken);

        Assert.True(startMultipartUploadResponse.IsSuccess);

        var abortMultipartUploadRequest = new AbortMultipartUploadRequest(
            startMultipartUploadResponse.Value.MediaAssetId,
            startMultipartUploadResponse.Value.UploadId);

        var abortMultipartUploadResponse = await AppHttpClient.PostAsJsonAsync(
            "api/files/multipart/abort",
            abortMultipartUploadRequest,
            cancellationToken);

        var abortMultipartUploadResult = await abortMultipartUploadResponse.HandleResponseAsync(cancellationToken);

        Assert.True(abortMultipartUploadResult.IsSuccess);

        await ExecuteInDbAndS3(async (dbContext, amazonS3Client) =>
        {
            var mediaAsset = await dbContext.MediaAssets.FirstOrDefaultAsync(
                mA => mA.Id == startMultipartUploadResponse.Value.MediaAssetId
                && mA.MediaStatus == MediaStatus.FAILED,
                cancellationToken);

            Assert.NotNull(mediaAsset);

            var exception = await Record.ExceptionAsync(() => amazonS3Client.GetObjectMetadataAsync(
                mediaAsset.RawKey.Bucket,
                mediaAsset.RawKey.Key,
                cancellationToken));

            Assert.IsType<AmazonS3Exception>(exception);

            var s3Exception = (AmazonS3Exception)exception;
            Assert.True(s3Exception.ErrorCode == "NoSuchKey" || s3Exception.ErrorCode == "NotFound");
        });
    }
}