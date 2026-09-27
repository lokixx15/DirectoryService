using CSharpFunctionalExtensions;
using DirectoryService.Application.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Positions.Features.RestorePosition;

public class RestorePositionHandler : ICommandHandler<RestorePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly HybridCache _cache;
    private readonly ILogger<RestorePositionHandler> _logger;

    public RestorePositionHandler(
        IPositionsRepository PositionsRepository,
        HybridCache cache,
        ILogger<RestorePositionHandler> logger)
    {
        _positionsRepository = PositionsRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(RestorePositionCommand command, CancellationToken cancellationToken)
    {
        var restorePositionResult = await _positionsRepository.RestorePositionByIdAsync(command.Id, cancellationToken);
        if (restorePositionResult.IsFailure)
        {
            _logger.LogError("Errors occured when restoring position");
            return restorePositionResult.Error.ToErrors();
        }

        await _cache.RemoveByTagAsync(CacheConstants.POSITIONS_CACHE_TAG, cancellationToken);
        _logger.LogInformation("Invalidated all positions cache after restoring using tag: {Tag}", CacheConstants.POSITIONS_CACHE_TAG);

        return UnitResult.Success<Errors>();
    }
}
