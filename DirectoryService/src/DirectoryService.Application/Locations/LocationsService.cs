using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Application.Locations;

public class LocationsService : ILocationsService
{
    private readonly ILocationsRepository _locationsRepository;

    public LocationsService(ILocationsRepository locationsRepository)
    {
        _locationsRepository = locationsRepository;
    }

    public async Task<Guid> Create(
        CreateLocationDto locationDto, 
        CancellationToken cancellationToken)
    {
        var locationName = LocationName.Create(locationDto.Name);

        var locationAddress = LocationAddress.Create(locationDto.Address);

        var locationTimezone = LocationTimezone.Create(locationDto.Timezone);

        var location = Location.Create(
            Guid.Empty,
            locationName.Value,
            locationAddress.Value,
            locationTimezone.Value,
            locationDto.IsActive);

        await _locationsRepository.AddAsync(location.Value, cancellationToken);

        return location.Value.Id;
    }
}
