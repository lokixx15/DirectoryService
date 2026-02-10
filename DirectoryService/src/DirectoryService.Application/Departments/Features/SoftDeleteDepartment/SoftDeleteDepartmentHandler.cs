using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Positions;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.SoftDeleteDepartment;

public class SoftDeleteDepartmentHandler 
    : ICommandHandler<SoftDeleteDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IPositionsRepository _positionsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<SoftDeleteDepartmentHandler> _logger;
    
    public SoftDeleteDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        IPositionsRepository positionsRepository,
        ITransactionManager transactionManager,
        ILogger<SoftDeleteDepartmentHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(SoftDeleteDepartmentCommand command, CancellationToken cancellationToken)
    {
        var departmentId = command.departmentId;

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
        {
            _logger.LogError("Errors occurred when beginning transaction");
            return transactionScopeResult.Error.ToErrors();
        }

        using var transactionScope = transactionScopeResult.Value;

        var department = await _departmentsRepository.GetByIdWithLockAsync(departmentId, cancellationToken);

        if (department.IsFailure)
        {
            _logger.LogError("Errors occurred when getting updated department by id {Id}", departmentId);
            return department.Error.ToErrors();
        }

        var oldDepartmentPath = department.Value.Path;

        var lockDescendantsResult = await _departmentsRepository.LockDescendants(oldDepartmentPath, cancellationToken);

        if (lockDescendantsResult.IsFailure)
        {
            _logger.LogError("Errors occurred when locking department's descendants with the path {Path}", oldDepartmentPath);
            return lockDescendantsResult.Error.ToErrors();
        }

        var softDeleteDepartmentResult = department.Value.SoftDelete();

        if (softDeleteDepartmentResult.IsFailure)
        {
            _logger.LogError("Errors occurred when soft deleting department");
            transactionScope.Rollback();

            return softDeleteDepartmentResult.Error;
        }

        var softDeleteLocationsWithoutActiveDepartmentsResult = await _locationsRepository
            .SoftDeleteLocationsWithoutActiveDepartments(departmentId, cancellationToken);

        if (softDeleteLocationsWithoutActiveDepartmentsResult.IsFailure)
        {
            _logger.LogError("Errors occurred when soft deleting locations without active departments");
            transactionScope.Rollback();

            return softDeleteLocationsWithoutActiveDepartmentsResult.Error.ToErrors();
        }

        var softDeletePositionsWithoutActiveDepartmentsResult = await _positionsRepository
            .SoftDeletePositionsWithoutActiveDepartments(departmentId, cancellationToken);

        if (softDeletePositionsWithoutActiveDepartmentsResult.IsFailure)
        {
            _logger.LogError("Errors occurred when soft deleting positions without active departments");
            transactionScope.Rollback();

            return softDeletePositionsWithoutActiveDepartmentsResult.Error.ToErrors();
        }

        var updateDescendantsPathsResult = await _departmentsRepository
            .UpdateDescendantsPathsAsync(department.Value.Path, oldDepartmentPath, cancellationToken);

        if (updateDescendantsPathsResult.IsFailure)
        {
            _logger.LogError("Errors occurred when updating descendants paths after soft deleting parent department");
            transactionScope.Rollback();

            return updateDescendantsPathsResult.Error.ToErrors();
        }

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Errors occurred when saving changes");
            transactionScope.Rollback();

            return saveChangesResult.Error.ToErrors();
        }

        var commitResult = transactionScope.Commit();

        if (commitResult.IsFailure)
        {
            _logger.LogError("Errors occurred when committing transaction");
            return commitResult.Error.ToErrors();
        }

        return UnitResult.Success<Errors>();
    }
}