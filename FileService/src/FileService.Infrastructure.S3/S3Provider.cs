using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Contracts;
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

    private readonly SemaphoreSlim _semaphore;

    public S3Provider(
        IAmazonS3 amazonS3,
        IOptions<S3Options> options,
        ILogger<S3Provider> logger)
    {
        _amazonS3 = amazonS3;
        _options = options.Value;
        _logger = logger;
        _semaphore = new SemaphoreSlim(_options.MaxConcurrentRequests);
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
        IEnumerable<StorageKey> storageKeys,
        CancellationToken cancellationToken)
    {
        var expirationTime = DateTime.UtcNow.AddHours(_options.DownloadUrlExpirationHours);

        try
        {
            var tasks = storageKeys.Select(async sK =>
            {
                await _semaphore.WaitAsync(cancellationToken);

                try
                {
                    var request = new GetPreSignedUrlRequest()
                    {
                        BucketName = sK.Bucket,
                        Key = sK.Key,
                        Verb = HttpVerb.GET,
                        Expires = expirationTime,
                        Protocol = _options.WithSsl ? Protocol.HTTPS : Protocol.HTTP,
                    };

                    var url = await _amazonS3.GetPreSignedURLAsync(request);
                    return url;
                }
                finally
                {
                    _semaphore.Release();
                }
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

    public async Task<Result<string, Error>> StartMultipartUploadAsync(
        StorageKey storageKey,
        MediaData mediaData,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new InitiateMultipartUploadRequest()
            {
                BucketName = storageKey.Bucket,
                Key = storageKey.Key,
                ContentType = mediaData.ContentType.Value
            };

            var response = await _amazonS3.InitiateMultipartUploadAsync(request, cancellationToken);

            return response.UploadId;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to initiate multipart upload");
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<ChunkUploadUrl, Error>> GenerateChunkUploadUrlAsync(
        StorageKey storageKey,
        string uploadId,
        int partNumber)
    {
        try
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = storageKey.Bucket,
                Key = storageKey.Key,
                Verb = HttpVerb.PUT,
                PartNumber = partNumber,
                UploadId = uploadId,
                Expires = DateTime.UtcNow.AddMinutes(_options.UploadUrlExpirationMinutes)
            };

            var url = await _amazonS3.GetPreSignedURLAsync(request);

            return new ChunkUploadUrl(partNumber, url);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate chunk upload url by uploadId {Id}", uploadId);
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<IReadOnlyList<ChunkUploadUrl>, Error>> GenerateAllChunkUploadUrlsAsync(
        StorageKey storageKey,
        string uploadId,
        int totalChunks,
        CancellationToken cancellationToken)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(_options.UploadUrlExpirationMinutes);

        try
        {
            var tasks = Enumerable.Range(1, totalChunks).Select(async partNumber =>
            {
                await _semaphore.WaitAsync(cancellationToken);

                try
                {
                    var request = new GetPreSignedUrlRequest()
                    {
                        BucketName = storageKey.Bucket,
                        Key = storageKey.Key,
                        Verb = HttpVerb.PUT,
                        PartNumber = partNumber,
                        UploadId = uploadId,
                        Expires = expirationTime
                    };

                    var url = await _amazonS3.GetPreSignedURLAsync(request);

                    return new ChunkUploadUrl(partNumber, url);
                }
                finally
                {
                    _semaphore.Release();
                }
            });

            var urls = await Task.WhenAll(tasks);

            return urls.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate all chunk upload urls by uploadId {Id}", uploadId);
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<CompleteMultipartUploadDto, Error>> CompleteMultipartUploadAsync(
        StorageKey storageKey,
        string uploadId,
        List<PartETagDto> partETags,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new CompleteMultipartUploadRequest()
            {
                BucketName = storageKey.Bucket,
                Key = storageKey.Key,
                UploadId = uploadId,
                PartETags = partETags.Select(pET => new PartETag
                {
                    PartNumber = pET.PartNumber,
                    ETag = pET.ETag,
                })
                .ToList()
            };

            var response = await _amazonS3.CompleteMultipartUploadAsync(request, cancellationToken);

            return new CompleteMultipartUploadDto(response.Location, response.BucketName, response.Key);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to complete multipart upload with uploadId {Id}", uploadId);
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<UnitResult<Error>> AbortMultipartUploadAsync(
        StorageKey storageKey,
        string uploadId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new AbortMultipartUploadRequest()
            {
                BucketName = storageKey.Bucket,
                Key = storageKey.Key,
                UploadId = uploadId
            };

            await _amazonS3.AbortMultipartUploadAsync(request, cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to abort multipart upload with uploadId {Id}", uploadId);
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<IReadOnlyList<MultipartUploadDto>, Error>> ListMultipartUploadAsync(
        string bucketName,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new ListMultipartUploadsRequest()
            {
                BucketName = bucketName
            };

            var response = await _amazonS3.ListMultipartUploadsAsync(request, cancellationToken);

            return response.MultipartUploads.Select(mU =>
                new MultipartUploadDto(mU.Key, mU.UploadId)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to list multipart upload");
            return S3ErrorMapper.ToError(ex);
        }
    }
}