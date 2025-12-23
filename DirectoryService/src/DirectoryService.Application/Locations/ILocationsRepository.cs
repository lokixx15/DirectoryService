using DirectoryService.Domain.Entities;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);
}
