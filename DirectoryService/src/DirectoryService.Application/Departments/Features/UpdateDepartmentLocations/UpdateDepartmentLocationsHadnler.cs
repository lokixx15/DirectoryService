using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentLocations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsHadnler : ICommandHandler<UpdateDepartmentLocationsCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<UpdateDepartmentLocationsCommand> _validator;
    private readonly ILogger<UpdateDepartmentLocationsHadnler> _logger;

    public UpdateDepartmentLocationsHadnler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        IValidator<UpdateDepartmentLocationsCommand> validator,
        ILogger<UpdateDepartmentLocationsHadnler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(
        UpdateDepartmentLocationsCommand command,
        CancellationToken cancellationToken)
    {
        var commandValidationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!commandValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating updateDepartmentLocationsCommand");
            return commandValidationResult.ToErrors();
        }

        var departmentId = command.departmentId;
        var locationIds = command.updateDepartmentLocationsDto.LocationIds;

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
        {
            _logger.LogError("Errors occurred when beginning transaction");
            return transactionScopeResult.Error.ToErrors();
        }

        var transactionScope = transactionScopeResult.Value;

        var departmentExistenceResult = await _departmentsRepository.ExistsAsync([departmentId], cancellationToken);

        if (departmentExistenceResult.IsFailure)
        {
            _logger.LogError("Errors occurred when checking the existence of department by id {Id}",
                departmentId);
            transactionScope.Rollback();

            return departmentExistenceResult.Error.ToErrors();
        }

        var locationsExistenceResult = await _locationsRepository.ExistsAsync(locationIds, cancellationToken);

        if (locationsExistenceResult.IsFailure)
        {
            _logger.LogError("Errors occurred when checking the existence of locations by ids {Ids}",
               string.Join(", ", locationIds));
            transactionScope.Rollback();

            return locationsExistenceResult.Error.ToErrors();
        }

        var deleteLocationsResult = await _departmentsRepository.DeleteLocationsByDepartmentIdAsync(departmentId, cancellationToken);

        if (deleteLocationsResult.IsFailure)
        {
            _logger.LogError("Errors occurred during deletion locations from the department with id {Id}",
                departmentId);
            transactionScope.Rollback();

            return deleteLocationsResult.Error.ToErrors();
        }

        var departmentLocationListResult = locationIds.Select(lI => 
            DepartmentLocation.Create(departmentId, lI)).ToList();

        var departmentLocationList = departmentLocationListResult.Select(d => d.Value).ToList();

        var addLocationsResult = await _departmentsRepository.AddLocationsToDepartmentAsync(
            departmentLocationList, cancellationToken);

        if (addLocationsResult.IsFailure)
        {
            _logger.LogError("Failed to insert department locations into database");
            transactionScope.Rollback();

            return addLocationsResult.Error.ToErrors();
        }

        _logger.LogInformation("The department locations have been updated");

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Errors occurred when saving changes");
            return saveChangesResult.Error.ToErrors();
        }

        var commitResult = transactionScope.Commit();

        if (commitResult.IsFailure)
        {
            _logger.LogError("Errors occurred when committing transaction");
            return saveChangesResult.Error.ToErrors();
        }

        return UnitResult.Success<Errors>();
    }
}