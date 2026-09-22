using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Locations.Features.SoftDeleteLocation;

public class SoftDeleteLocationHandler : ICommandHandler<SoftDeleteLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<SoftDeleteLocationHandler> _logger;

    public SoftDeleteLocationHandler(
        ILocationsRepository locationsRepository,
        ILogger<SoftDeleteLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
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

        return UnitResult.Success<Errors>();
    }
}
