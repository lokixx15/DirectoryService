using CSharpFunctionalExtensions;
using DirectoryService.Domain.Positions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Positions;

public interface IPositionsRepository
{
    Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken);

    Task<UnitResult<Error>> SoftDeletePositionsWithoutActiveDepartments(Guid departmentId, CancellationToken cancellationToken);

    Task<UnitResult<Error>> RestorePositionByIdAsync(Guid positionId, CancellationToken cancellationToken);
}
