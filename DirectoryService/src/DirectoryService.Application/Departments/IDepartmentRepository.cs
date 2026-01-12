using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using SharedKernel;

namespace DirectoryService.Application.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Department, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UnitResult<Error>> ExistsAsync(IEnumerable<Guid> ids, CancellationToken cancellation);
    Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken);
}
