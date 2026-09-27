using CSharpFunctionalExtensions;
using DirectoryService.Application.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Locations.Features.SoftDeleteLocation;

public class SoftDeleteLocationHandler : ICommandHandler<SoftDeleteLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly HybridCache _cache;
    private readonly ILogger<SoftDeleteLocationHandler> _logger;

    public SoftDeleteLocationHandler(
        ILocationsRepository locationsRepository,
        HybridCache cache,
        ILogger<SoftDeleteLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(
        SoftDeleteLocationCommand command,
        CancellationToken cancellationToken)
    {
        var softDeleteLocationResult = await _locationsRepository.SoftDeleteByIdAsync(command.Id, cancellationToken);
        if (softDeleteLocationResult.IsFailure)
        {
            _logger.LogError("Errors occurred when deleting location");
            return softDeleteLocationResult.Error.ToErrors();
        }

        await _cache.RemoveByTagAsync(CacheConstants.LOCATIONS_CACHE_TAG, cancellationToken);
        _logger.LogInformation("Invalidated all locations cache after deletion using tag: {Tag}", CacheConstants.LOCATIONS_CACHE_TAG);

        return UnitResult.Success<Errors>();
    }
}
