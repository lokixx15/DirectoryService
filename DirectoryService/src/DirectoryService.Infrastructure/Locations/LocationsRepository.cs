using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using SharedService.SharedKernel;

namespace DirectoryService.Infrastructure.Locations;

public sealed class LocationsRepository : ILocationsRepository
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

    public async Task<UnitResult<Error>> ExistsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
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

    public async Task<Result<Guid, Error>> AddAsync(
        Location location,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Locations.AddAsync(location, cancellationToken);
            _logger.LogInformation("Location was added to the database");

            return location.Id;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled when adding location {LocationId}", location.Id);
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add location {LocationId}", location.Id);
            return GeneralErrors.DatabaseAddFailed("Failed to add location");
        }
    }

    public async Task<UnitResult<Error>> SoftDeleteLocationsWithoutActiveDepartments(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        var sql = @"
                    WITH department_locations AS (
                    							  SELECT l.*
                    							  FROM locations AS l
                    							  JOIN department_location AS dl ON dl.location_id = l.id
                    							  WHERE dl.department_id = @departmentId)								  
                    UPDATE locations 
                    SET is_active = false,
                    	deleted_at = NOW() AT TIME ZONE 'UTC'
                    WHERE id IN (
                    	         SELECT dls.id
                    	         FROM department_locations AS dls
                    	         WHERE NOT EXISTS (
                    	         				   SELECT 1
                    	                           FROM department_location AS dl
                    	                           JOIN departments AS d ON dl.department_id = d.id
                    	                           WHERE dl.department_id != @departmentId
                    	         					 AND dl.location_id = dls.id
                    	         	                 AND d.is_active = true) 
                          AND dls.is_active = true);
                    ";

        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                sql,
                [new NpgsqlParameter("@departmentId", departmentId)],
                cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled when deleting locations without active departments");
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete locations without active departments");
            return GeneralErrors.DatabaseDeleteFailed("Failed to delete locations without active departments");
        }
    }
}