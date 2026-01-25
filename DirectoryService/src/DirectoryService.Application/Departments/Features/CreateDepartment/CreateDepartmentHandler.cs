using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.VO;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.CreateDepartment;

public class CreateDepartmentHandler : ICommandHandler<Guid, CreateDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<CreateDepartmentCommand> _validator;
    private readonly ILogger<CreateDepartmentHandler> _logger;

    public CreateDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        IValidator<CreateDepartmentCommand> validator,
        ILogger<CreateDepartmentHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(
        CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var commandValidationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!commandValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating departmentCommand");
            return commandValidationResult.ToErrors();
        }

        var departmentId = Guid.NewGuid();

        var departmentName = DepartmentName.Create(command.CreateDepartmentDto.Name).Value;

        var departmentIdentifier = DepartmentIdentifier.Create(command.CreateDepartmentDto.Identifier).Value;

        var locationIds = command.CreateDepartmentDto.LocationIds;

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
        {
            _logger.LogError("Errors occurred when beginning transaction");
            return transactionScopeResult.Error.ToErrors();
        }

        using var transactionScope = transactionScopeResult.Value;

        var locationsExistenceResult = await _locationsRepository.ExistsAsync(locationIds, cancellationToken);

        if (locationsExistenceResult.IsFailure)
        {
            _logger.LogError("Errors occurred when checking the existence of locations by ids: {Ids}",
                string.Join(",", locationIds));
            transactionScope.Rollback();

            return locationsExistenceResult.Error.ToErrors();
        }

        var departmentLocationList = locationIds.Select(id =>
            DepartmentLocation.Create(departmentId, id).Value);

        _logger.LogInformation("The departmentLocation has been created");

        var parentResult = command.CreateDepartmentDto.ParentId != null
            ? await _departmentsRepository.GetByIdAsync(command.CreateDepartmentDto.ParentId.Value, cancellationToken)
            : Result.Success<Department, Error>(null!);

        if (parentResult.IsFailure)
        {
            _logger.LogError("Errors occurred when getting parent department by id {Id}",
                command.CreateDepartmentDto.ParentId);
            transactionScope.Rollback();

            return parentResult.Error.ToErrors();
        }

        var departmentResult = parentResult.Value == null
            ? Department.CreateParent(departmentId, departmentName, departmentIdentifier, departmentLocationList)
            : Department.CreateChild(departmentId, departmentName, departmentIdentifier, parentResult.Value, departmentLocationList);

        _logger.LogInformation("The department has been created with the name {Name}", departmentResult.Value.Name);

        var addDepartmentResult = await _departmentsRepository.AddAsync(departmentResult.Value, cancellationToken);

        if (addDepartmentResult.IsFailure)
        {
            _logger.LogError("Failed to insert department into database");
            transactionScope.Rollback();

            return addDepartmentResult.Error.ToErrors();
        }

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

        _logger.LogInformation("The department has been inserted into database");

        return addDepartmentResult.Value;
    }
}