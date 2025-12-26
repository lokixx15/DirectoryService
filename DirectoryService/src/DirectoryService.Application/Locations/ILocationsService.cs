using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Locations;

public interface ILocationsService
{
    Task<Guid> Create(CreateLocationDto locationDto, CancellationToken cancellationToken);
}
