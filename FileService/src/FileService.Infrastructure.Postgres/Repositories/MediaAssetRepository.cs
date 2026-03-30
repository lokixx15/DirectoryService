using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using FileService.Core;
using FileService.Domain.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;

namespace FileService.Infrastructure.Postgres.Repositories;

public class MediaAssetRepository : IMediaRepository
{
    private readonly FileServiceDbContext _dbContext;
    private readonly ILogger<MediaAssetRepository> _logger;

    public MediaAssetRepository(
        FileServiceDbContext dbContext,
        ILogger<MediaAssetRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<MediaAsset, Error>> GetByAsync(
        Expression<Func<MediaAsset, bool>> predicate,
        CancellationToken cancellationToken)
    {
        try
        {
            var mediaAsset = await _dbContext.MediaAssets
                .FirstOrDefaultAsync(predicate, cancellationToken);

            if (mediaAsset == null)
            {
                _logger.LogError("Media asset was not found in the database");
                return GeneralErrors.EntityNotFound("Media asset");
            }

            _logger.LogInformation("Media asset was obtained from the database");
            return mediaAsset;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled while reading media asset");
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when reading media asset");
            return GeneralErrors.DatabaseReadFailed(ex.Message);
        }
    }

    public async Task<Result<Guid, Error>> AddAsync(
        MediaAsset mediaAsset,
        CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.MediaAssets.AddAsync(mediaAsset, cancellationToken);
            _logger.LogInformation("Media asset was added to the database");

            return mediaAsset.Id;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled when adding media asset with id {MediaAssetId}", mediaAsset.Id);
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add media asset with id {MediaAssetId}", mediaAsset.Id);
            return GeneralErrors.DatabaseAddFailed("Failed to add media asset");
        }
    }

    public async Task<UnitResult<Error>> DeleteById(
        Guid mediaAssetId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.MediaAssets
                .Where(mA => mA.Id == mediaAssetId)
                .ExecuteDeleteAsync(cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled when deleting media asset with id {MediaAssetId}", mediaAssetId);
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete media asset with id {MediaAssetId}", mediaAssetId);
            return GeneralErrors.DatabaseAddFailed("Failed to de;ete media asset");
        }
    }
}