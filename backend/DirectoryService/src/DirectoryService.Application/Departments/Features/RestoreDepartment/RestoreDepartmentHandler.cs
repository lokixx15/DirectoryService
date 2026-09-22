using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments.Features.RestoreDepartment;

public class RestoreDepartmentHandler : ICommandHandler<RestoreDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly HybridCache _cache;
    private readonly ILogger<RestoreDepartmentHandler> _logger;

    public RestoreDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        HybridCache cache,
        ILogger<RestoreDepartmentHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _cache = cache;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(RestoreDepartmentCommand command, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByAsync(d => d.Id == command.Id, cancellationToken);
        if (departmentResult.IsFailure)
        {
            _logger.LogError("Errors occurred when getting updated department by id {Id}", command.Id);
            return departmentResult.Error.ToErrors();
        }

        var oldParentPath = departmentResult.Value.Path;

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionScopeResult.IsFailure)
        {
            _logger.LogError("Errors occurred when beginning transaction");
            return transactionScopeResult.Error.ToErrors();
        }

        using var transactionScope = transactionScopeResult.Value;

        var restoreDepartmentResult = departmentResult.Value.Restore();
        if (restoreDepartmentResult.IsFailure)
        {
            _logger.LogError("Errors occured when restoring department");
            transactionScope.Rollback();

            return restoreDepartmentResult.Error;
        }

        var updateDescendantsPathsResult = await _departmentsRepository.UpdateDescendantsPathsAsync(
            departmentResult.Value.Path, oldParentPath, cancellationToken);
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

        await _cache.RemoveByTagAsync(CacheConstants.DEPARTMENTS_CACHE_TAG, cancellationToken);
        _logger.LogInformation("Invalidated all departments cache after restore using tag: {Tag}", CacheConstants.DEPARTMENTS_CACHE_TAG);

        return UnitResult.Success<Errors>();
    }
}
