using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.FileStorage;
using FileService.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedService.SharedKernel;

namespace FileService.Infrastructure.S3;

public class S3Provider : IS3Provider
{
    private readonly IAmazonS3 _amazonS3;

    private readonly S3Options _options;

    private readonly ILogger<S3Provider> _logger;

    public S3Provider(
        IAmazonS3 amazonS3,
        IOptions<S3Options> options,
        ILogger<S3Provider> logger)
    {
        _amazonS3 = amazonS3;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> UploadFileAsync(
        StorageKey storageKey,
        Stream stream,
        MediaData mediaData,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new PutObjectRequest()
            {
                BucketName = storageKey.Bucket,
                Key = storageKey.Key,
                InputStream = stream,
                ContentType = mediaData.ContentType.Value
            };

            await _amazonS3.PutObjectAsync(request, cancellationToken);

            _logger.LogInformation("File was uploaded successfully");
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to upload file");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> DownloadFileAsync(
        StorageKey storageKey,
        string tempPath,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetObjectRequest()
            {
                BucketName = storageKey.Bucket,
                Key = storageKey.Key
            };

            using var response = await _amazonS3.GetObjectAsync(request, cancellationToken);

            var newPath = Path.Combine(tempPath, storageKey.Key);

            using var fileStream = File.Create(newPath);

            await response.ResponseStream.CopyToAsync(fileStream, cancellationToken);

            _logger.LogInformation("File downloaded: {Path}", tempPath);
            return newPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> DeleteFileAsync(
        StorageKey storageKey,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteObjectRequest()
            {
                BucketName = storageKey.Bucket,
                Key = storageKey.Key
            };

            await _amazonS3.DeleteObjectAsync(request, cancellationToken);

            _logger.LogInformation("File was deleted successfully");
            return storageKey.Key;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to delete file");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> GenerateUploadUrlAsync(
        StorageKey storageKey,
        MediaData mediaData,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = storageKey.Bucket,
                Key = storageKey.Key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(_options.UploadUrlExpirationMinutes),
                Protocol = _options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
                ContentType = mediaData.ContentType.Value
            };

            var url = await _amazonS3.GetPreSignedURLAsync(request);

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate upload url");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> GenerateDownloadUrlAsync(
        StorageKey storageKey)
    {
        try
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = storageKey.Bucket,
                Key = storageKey.Key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddHours(_options.DownloadUrlExpirationHours),
                Protocol = _options.WithSsl ? Protocol.HTTPS : Protocol.HTTP
            };

            var url = await _amazonS3.GetPreSignedURLAsync(request);

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate download url");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<IReadOnlyList<string>, Error>> GenerateDownloadUrlsAsync(
        IEnumerable<StorageKey> storageKeys)
    {
        try
        {
            var tasks = storageKeys.Select(async sK =>
            {
                var request = new GetPreSignedUrlRequest()
                {
                    BucketName = sK.Bucket,
                    Key = sK.Key,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.UtcNow.AddHours(_options.DownloadUrlExpirationHours),
                    Protocol = _options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
                };

                var url = await _amazonS3.GetPreSignedURLAsync(request);
                return url;
            });

            var urls = await Task.WhenAll(tasks);
            return urls.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate download urls");
            return S3ErrorMapper.ToError(ex);
        }
    }
}