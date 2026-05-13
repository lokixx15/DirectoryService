using System.Data.Common;
using Amazon.Runtime;
using Amazon.S3;
using FileService.Core.Abstractions.Database;
using FileService.Infrastructure.Postgres;
using FileService.Infrastructure.S3;
using FileService.IntegrationTests.Mocks;
using FileService.VideoProcessing.FfmpegProcess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;
using Respawn;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;

namespace FileService.IntegrationTests.Infrastructure;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres")
        .WithDatabase("file_service_db_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly MinioContainer _minioContainer = new MinioBuilder("minio/minio")
        .WithUsername("minioadmin")
        .WithPassword("minioadmin")
        .Build();

    private Respawner _respawner = null!;
    private DbConnection _dbConnection = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        await _minioContainer.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FileServiceDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        _dbConnection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
        await _dbConnection.OpenAsync();

        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await _minioContainer.DisposeAsync();

        await _dbConnection.CloseAsync();
        await _dbConnection.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.Tests.json"), optional: true);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<FileServiceDbContext>();
            services.RemoveAll<IReadFileServiceDbContext>();
            services.RemoveAll<IAmazonS3>();

            services.AddScoped(_ => new FileServiceDbContext(
                _postgresContainer.GetConnectionString()));

            services.AddScoped<IReadFileServiceDbContext, FileServiceDbContext>(
                _ => new FileServiceDbContext(_postgresContainer.GetConnectionString()));

            services.AddSingleton<IAmazonS3>(sp =>
            {
                var s3Options = sp.GetRequiredService<IOptions<S3Options>>().Value;

                var s3Config = new AmazonS3Config()
                {
                    ServiceURL = $"http://{_minioContainer.Hostname}:{_minioContainer.GetMappedPublicPort()}",
                    ForcePathStyle = true,
                    UseHttp = true,
                };

                var credentials = new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey);
                return new AmazonS3Client(credentials, s3Config);
            });

            services.RemoveAll<IFfmpegProcessRunner>();
            services.AddSingleton<IFfmpegProcessRunner, FakeHlsGenerator>();
        });
    }

    private async Task InitializeRespawner()
    {
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"]
            });
    }
}