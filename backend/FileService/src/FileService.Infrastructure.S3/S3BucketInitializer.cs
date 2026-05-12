using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.S3;

public class S3BucketInitializer : BackgroundService
{
    private readonly S3Options _options;
    private readonly IAmazonS3 _amazonS3;
    private readonly ILogger<S3BucketInitializer> _logger;

    public S3BucketInitializer(
        IOptions<S3Options> options,
        IAmazonS3 amazonS3,
        ILogger<S3BucketInitializer> logger)
    {
        _options = options.Value;
        _amazonS3 = amazonS3;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("S3 bucket initialization started");

            if (_options.RequiredBuckets.Length == 0)
            {
                _logger.LogInformation("S3 bucket initialization service required buckets");
                throw new ArgumentException("Required buckets is required");
            }

            _logger.LogInformation("Starting S3 buckets initialization");

            var tasks = _options.RequiredBuckets.Select(b =>
                InitializeBucketAsync(b, stoppingToken));

            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("S3 bucket initialization service was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Critical error during S3 bucket initialization");
            throw;
        }
    }

    private async Task InitializeBucketAsync(
        string bucketName,
        CancellationToken cancellationToken)
    {
        try
        {
            var bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);

            if (bucketExist)
            {
                _logger.LogInformation("Bucket {Bucket} already exists", bucketName);
                return;
            }

            _logger.LogInformation("Creating bucket '{BucketName}'", bucketName);

            var bucketRequst = new PutBucketRequest()
            {
                BucketName = bucketName
            };

            await _amazonS3.PutBucketAsync(bucketRequst, cancellationToken);

            var policy = $$"""
                {
                    "Version": "2012-10-17",
                    "Statement": [
                    {
                        "Effect": "Allow",
                        "Principal": {
                        "AWS": [""]
                        },
                    "Action": ["s3:GetObject"],
                    "Resource": ["arn:aws:s3:::{{bucketName}}/"]
                    }]
                }
                """;

            var bucketPolicyRequest = new PutBucketPolicyRequest()
            {
                BucketName = bucketName,
                Policy = policy
            };

            await _amazonS3.PutBucketPolicyAsync(bucketPolicyRequest, cancellationToken);

            _logger.LogInformation("Bucket '{BucketName}' created successfully", bucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize bucket '{BucketName}'", bucketName);
            throw;
        }
    }
}