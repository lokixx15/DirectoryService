using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using FileService.Core.Abstractions.FileStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Infrastructure.S3;

public static class DependencyInjectionS3
{
    public static IServiceCollection AddS3Infrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var s3Section = configuration.GetSection("S3Options");
        if (!s3Section.Exists())
        {
            throw new ConfigurationException("S3Options section does not exist in configuration");
        }

        var s3Options = configuration.GetSection("S3Options").Get<S3Options>()
            ?? throw new ConfigurationException("Missing s3 configuration section: S3Options");

        var s3Config = new AmazonS3Config()
        {
            ServiceURL = s3Options.Endpoint,
            ForcePathStyle = true,
            UseHttp = !s3Options.WithSsl,
        };

        var credentials = new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey);

        services.AddSingleton<IAmazonS3>(new AmazonS3Client(credentials, s3Config));

        services.AddHostedService<S3BucketInitializer>();

        services.Configure<S3Options>(configuration.GetSection("S3Options"));

        services.AddScoped<IS3Provider, S3Provider>();

        return services;
    }
}