using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.VO;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Application.Positions.Features.CreatePosition;

public class CreatePositionHandler : ICommandHandler<Guid, CreatePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<CreatePositionCommand> _validator;
    private readonly ILogger<CreatePositionHandler> _logger;

    public CreatePositionHandler(
        IPositionsRepository positionsRepository,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        IValidator<CreatePositionCommand> validator,
        ILogger<CreatePositionHandler> logger)
    {
        _positionsRepository = positionsRepository;
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(
        CreatePositionCommand command,
        CancellationToken cancellationToken)
    {
        var commandValidationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!commandValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating departmentCommand");
            return commandValidationResult.ToErrors();
        }

        var positionId = Guid.NewGuid();

        var positionName = PositionName.Create(command.Request.Name).Value;

        var departmentIds = command.Request.DepartmentIds;

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
        {
            _logger.LogError("Errors occurred when beginning transaction");
            return transactionScopeResult.Error.ToErrors();
        }

        using var transactionScope = transactionScopeResult.Value;

        var departmentsExistenceResult = await _departmentsRepository.ExistsAsync(departmentIds, cancellationToken);

        if (departmentsExistenceResult.IsFailure)
        {
            _logger.LogError("Errors occurred when checking the existence of departments by ids {Ids}",
                string.Join(",", departmentIds));
            transactionScope.Rollback();

            return departmentsExistenceResult.Error.ToErrors();
        }

        var departmentPositionList = departmentIds.Select(id =>
            DepartmentPosition.Create(id, positionId).Value);

        _logger.LogInformation("The departmentPosition has been created");

        var positionResult = Position.Create(
            positionId,
            positionName,
            command.Request.Description,
            departmentPositionList);

        if (positionResult.IsFailure)
        {
            _logger.LogError("Errors occurred when creating position");
            return positionResult.Error;
        }

        var addPositionResult = await _positionsRepository.AddAsync(positionResult.Value, cancellationToken);

        if (addPositionResult.IsFailure)
        {
            _logger.LogError("Failed to insert position into database");
            transactionScope.Rollback();

            return addPositionResult.Error.ToErrors();
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

        _logger.LogInformation("The position has been inserted into database");

        return positionId;
    }
}