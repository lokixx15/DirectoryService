using FileService.Core.Abstractions.Database;
using FileService.Domain.Assets;
using FileService.Domain.MediaProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileService.Infrastructure.Postgres;

public class FileServiceDbContext : DbContext, IReadFileServiceDbContext
{
    private readonly string _connectionString;

    public FileServiceDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
        optionsBuilder
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(FileServiceDbContext).Assembly);
    }

    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    public DbSet<VideoProcess> VideoProcesses => Set<VideoProcess>();

    public IQueryable<MediaAsset> ReadMediaAssets => Set<MediaAsset>().AsNoTracking().AsQueryable();

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
}