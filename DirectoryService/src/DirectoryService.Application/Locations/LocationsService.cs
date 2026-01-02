using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Entities;
using SharedKernel;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Locations;

public class LocationsService : ILocationsService
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateLocationDto> _createLocationDtoValidator;

    public LocationsService(ILocationsRepository locationsRepository,
        IValidator<CreateLocationDto> createLocationDtoValidator)
    {
        _locationsRepository = locationsRepository;
        _createLocationDtoValidator = createLocationDtoValidator;
    }

    public async Task<Result<Guid, Errors>> Create(
        CreateLocationDto locationDto, 
        CancellationToken cancellationToken)
    {
        var locationDtoResult = await _createLocationDtoValidator.ValidateAsync(locationDto, cancellationToken);
        if (!locationDtoResult.IsValid)
            return locationDtoResult.ToErrors();

        var locationName = LocationName.Create(locationDto.Name);

        var locationAddress = LocationAddress.Create(locationDto.Address);

        var locationTimezone = LocationTimezone.Create(locationDto.Timezone);

        var location = Location.Create(
            Guid.Empty,
            locationName.Value,
            locationAddress.Value,
            locationTimezone.Value,
            locationDto.IsActive);

        if (location.IsFailure)
            return location.Error;

        var insertResult = await _locationsRepository.AddAsync(location.Value, cancellationToken);

        if (insertResult.IsFailure)
            return insertResult.Error.ToErrors();

        return location.Value.Id;
    }
}
