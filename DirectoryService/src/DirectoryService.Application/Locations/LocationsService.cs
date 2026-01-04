using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Entities;
using SharedKernel;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using DirectoryService.Application.Validation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Locations;

public class LocationsService : ILocationsService
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateLocationDto> _createLocationDtoValidator;
    private readonly ILogger<LocationsService> _logger;

    public LocationsService(ILocationsRepository locationsRepository,
        IValidator<CreateLocationDto> createLocationDtoValidator,
        ILogger<LocationsService> logger)
    {
        _locationsRepository = locationsRepository;
        _createLocationDtoValidator = createLocationDtoValidator;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Create(
        CreateLocationDto locationDto, 
        CancellationToken cancellationToken)
    {
        var locationDtoResult = await _createLocationDtoValidator.ValidateAsync(locationDto, cancellationToken);

        if (!locationDtoResult.IsValid)
        {
            _logger.LogError("Errors occured when validating locationDto");
            return locationDtoResult.ToErrors();
        }

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
        {
            _logger.LogError("Errors occured when creating location");
            return location.Error;
        }

        _logger.LogInformation("The location has been created");

        var insertResult = await _locationsRepository.AddAsync(location.Value, cancellationToken);

        if (insertResult.IsFailure)
        {
            _logger.LogError("Failed to insert location into database");
            return insertResult.Error.ToErrors();
        }

        _logger.LogInformation("The location has been inserted into the database");

        return location.Value.Id;
    }
}
