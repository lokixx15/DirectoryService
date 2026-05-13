using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using FileService.Core.Abstractions.FileStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        services.Configure<S3Options>(configuration.GetSection("S3Options"));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Options = sp.GetRequiredService<IOptions<S3Options>>().Value;

            var s3Config = new AmazonS3Config()
            {
                ServiceURL = s3Options.Endpoint,
                ForcePathStyle = true,
                UseHttp = !s3Options.WithSsl,
            };

            var credentials = new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey);

            return new AmazonS3Client(credentials, s3Config);
        });

        services.AddHostedService<S3BucketInitializer>();

        services.AddScoped<IS3Provider, S3Provider>();

        services.AddTransient<IChunkSizeCalculator, ChunkSizeCalculator>();

        return services;
    }
}