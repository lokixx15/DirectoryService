using CSharpFunctionalExtensions;
using DirectoryService.Application.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Positions.Features.SoftDeletePosition;

public class SoftDeletePositionHandler : ICommandHandler<SoftDeletePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly HybridCache _cache;
    private readonly ILogger<SoftDeletePositionHandler> _logger;

    public SoftDeletePositionHandler(
        IPositionsRepository PositionsRepository,
        HybridCache cache,
        ILogger<SoftDeletePositionHandler> logger)
    {
        _positionsRepository = PositionsRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(
        SoftDeletePositionCommand command,
        CancellationToken cancellationToken)
    {
        var softDeletePositionResult = await _positionsRepository.SoftDeleteByIdAsync(command.Id, cancellationToken);
        if (softDeletePositionResult.IsFailure)
        {
            _logger.LogError("Errors occurred when deleting Position");
            return softDeletePositionResult.Error.ToErrors();
        }

        await _cache.RemoveByTagAsync(CacheConstants.POSITIONS_CACHE_TAG, cancellationToken);
        _logger.LogInformation("Invalidated all positions cache after deletion using tag: {Tag}", CacheConstants.POSITIONS_CACHE_TAG);

        return UnitResult.Success<Errors>();
    }
}
