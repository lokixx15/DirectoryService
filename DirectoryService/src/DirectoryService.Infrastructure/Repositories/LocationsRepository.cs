using DirectoryService.Domain.Entities;
using DirectoryService.Application.Locations;
using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public LocationsRepository(DirectoryServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken)
    {  
        await _dbContext.AddAsync(location, cancellationToken);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);

            return location.Id;
        }
        catch(Exception ex) 
        {
            return GeneralErrors.InsertFailed(ex.Message, "database.insert.failed");
        }
    }
}
