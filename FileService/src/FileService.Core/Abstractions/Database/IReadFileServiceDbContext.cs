using FileService.Domain.Assets;

namespace FileService.Core.Abstractions.Database;

public interface IReadFileServiceDbContext
{
    IQueryable<MediaAsset> ReadMediaAssets { get; }
}