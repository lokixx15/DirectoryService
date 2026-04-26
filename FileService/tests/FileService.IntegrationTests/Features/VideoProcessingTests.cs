using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using FileService.Core;
using FileService.Domain;
using FileService.Domain.Assets;
using FileService.Domain.MediaProcessing;
using FileService.IntegrationTests.Infrastructure;
using FileService.VideoProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.IntegrationTests.Features;

public class VideoProcessingTests : FileServiceTestsBase
{
    public VideoProcessingTests(IntegrationTestsWebFactory factory)
    : base(factory) { }

    [Fact]
    public async Task ProcessVideo_WhenValidVideoUploaded_ShouldCompleteVideoProcessingSuccessfully()
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        await using var scope = Services.CreateAsyncScope();
        var processingService = scope.ServiceProvider.GetRequiredService<IVideoProcessingService>();

        var videoAssetId = await UploadTestVideoAsync(cancellationToken);

        var result = await processingService.ProcessVideoAsync(videoAssetId, cancellationToken);

        Assert.True(result.IsSuccess);

        await ExecuteInDbAndS3(async (dbContext, amazonS3Client) =>
        {
            var mediaAsset = await dbContext.MediaAssets
                .AsNoTracking()
                .FirstOrDefaultAsync(mA => mA.Id == videoAssetId, cancellationToken);

            var videoProcess = await dbContext.VideoProcesses
                .AsNoTracking()
                .FirstOrDefaultAsync(vP => vP.Id == videoAssetId, cancellationToken);

            Assert.NotNull(mediaAsset);
            Assert.Equal(MediaStatus.READY, mediaAsset.MediaStatus);

            Assert.NotNull(mediaAsset.FinalKey);
            Assert.Equal($"hls/{videoAssetId}/master.m3u8", mediaAsset.FinalKey.Value);

            Assert.NotNull(videoProcess);
            Assert.Equal(VideoProcessStatus.SUCCEEDED, videoProcess.Status);

            VideoAsset? videoAsset = mediaAsset as VideoAsset;
            Assert.NotNull(videoAsset);
            Assert.NotNull(videoAsset.RawKey);

            var listRequest = new ListObjectsV2Request
            {
                BucketName = VideoAsset.BUCKET,
                Prefix = mediaAsset.FinalKey.Prefix,
            };

            var listObjectResponse = await amazonS3Client.ListObjectsV2Async(listRequest, cancellationToken);

            Assert.NotEmpty(listObjectResponse.S3Objects);

            var getMetadataRequest = new GetObjectMetadataRequest
            {
                BucketName = VideoAsset.BUCKET,
                Key = mediaAsset.FinalKey.Value,
            };

            var objectData = await amazonS3Client.GetObjectMetadataAsync(getMetadataRequest, cancellationToken);
            Assert.NotNull(objectData);

            var exception = await Assert.ThrowsAsync<AmazonS3Exception>(async () =>
                await amazonS3Client.GetObjectMetadataAsync(VideoAsset.BUCKET, videoAsset.RawKey.Value, cancellationToken));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        });
    }

    private async Task<Guid> UploadTestVideoAsync(CancellationToken cancellationToken)
    {
        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", Constants.VIDEO_FILE_NAME));

        var fileName = FileName.Create(fileInfo.Name).Value;
        var contentType = ContentType.Create("video/mp4").Value;
        var mediaData = MediaData.Create(fileName, contentType, fileInfo.Length, 1).Value;
        var mediaOwner = MediaOwner.Create(Guid.NewGuid(), "lesson").Value;

        var mediaAssetFactory = new MediaAssetFactory();
        var mediaAsset = mediaAssetFactory.CreateForUpload(mediaData, AssetType.VIDEO, mediaOwner).Value;
        mediaAsset.MarkUploaded(DateTime.UtcNow);

        await ExecuteInDb(async dbContext =>
        {
            await dbContext.MediaAssets.AddAsync(mediaAsset);
            await dbContext.SaveChangesAsync(cancellationToken);
        });

        await using var stream = fileInfo.OpenRead();

        var request = new PutObjectRequest()
        {
            BucketName = "videos",
            Key = mediaAsset.Id.ToString(),
            InputStream = stream,
            ContentType = "video/mp4"
        };

        var amazonS3Client = Services.GetRequiredService<IAmazonS3>();
        await amazonS3Client.PutObjectAsync(request, cancellationToken);

        return mediaAsset.Id;
    }
}