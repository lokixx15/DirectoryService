using DirectoryService.Domain.Entities;
using DirectoryService.Application.Locations;

namespace DirectoryService.Infrastructure.Repositories;

public class EfCoreLocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public EfCoreLocationsRepository(DirectoryServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {  
        await _dbContext.AddAsync(location, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return location.Id;
    }
}
