using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using SharedKernel;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken);
    Task<UnitResult<Error>> ExistsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    Task<UnitResult<Error>> SoftDeleteLocationsWithoutActiveDepartments(Guid departmentId, CancellationToken cancellationToken);
}