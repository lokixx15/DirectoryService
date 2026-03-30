using Amazon.S3;
using FileService.Domain;
using SharedService.SharedKernel;

namespace FileService.Infrastructure.S3;

public static class S3ErrorMapper
{
    public static Error ToError(Exception exception) => exception switch
    {
        AmazonS3Exception { ErrorCode: "NoSuchBucket" or "NoSuchKey" } => FileErrors.NotFound(),
        AmazonS3Exception { ErrorCode: "AccessDenied" } => FileErrors.Forbidden(),
        AmazonS3Exception { ErrorCode: "InvalidObjectState" } => FileErrors.Conflict(),
        _ => FileErrors.InternalError()
    };
}