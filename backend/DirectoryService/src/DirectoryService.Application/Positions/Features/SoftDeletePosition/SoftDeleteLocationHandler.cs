using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Positions.Features.SoftDeletePosition;

public class SoftDeletePositionHandler : ICommandHandler<SoftDeletePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly ILogger<SoftDeletePositionHandler> _logger;

    public SoftDeletePositionHandler(
        IPositionsRepository PositionsRepository,
        ILogger<SoftDeletePositionHandler> logger)
    {
        _positionsRepository = PositionsRepository;
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

        return UnitResult.Success<Errors>();
    }
}
