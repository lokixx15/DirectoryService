using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using SharedKernel;
using System.Data.Common;

namespace DirectoryService.Infrastructure.Locations;

public class LocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<LocationsRepository> _logger;

    public LocationsRepository(
        DirectoryServiceDbContext dbContext, 
        ILogger<LocationsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken)
    {  
        await _dbContext.Locations.AddAsync(location, cancellationToken);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Location with name {Name} has been added to the database", location.Name);

            return location.Id;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
                return GeneralErrors.ValueAlreadyExists("Location already exists");

            _logger.LogError(ex, "Database update error when creating location with name {Name}", location.Name.Value);

            return GeneralErrors.DatabaseInsertFailed(ex.Message);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled when creating location with name {Name}", location.Name.Value);
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when creating location with name {Name}", location.Name.Value);
            return GeneralErrors.DatabaseInsertFailed(ex.Message);
        }
    }

    public async Task<UnitResult<Error>> ExistsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        try
        { 
            var idList = ids.ToList();

            var existingIds = await _dbContext.Locations
                .AsNoTracking()
                .Where(l => idList.Contains(l.Id))
                .Select(l => l.Id)
                .ToListAsync(cancellationToken);

            var nonexistentIds = idList.Except(existingIds).ToList();

            return nonexistentIds.Any()
                ? GeneralErrors.EntityNotFound("Locations", $"Locations with ids {string.Join(",", nonexistentIds)}")
                : UnitResult.Success<Error>();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled checking the existence of locations with ids {Ids}", ids);
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when checking the existence of locations with ids {Ids}", ids);
            return GeneralErrors.DatabaseReadFailed("Locations existence check failed");
        }
    }
}
