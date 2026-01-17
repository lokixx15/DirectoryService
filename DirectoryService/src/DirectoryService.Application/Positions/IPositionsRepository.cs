using CSharpFunctionalExtensions;
using DirectoryService.Domain.Positions;
using SharedKernel;

namespace DirectoryService.Application.Positions;

public interface IPositionsRepository
{
    Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken);
}