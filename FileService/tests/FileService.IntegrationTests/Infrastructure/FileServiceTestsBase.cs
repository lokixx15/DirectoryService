using System.Net.Http.Json;
using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Core;
using FileService.Domain;
using FileService.Infrastructure.Postgres;
using FileService.IntegrationTests.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedService.SharedKernel;

namespace FileService.IntegrationTests.Infrastructure;

[Collection("FileTestsCollection")]
public abstract class FileServiceTestsBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    private readonly IntegrationTestsWebFactory _factory;

    protected HttpClient AppHttpClient { get; init; }

    protected HttpClient HttpClient { get; init; }

    protected IServiceProvider Services => _factory.Services;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _factory.ResetDatabaseAsync();

    protected FileServiceTestsBase(IntegrationTestsWebFactory factory)
    {
        _factory = factory;
        AppHttpClient = factory.CreateClient();
        HttpClient = new HttpClient();
    }

    protected async Task ExecuteInDb<T>(Func<FileServiceDbContext, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<FileServiceDbContext>();

        await action(sut);
    }

    protected async Task ExecuteInDb(Func<FileServiceDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<FileServiceDbContext>();

        await action(sut);
    }

    protected async Task ExecuteInDbAndS3(Func<FileServiceDbContext, IAmazonS3, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FileServiceDbContext>();
        var amazonS3Client = scope.ServiceProvider.GetRequiredService<IAmazonS3>();

        await action(dbContext, amazonS3Client);
    }

    protected async Task<Result<StartMultipartUploadResponse, Error>> StartMultipartUploadAsync(
        FileInfo fileInfo,
        string contentType,
        string assetType,
        string context,
        CancellationToken cancellationToken)
    {
        var request = new StartMultipartUploadRequest(
            fileInfo.Name,
            contentType,
            fileInfo.Length,
            assetType,
            context,
            Guid.NewGuid());

        var response = await AppHttpClient.PostAsJsonAsync(
            "/api/files/multipart/start",
            request,
            cancellationToken);

        var result = await response.HandleResponseAsync<StartMultipartUploadResponse>(cancellationToken);

        if (!result.IsSuccess)
            return result.Error;

        var uploadResponse = result.Value;

        if (uploadResponse.MediaAssetId == Guid.Empty)
            return GeneralErrors.ValueIsNotValid("Invalid MediaAssetId from StartMultipartUpload");

        if (string.IsNullOrWhiteSpace(uploadResponse.UploadId))
            return GeneralErrors.ValueIsNullOrWhitespace("Missing UploadId from StartMultipartUpload");

        if (uploadResponse.ChunkUrls.Count == 0)
            return GeneralErrors.ValueIsNotValid("ChunkUrls is empty");

        if (uploadResponse.ChunkSize <= 0)
            return GeneralErrors.ValueIsNotValid("Invalid ChunkSize");

        await ExecuteInDb(async dbContext =>
        {
            var mediaAsset = await dbContext.MediaAssets
                .FirstOrDefaultAsync(
                    mA => mA.Id == uploadResponse.MediaAssetId
                       && mA.MediaStatus == MediaStatus.UPLOADING,
                    cancellationToken);

            if (mediaAsset is null)
                return GeneralErrors.EntityNotFound("No MediaAsset found in DB for MediaAssetId " + uploadResponse.MediaAssetId);

            return UnitResult.Success<Error>();
        });

        return uploadResponse;
    }

    protected async Task<Guid> UploadTestImageAsync(CancellationToken cancellationToken)
    {
        FileInfo fileInfo = new(Path.Combine(AppContext.BaseDirectory, "Resources", Constants.IMAGE_FILE_NAME));

        var fileName = FileName.Create(fileInfo.Name).Value;
        var contentType = ContentType.Create("image/png").Value;
        var mediaData = MediaData.Create(fileName, contentType, fileInfo.Length, 1).Value;
        var mediaOwner = MediaOwner.Create(Guid.NewGuid(), "lesson").Value;

        var mediaAssetFactory = new MediaAssetFactory();
        var mediaAsset = mediaAssetFactory.CreateForUpload(mediaData, AssetType.PREVIEW, mediaOwner).Value;

        await ExecuteInDb(async dbContext =>
        {
            await dbContext.MediaAssets.AddAsync(mediaAsset);
            await dbContext.SaveChangesAsync(cancellationToken);
        });

        await using var stream = fileInfo.OpenRead();

        var request = new PutObjectRequest()
        {
            BucketName = "previews",
            Key = mediaAsset.Id.ToString(),
            InputStream = stream,
            ContentType = "image/png"
        };

        var amazonS3Client = Services.GetRequiredService<IAmazonS3>();
        await amazonS3Client.PutObjectAsync(request, cancellationToken);

        return mediaAsset.Id;
    }
}