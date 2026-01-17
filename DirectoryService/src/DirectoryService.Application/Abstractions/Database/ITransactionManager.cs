using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Application.Abstractions.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken);
    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);
}