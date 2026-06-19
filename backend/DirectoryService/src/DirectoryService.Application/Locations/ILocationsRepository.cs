using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Result<Location, Error>> GetByAsync(Expression<Func<Location, bool>> predicate, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken);

    Task<UnitResult<Error>> ExistsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<UnitResult<Error>> SoftDeleteLocationsWithoutActiveDepartments(Guid departmentId, CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteDepartmentLocationsByLocationIdAsync(Guid locationId, CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteByIdAsync(Guid id, CancellationToken cancellationToken);
}