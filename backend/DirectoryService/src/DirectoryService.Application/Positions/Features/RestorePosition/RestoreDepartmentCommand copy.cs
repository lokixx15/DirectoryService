using CSharpFunctionalExtensions;
using DirectoryService.Application.Positions;
using DirectoryService.Application.Positions.Features.RestorePosition;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments.Features.RestoreDepartment;

public class RestorePositionHandler : ICommandHandler<RestorePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly ILogger<RestorePositionHandler> _logger;

    public RestorePositionHandler(
        IPositionsRepository PositionsRepository,
        ILogger<RestorePositionHandler> logger)
    {
        _positionsRepository = PositionsRepository;
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

        return UnitResult.Success<Errors>();
    }
}
