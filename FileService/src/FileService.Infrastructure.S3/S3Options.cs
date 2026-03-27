namespace FileService.Infrastructure.S3;

public class S3Options
{
    public string Endpoint { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public bool WithSsl { get; set; }

    public string[] RequiredBuckets { get; set; } = [];

    public int UploadUrlExpirationMinutes { get; set; }

    public int DownloadUrlExpirationHours { get; set; }

    public int MaxConcurrentRequests { get; set; }
}