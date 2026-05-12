using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace FileService.Core.Abstractions.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancelToken);
}