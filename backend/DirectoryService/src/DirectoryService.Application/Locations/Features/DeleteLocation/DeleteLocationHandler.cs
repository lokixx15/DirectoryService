using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Database;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Locations.Features.DeleteLocation;

public class DeleteLocationHandler : ICommandHandler<DeleteLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<DeleteLocationHandler> _logger;

    public DeleteLocationHandler(
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        ILogger<DeleteLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(
        DeleteLocationCommand command,
        CancellationToken cancellationToken)
    {
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
        {
            _logger.LogError("Errors occurred when beginning transaction");
            return transactionScopeResult.Error.ToErrors();
        }

        using var transactionScope = transactionScopeResult.Value;

        var deleteDepartmentLocationsResult = await _locationsRepository
            .DeleteDepartmentLocationsByLocationIdAsync(command.Id, cancellationToken);
        if (deleteDepartmentLocationsResult.IsFailure)
        {
            _logger.LogError("Errors occurred when deleting department locations");
            return deleteDepartmentLocationsResult.Error.ToErrors();
        }

        var deleteLocationResult = await _locationsRepository.DeleteByIdAsync(command.Id, cancellationToken);
        if (deleteLocationResult.IsFailure)
        {
            _logger.LogError("Errors occurred when deleting location");
            return deleteLocationResult.Error.ToErrors();
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