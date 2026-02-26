using CSharpFunctionalExtensions;
using SharedKernel;
using FluentValidation;
using DirectoryService.Application.Validation;
using Microsoft.Extensions.Logging;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.VO;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;

namespace DirectoryService.Application.Locations.Features.CreateLocation;

public sealed class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<CreateLocationCommand> _validator;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        IValidator<CreateLocationCommand> validator,
        ILogger<CreateLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(
        CreateLocationCommand command, 
        CancellationToken cancellationToken)
    {
        var commandValidationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!commandValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating locationCommand");
            return commandValidationResult.ToErrors();
        }

        var locationName = LocationName.Create(command.Request.Name).Value;

        var locationAddress = LocationAddress.Create(
            command.Request.AddressRequest.Country,
            command.Request.AddressRequest.City,
            command.Request.AddressRequest.Street,
            command.Request.AddressRequest.Building,
            command.Request.AddressRequest.Region,
            command.Request.AddressRequest.District,
            command.Request.AddressRequest.Apartment).Value;

        var locationTimezone = LocationTimezone.Create(command.Request.Timezone).Value;

        var locationResult = Location.Create(
            Guid.Empty,
            locationName,
            locationAddress,
            locationTimezone);

        if (locationResult.IsFailure)
        {
            _logger.LogError("Errors occurred when creating location");
            return locationResult.Error;
        }

        _logger.LogInformation("The location has been created");

        var addLocationResult = await _locationsRepository.AddAsync(locationResult.Value, cancellationToken);

        if (addLocationResult.IsFailure)
        {
            _logger.LogError("Failed to insert location into database");
            return addLocationResult.Error.ToErrors();
        }

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Errors occurred when saving changes");
            return saveChangesResult.Error.ToErrors();
        }

        _logger.LogInformation("The location has been inserted into database");

        return addLocationResult.Value;
    }
}