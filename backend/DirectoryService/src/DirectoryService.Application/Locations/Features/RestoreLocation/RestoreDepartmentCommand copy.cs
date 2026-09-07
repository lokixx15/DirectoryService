using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Locations.Features.RestoreLocation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments.Features.RestoreDepartment;

public class RestoreLocationHandler : ICommandHandler<RestoreLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<RestoreLocationHandler> _logger;

    public RestoreLocationHandler(
        ILocationsRepository locationsRepository,
        ILogger<RestoreLocationHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(RestoreLocationCommand command, CancellationToken cancellationToken)
    {
        var restoreLocationResult = await _locationsRepository.RestoreLocationByIdAsync(command.Id, cancellationToken);
        if (restoreLocationResult.IsFailure)
        {
            _logger.LogError("Errors occured when restoring location");
            return restoreLocationResult.Error.ToErrors();
        }

        return UnitResult.Success<Errors>();
    }
}
