using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.UpdateDepartmentParent;

public class UpdateDepartmentParentHandler : ICommandHandler<UpdateDepartmentParentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<UpdateDepartmentParentCommand> _validator;
    private readonly ILogger<UpdateDepartmentParentHandler> _logger;

    public UpdateDepartmentParentHandler(
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        IValidator<UpdateDepartmentParentCommand> validator, 
        ILogger<UpdateDepartmentParentHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(
        UpdateDepartmentParentCommand command,
        CancellationToken cancellationToken)
    {
        var commandValidationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!commandValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating updateDepartmentParentCommand");
            return commandValidationResult.ToErrors();
        }

        var departmentId = command.departmentId;
        var departmentParentId = command.updateDepartmentParentDto.parentId;

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
        {
            _logger.LogError("Errors occurred when beginning transaction");
            return transactionScopeResult.Error.ToErrors();
        }

        using var transactionScope = transactionScopeResult.Value;

        var udpdatedDepartmentResult = await _departmentsRepository.GetByIdWithLockAsync(departmentId, cancellationToken);

        if (udpdatedDepartmentResult.IsFailure)
        {
            _logger.LogError("Errors occurred when getting updated department by id {Id}", departmentId);
            return udpdatedDepartmentResult.Error.ToErrors();
        }

        var updatedDepartment = udpdatedDepartmentResult.Value;
        var oldDepartmentPath = updatedDepartment.Path;

        var lockDescendantsResult = await _departmentsRepository.LockDescendants(oldDepartmentPath, cancellationToken);

        if (lockDescendantsResult.IsFailure)
        {
            _logger.LogError("Errors occurred when locking department's descendants with the path {Path}", oldDepartmentPath);
            return lockDescendantsResult.Error.ToErrors();
        }

        var newParentDepartmentResult = departmentParentId is null
            ? Result.Success<Department, Error>(null!)
            : await _departmentsRepository.GetByIdAsync(departmentParentId.Value, cancellationToken);

        if (newParentDepartmentResult.IsFailure)
        {
            _logger.LogError("Errors occurred when getting new parent department by id {Id}", departmentParentId);
            return udpdatedDepartmentResult.Error.ToErrors();
        }

        var newParentDepartment = newParentDepartmentResult.Value;

        var updateDepartmentResult = updatedDepartment.UpdateParent(newParentDepartment);

        if (updateDepartmentResult.IsFailure)
        {
            _logger.LogError("Errors occurred when updating department");
            transactionScope.Rollback();

            return updateDepartmentResult.Error;
        }

        var updateDepartmentDescedantsParentResult = await _departmentsRepository.UpdateDepartmentDescendantsParentAsync(
            updatedDepartment.Path, oldDepartmentPath, cancellationToken);

        if (updateDepartmentDescedantsParentResult.IsFailure)
        {
            _logger.LogError("Errors occurred when updating department's descedants");
            transactionScope.Rollback();

            return udpdatedDepartmentResult.Error.ToErrors();
        }

        _logger.LogInformation("The department parent has been updated");

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