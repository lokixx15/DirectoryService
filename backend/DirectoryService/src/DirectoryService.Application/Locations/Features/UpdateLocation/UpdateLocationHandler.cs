using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Domain.Locations.VO;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Locations.Features.UpdateLocation;

public sealed class UpdateLocationHandler : ICommandHandler<UpdateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<UpdateLocationCommand> _validator;
    private readonly ILogger<UpdateLocationHandler> _logger;

    public UpdateLocationHandler(
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        IValidator<UpdateLocationCommand> validator,
        ILogger<UpdateLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(
        UpdateLocationCommand command,
        CancellationToken cancellationToken)
    {
        var commandValidationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!commandValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating updateLocationCommand");
            return commandValidationResult.ToErrors();
        }

        var locationResult = await _locationsRepository.GetByAsync(l => l.Id == command.Id, cancellationToken);
        if (locationResult.IsFailure)
        {
            _logger.LogError("Errors occurred when received location");
            return locationResult.Error.ToErrors();
        }

        var locationName = LocationName.Create(command.Request.Name).Value;

        var locationAddress = LocationAddress.Create(
            command.Request.Address.Country,
            command.Request.Address.City,
            command.Request.Address.Street,
            command.Request.Address.Building,
            command.Request.Address.Region,
            command.Request.Address.District,
            command.Request.Address.Apartment).Value;

        var locationTimezone = LocationTimezone.Create(command.Request.Timezone).Value;

        locationResult.Value.Update(
            locationName,
            locationAddress,
            locationTimezone);

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Errors occurred when saving changes");
            return saveChangesResult.Error.ToErrors();
        }

        _logger.LogInformation("The location has been inserted into database");

        return UnitResult.Success<Errors>();
    }
}