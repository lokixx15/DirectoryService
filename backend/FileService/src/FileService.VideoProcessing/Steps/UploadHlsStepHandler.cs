using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.FileStorage;
using FileService.Domain;
using FileService.Domain.MediaProcessing;
using FileService.VideoProcessing.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedService.SharedKernel;

namespace FileService.VideoProcessing.Steps;

public class UploadHlsStepHandler : IProcessingStepHandler
{
    private readonly IS3Provider _s3Provider;
    private readonly VideoProcessingOptions _options;
    private readonly ILogger<UploadHlsStepHandler> _logger;

    public UploadHlsStepHandler(
        IS3Provider s3Provider,
        IOptions<VideoProcessingOptions> options,
        ILogger<UploadHlsStepHandler> logger)
    {
        _s3Provider = s3Provider;
        _options = options.Value;
        _logger = logger;
    }

    public StepType StepType => StepType.UPLOAD_HLS;

    public async Task<Result<ProcessingContext, Error>> ExecuteAsync(
        ProcessingContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting upload hls to S3 for VideoAsset {VideoAssetId}",
            context.VideoAsset.Id);

        if (string.IsNullOrWhiteSpace(context.HlsOutputDirectory))
            return FileErrors.HlsProcessingFailed("Hls output directory is not set");

        if (!Directory.Exists(context.HlsOutputDirectory))
            return FileErrors.HlsProcessingFailed("Hls output directory doesn't exist");

        string[] hlsFiles = Directory.GetFiles(context.HlsOutputDirectory, "*", SearchOption.AllDirectories);
        if (hlsFiles.Length == 0)
            return FileErrors.HlsProcessingFailed("No hls files found in hls output directory");

        var semaphoreSlim = new SemaphoreSlim(_options.UploadDegreeOfParallelism);

        var tasks = hlsFiles.Select(async hF =>
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                return await UploadHlsFileAsync(context.VideoAsset.HslRootKey, hF, cancellationToken);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        var firstFailure = results.FirstOrDefault(r => r.IsFailure);
        if (firstFailure.IsFailure)
            return firstFailure.Error;

        _logger.LogInformation("Successfully uploaded hls files for videoAsset with id {VideoAssetId}",
            context.VideoAsset.Id);

        var setProgressResult = context.VideoProcess.ReportStepProgress(100);
        if (setProgressResult.IsFailure)
        {
            _logger.LogError("Failed to set progress for VideoAsset with id {VideoAssetId} in {Step}",
                context.VideoAsset.Id, StepType.ToString());
            return setProgressResult.Error;
        }

        return context;
    }

    private async Task<UnitResult<Error>> UploadHlsFileAsync(
        StorageKey hlsRootKey,
        string localFilePath,
        CancellationToken cancellationToken)
    {
        var fileNameResult = FileName.Create(Path.GetFileName(localFilePath));
        if (fileNameResult.IsFailure)
            return fileNameResult.Error;

        var contentTypeResult = ContentType.Create(GetContentType(localFilePath));
        if (contentTypeResult.IsFailure)
            return contentTypeResult.Error;

        var storageKeyResult = hlsRootKey.AppendSegment(fileNameResult.Value.Value);
        if (storageKeyResult.IsFailure)
            return storageKeyResult.Error;

        await using FileStream fileStream = File.OpenRead(localFilePath);

        var mediaDataResult = MediaData.Create(
            fileNameResult.Value,
            contentTypeResult.Value,
            fileStream.Length,
            1);

        return await _s3Provider.UploadFileAsync(
            storageKeyResult.Value,
            fileStream,
            mediaDataResult.Value,
            cancellationToken);
    }

    private string GetContentType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension switch
        {
            ".m3u8" => "application/vnd.apple.mpegurl",
            ".ts" => "video/mp2t",
            _ => "application/octet-stream"
        };
    }
}