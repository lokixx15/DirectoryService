using Amazon.S3;
using FileService.Domain;
using FileService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.IntegrationTests.Features;

public class DeleteFileTests : FileServiceTestsBase
{
    public DeleteFileTests(IntegrationTestsWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task DeleteFile_Should_Success()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var imageId = await UploadTestImageAsync(cancellationToken);

        var response = await AppHttpClient.DeleteAsync($"/api/files/{imageId}", cancellationToken);

        var result = await response.HandleResponseAsync<string>(cancellationToken);

        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var mediaAsset = await dbContext.MediaAssets.FirstOrDefaultAsync(
                mA => mA.Id == imageId
                && mA.MediaStatus == MediaStatus.DELETED,
                cancellationToken);

            Assert.NotNull(mediaAsset);

            var amazonS3Client = Services.GetRequiredService<IAmazonS3>();

            var exception = await Record.ExceptionAsync(() => amazonS3Client.GetObjectMetadataAsync(
                mediaAsset.RawKey.Bucket,
                mediaAsset.RawKey.Key,
                cancellationToken));

            Assert.IsType<AmazonS3Exception>(exception);

            var s3Exception = (AmazonS3Exception)exception;
            Assert.True(s3Exception.ErrorCode == "NoSuchKey" || s3Exception.ErrorCode == "NotFound");
        });
    }

    [Fact]
    public async Task DeleteDeletedFile_Should_Fail()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var imageId = await UploadTestImageAsync(cancellationToken);

        await ExecuteInDb(async dbContext =>
        {
            await dbContext.MediaAssets.Where(mA => mA.Id == imageId).ExecuteUpdateAsync(
                s => s.SetProperty(mA => mA.MediaStatus, MediaStatus.DELETED),
                cancellationToken);
        });

        var response = await AppHttpClient.DeleteAsync($"/api/files/{imageId}", cancellationToken);

        var result = await response.HandleResponseAsync<string>(cancellationToken);

        Assert.False(result.IsSuccess);
    }
}