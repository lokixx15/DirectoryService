using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Caching;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.VO;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.CreateDepartment;

public sealed class CreateDepartmentHandler : ICommandHandler<Guid, CreateDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<CreateDepartmentCommand> _validator;
    private readonly HybridCache _cache;
    private readonly ILogger<CreateDepartmentHandler> _logger;

    public CreateDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        IValidator<CreateDepartmentCommand> validator,
        HybridCache cache,
        ILogger<CreateDepartmentHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _cache = cache;
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

        var departmentName = DepartmentName.Create(command.Request.Name).Value;

        var departmentIdentifier = DepartmentIdentifier.Create(command.Request.Identifier).Value;

        var locationIds = command.Request.LocationIds;

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

        var parentResult = command.Request.ParentId != null
            ? await _departmentsRepository.GetByAsync(d => d.Id == command.Request.ParentId.Value, cancellationToken)
            : Result.Success<Department, Error>(null!);

        if (parentResult.IsFailure)
        {
            _logger.LogError("Errors occurred when getting parent department by id {Id}",
                command.Request.ParentId);
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

        await _cache.RemoveByTagAsync(CacheConstants.DEPARTMENTS_CACHE_TAG, cancellationToken);

        _logger.LogInformation("Invalidated all departments cache using tag: {Tag}", CacheConstants.DEPARTMENTS_CACHE_TAG);

        return addDepartmentResult.Value;
    }
}