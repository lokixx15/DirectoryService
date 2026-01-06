using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using SharedKernel;
using FluentValidation;
using DirectoryService.Application.Validation;
using Microsoft.Extensions.Logging;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.VO;
using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Locations.Features;

public class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateLocationDto> _createLocationDtoValidator;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(ILocationsRepository locationsRepository,
        IValidator<CreateLocationDto> createLocationDtoValidator,
        ILogger<CreateLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _createLocationDtoValidator = createLocationDtoValidator;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(
        CreateLocationCommand command, 
        CancellationToken cancellationToken)
    {
        var locationDtoResult = await _createLocationDtoValidator.ValidateAsync(command.createLocationDto, cancellationToken);

        if (!locationDtoResult.IsValid)
        {
            _logger.LogError("Errors occured when validating locationDto");
            return locationDtoResult.ToErrors();
        }

        var locationName = LocationName.Create(command.createLocationDto.Name);

        var locationAddress = LocationAddress.Create(command.createLocationDto.Address);

        var locationTimezone = LocationTimezone.Create(command.createLocationDto.Timezone);

        var location = Location.Create(
            Guid.Empty,
            locationName.Value,
            locationAddress.Value,
            locationTimezone.Value,
            command.createLocationDto.IsActive);

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
