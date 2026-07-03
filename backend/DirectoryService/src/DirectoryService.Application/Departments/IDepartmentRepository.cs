using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.VO;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Department, Error>> GetByAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdWithLockAsync(Guid id, CancellationToken cancellationToken);

    Task<UnitResult<Error>> LockDescendants(DepartmentPath oldDepartmentPath, CancellationToken cancellationToken);

    Task<UnitResult<Error>> ExistsAsync(IEnumerable<Guid> ids, CancellationToken cancellation);

    Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken);

    Task<UnitResult<Error>> AddLocationsToDepartmentAsync(List<DepartmentLocation> locations, CancellationToken cancellationToken);

    Task<UnitResult<Error>> UpdateDescendantsParentAsync(DepartmentPath? newUpdatedDepartmentPath, DepartmentPath oldUpdatedDepartmentPath, CancellationToken cancellationToken);

    Task<UnitResult<Error>> UpdateDescendantsPathsAsync(DepartmentPath newParentPath, DepartmentPath oldParentPath, CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteLocationsByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken);
}
